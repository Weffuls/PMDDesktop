using PMDDesktop.Requests;
using PMDDesktop.Results;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Utils;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PMDDesktop.Server.RequestHandlers;

public abstract class JsonRequestHandler<TReq, TRes> : IRequestHandler where TReq : ServerRequest<TRes> where TRes : ServerJsonResponse
{

	private MemberInfo[] queryParameters;

	protected JsonRequestHandler()
	{

		queryParameters = [.. GetRequestType().GetMembersWithAttribute<QueryParameterAttribute>()];

	}

	public Type GetRequestType()
	{
		return typeof(TReq);
	}

	/// <summary>
	/// Creates a JSON document by reading from the request body and combining that with the query strings.
	/// </summary>
	/// <param name="context">HttpContext of the request</param>
	/// <returns></returns>
	private async Task<Result<TReq, ResponseResultError>> GetJsonObject(HttpContext context)
	{

		JsonObject json;

		// Create the inital JsonObject
		if (context.Request.Method == "GET")
		{

			// For GET requests, the body should be ignored, even if it exists. So don't even bother checking it.
			json = (JsonObject?)JsonNode.Parse("{}")
				?? throw new Exception("Parsed JsonObject was null. This shouldn't happen under normal circumstances.");

		}
		else
		{

			JsonNode? node;

			try
			{
				node = await JsonNode.ParseAsync(context.Request.Body);
			}
			catch (JsonException e)
			{
				return new(new ResponseResultRequestJsonParseError(e));
			}
			// Other exceptions should be unhandled. They'll be caught by HandleRequest as a 500 error.

			if (node is not JsonObject parsedJson)
				return new(new ResponseResultRequestJsonWrongTypeError(JsonValueKind.Object, node?.GetValueKind()));

			json = parsedJson;

		}

		// Reject any JSON fields that are supposed to be query parameters.
		foreach (MemberInfo member in queryParameters)
		{

			if (json.ContainsKey(member.Name))
				return new(new ResponseResultRequestQueryMisplacedError(member.Name));

		}

		List<MemberInfo> acceptableQueries = [.. queryParameters];

		// Add query fields to the JsonObject
		foreach (var query in context.Request.Query)
		{

			string key = query.Key;

			// Get the member we're looking at, and remove it from our query list.
			MemberInfo? member = acceptableQueries.Find((member) => member.Name == key);
			if (member is null)
				return new(new ResponseResultRequestQueryUnexpectedError(key));

			acceptableQueries.Remove(member);

			if (query.Value.Count != 1) // We only allow one of each query.
			{
				return new(new ResponseResultRequestQueryDuplicateError(key, query.Value.Count));
			}

			// Get that value, now that we know it's a singleton, and parse it into a node.
			string value = query.Value[0]
				?? throw new IndexOutOfRangeException($"This should not be possible, but indexing for the first query value of '{key}' was out of range.");

			JsonNode? node;

			try
			{
				node = JsonNode.Parse(value);
			}
			catch (JsonException e)
			{
				return new(new ResponseResultRequestJsonParseError(e, key));
			}

			json.Add(key, node);

		}

		// All remaining query parameters are to be set as null.
		foreach (MemberInfo nullQuery in acceptableQueries)
			json.Add(nullQuery.Name, null);

		// Finally, try to deserialize the object.
		TReq? deserialized;
		try
		{
			deserialized = json.Deserialize<TReq>(AppInfo.NETWORK_JSON_OPTIONS);
		}
		catch(JsonException e)
		{
			return new(new ResponseResultRequestJsonParseError(e));
		}

		if (deserialized is null)
			return new(new ResponseResultRequestJsonWrongTypeError(JsonValueKind.Object, JsonValueKind.Null));

		return new(deserialized);

	}

	public async Task<ResponseJsonResult<TRes, ResponseResultError>> DetermineResult(HttpContext context, GameServer game)
	{

		try
		{

			// This is responsible for all the logic that goes into creating a .NET object to use in the request handler.
			Result<TReq, ResponseResultError> deserializedResult = await GetJsonObject(context);
			if (!deserializedResult.TryResult(out TReq? deserialized, out ResponseResultError? error))
				return new(error);

			// This tries to return a User object if we could log them in using their token.
			if (IRequestHandler.TryGetUserFromContext(game.State.Users, context, out User? user, out UserAccessToken? token))
				if (token.NeedsRefresh())
					await token.RefreshToken();

			// This holds all the actual logic for responding to this specific request.
			return await CreateResponse(deserialized, game, user);

		}
		catch (Exception e)
		{

			return new(new ResponseResultExceptionError(e));

		}

	}

	public async Task HandleRequest(HttpContext context, GameServer game)
	{

		// Main chunk of the logic is inside DetermineResult.
		ResponseJsonResult<TRes, ResponseResultError> result = await DetermineResult(context, game);

		if (!result.TryResult(out TRes? success, out ResponseResultError? error))
		{
			context.Response.ContentType = "text/plain";
			context.Response.StatusCode = error.GetReturnCode();
			byte[] messageBytes = Encoding.UTF8.GetBytes(error.GetErrorReason());
			await context.Response.BodyWriter.WriteAsync(messageBytes);
			return;
		}

		context.Response.ContentType = "application/json";
		context.Response.StatusCode = 200;

		await context.Response.WriteAsJsonAsync(success, AppInfo.NETWORK_JSON_OPTIONS);

		return;

	}

	protected abstract Task<ResponseJsonResult<TRes, ResponseResultError>> CreateResponse(TReq request, GameServer game, User? requestingUser);

}
