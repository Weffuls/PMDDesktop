using PMDDesktop.Requests;
using PMDDesktop.Server.Game;
using PMDDesktop.Utils;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PMDDesktop.Server.RequestHandlers;

public abstract class JsonRequestHandler<TReq, TRes> : IRequestHandler where TReq : ServerRequest<TRes> where TRes : ServerResponse
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
					?? throw new UserRequestException("Parsed JsonNode was not a JsonObject or otherwise null.");
			}
			catch (JsonException e)
			{
				throw new UserRequestException($"Error occurred while parsing JSON: {e.Message}", e);
			}
			// Other exceptions should be unhandled. They'll be caught by HandleRequest as a 500 error.

		}

		// Reject any JSON fields that are supposed to be query parameters.
		foreach (MemberInfo member in queryParameters)
		{

			if (json.ContainsKey(member.Name))
				throw new UserRequestException($"'{member}' should be passed as a query parameter (url components after the ?) and not inside the JSON object.");

		}

		List<MemberInfo> acceptableQueries = [.. queryParameters];

		// Add query fields to the JsonObject
		foreach (var query in context.Request.Query)
		{

			string key = query.Key;

			// Get the member we're looking at, and remove it from our query list.
			MemberInfo member = acceptableQueries.Find((member) => member.Name == key)
				?? throw new UserRequestException($"Wasn't expecting a query with key '{key}'");
			acceptableQueries.Remove(member);

			if (query.Value.Count != 1) // We only allow one of each query.
			{
				throw new UserRequestException($"Cannot handle {query.Value.Count} values for {key}");
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
				throw new UserRequestException($"Error occurred while parsing query '{key}' as JSON: {e.Message}", e);
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

		try
		{

			json = await GetJsonObject(context);

		}
		catch (UserRequestException e) // Intended for user-facing errors, like bad inputs.
		{

			await WritePlainText(context, 400, e.Message);
			return;

		}
		catch (Exception e)
		{

			await WritePlainText(context, 500, "Internal error while creating json object: " + e.Message);
			return;

		}

		TReq deserialized;

		try
		{

			deserialized = json.Deserialize<TReq>(AppInfo.NETWORK_JSON_OPTIONS)
				?? throw new Exception($"Deserialized data from {context} was null.");

		}
		catch (Exception e)
		{

			await WritePlainText(context, 400, "Error while deserializing JSON: " + e.Message);
			return;

		}

		try
		{

			context.Response.StatusCode = 200;
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsJsonAsync(CreateResponse(deserialized, game), AppInfo.NETWORK_JSON_OPTIONS);

		}
		catch (UserRequestException e) // Intended for user-facing errors, like bad inputs, 
		{

			await WritePlainText(context, 400, e.Message);
			return;

		}
		catch (Exception e)
		{

			await WritePlainText(context, 500, "Internal error while processing request: " + e.Message);
			return;

		}

	}

	/// <summary>
	/// Helper function to write a plain text response. Intended for error messages.
	/// </summary>
	/// <param name="context">Context to write to.</param>
	/// <param name="code">Code to return.</param>
	/// <param name="errorMessage">Message to send as a response.</param>
	/// <returns></returns>
	private static async Task WritePlainText(HttpContext context, int code, string errorMessage)
	{

		context.Response.StatusCode = code;
		context.Response.ContentType = "text/plain";
		await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(errorMessage));
		return;

	}

	protected abstract TRes CreateResponse(TReq request, GameServer game);

}
