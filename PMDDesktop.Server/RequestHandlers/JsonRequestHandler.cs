using PMDDesktop.Requests;
using PMDDesktop.Server.Game;
using System.Text;
using System.Text.Json;

namespace PMDDesktop.Server.RequestHandlers;

public abstract class JsonRequestHandler<TReq, TRes> : IRequestHandler where TReq : ServerRequest<TRes> where TRes : ServerResponse
{

	public async Task HandleRequest(HttpContext context, GameServer game)
	{

		TReq deserialized;

		try
		{

			deserialized = await JsonSerializer.DeserializeAsync<TReq>(context.Request.Body, AppInfo.NETWORK_JSON_OPTIONS)
				?? throw new Exception($"Deserialized data from {context} was not a {typeof(TReq).FullName}.");

		}
		catch (Exception e)
		{

			context.Response.StatusCode = 400;
			context.Response.ContentType = "text/plain";
			await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(e.ToString()));

			return;

		}

		try
		{

			context.Response.StatusCode = 200;
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsJsonAsync(CreateResponse(deserialized, game), AppInfo.NETWORK_JSON_OPTIONS);

		}
		catch (Exception e)
		{

			context.Response.StatusCode = 500;
			context.Response.ContentType = "text/plain";
			await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(e.ToString()));

			return;

		}

	}

	protected abstract TRes CreateResponse(TReq request, GameServer game);

}
