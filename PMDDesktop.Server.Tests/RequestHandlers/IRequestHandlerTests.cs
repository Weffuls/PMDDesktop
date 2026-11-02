using PMDDesktop.Requests;
using PMDDesktop.Server.RequestHandlers;
using PMDDesktop.Utils;

namespace PMDDesktop.Server.Tests.RequestHandlers;

public class IRequestHandlerTests
{

	/// <summary>
	/// This test makes sure that every <see cref="ServerRequest{}"/> has exactly one corresponding <see cref="IRequestHandler"/>.
	/// </summary>
	[Fact]
	public void AllRequestsHaveHandlers()
	{

		IEnumerable<Type> handlerTypes = TypeUtils.GetInstanceableClassesAssignableTo(typeof(IRequestHandler));

		List<Type> handledRequestTypes = [];

		foreach (Type type in handlerTypes)
		{

			IRequestHandler handler =
				((IRequestHandler?)Activator.CreateInstance(type))
				?? throw new InvalidCastException($"Unable to cast instance of {type} to {typeof(IRequestHandler).FullName}");

			Type requestType = handler.GetRequestType();

			handledRequestTypes.Add(requestType);

		}

		Assert.Distinct(handledRequestTypes);

		List<Type> allRequests = [.. TypeUtils.GetInstanceableClassesAssignableTo(typeof(ServerRequest<>))];

		Assert.All(allRequests, (req) => handledRequestTypes.Contains(req));

	}

}
