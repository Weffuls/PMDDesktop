using Microsoft.AspNetCore.Http.HttpResults;
using PMDDesktop.Requests;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Utils;
using System.Buffers;
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
	/// <exception cref="UserRequestException"></exception>
	private async Task<JsonObject> GetJsonObject(HttpContext context)
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

			try
			{
				json = (JsonObject?)await JsonNode.ParseAsync(context.Request.Body)
					?? throw new Exception("Parsed JsonNode was not a JsonObject or otherwise null."); // TODO: Find a way to do this without throwing
			}
			catch (JsonException e)
			{
				throw new Exception($"Error occurred while parsing JSON: {e.Message}", e); // TODO: Find a way to do this without throwing
			}
			// Other exceptions should be unhandled. They'll be caught by HandleRequest as a 500 error.

		}

		// Reject any JSON fields that are supposed to be query parameters.
		foreach (MemberInfo member in queryParameters)
		{

			if (json.ContainsKey(member.Name))
				throw new Exception($"'{member}' should be passed as a query parameter (url components after the ?) and not inside the JSON object."); // TODO: Find a way to do this without throwing

		}

		List<MemberInfo> acceptableQueries = [.. queryParameters];

		// Add query fields to the JsonObject
		foreach (var query in context.Request.Query)
		{

			string key = query.Key;

			// Get the member we're looking at, and remove it from our query list.
			MemberInfo member = acceptableQueries.Find((member) => member.Name == key)
				?? throw new Exception($"Wasn't expecting a query with key '{key}'"); // TODO: Find a way to do this without throwing
			acceptableQueries.Remove(member);

			if (query.Value.Count != 1) // We only allow one of each query.
			{
				throw new Exception($"Cannot handle {query.Value.Count} values for {key}"); // TODO: Find a way to do this without throwing
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
				throw new Exception($"Error occurred while parsing query '{key}' as JSON: {e.Message}", e); // TODO: Find a way to do this without throwing
			}

			json.Add(key, node);

		}

		// All remaining query parameters are to be set as null.
		foreach (MemberInfo nullQuery in acceptableQueries)
			json.Add(nullQuery.Name, null);

		return json;

	}

	public async Task HandleRequest(HttpContext context, GameServer game)
	{

		JsonObject json;

		ResponseJsonResult<TRes, ResponseResultError> result;

		try
		{

			json = await GetJsonObject(context);

			TReq deserialized = json.Deserialize<TReq>(AppInfo.NETWORK_JSON_OPTIONS)
					?? throw new Exception($"Deserialized data from {context} was null.");

			if (IRequestHandler.TryGetUserFromContext(game.State.Users, context, out User? user, out UserAccessToken? token))
				if (token.NeedsRefresh())
					await token.RefreshToken();

			result = await CreateResponse(deserialized, game, user);

		}
		catch (Exception e)
		{

			result = new(new ResponseResultExceptionError(e));

		}

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
