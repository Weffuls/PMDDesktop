using PMDDesktop.Server.RequestHandlers;

namespace PMDDesktop.Server.Game;

public static class WebApplicationCreator
{

	private static WebApplicationOptions OPTIONS = new()
	{

	};

	/// <summary>
	/// Creates a WebApplication with our defined configuration by using WebApplicationBuilder.
	/// </summary>
	/// <returns></returns>
	private static WebApplication GetFromBuilder()
	{

		WebApplicationBuilder builder = WebApplication.CreateBuilder(OPTIONS);

		return builder.Build();

	}

	internal static WebApplication CreateWebApplication()
	{

		WebApplication webApp = GetFromBuilder();

		webApp.UseStatusCodePages();

		return webApp;

	}

	internal static void PrepareWebApplication(GameServer server)
	{

		IRequestHandler.AddApiPoints(server);

	}

}
