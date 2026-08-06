namespace PMDDesktop.Server.RequestHandlers;

internal class UserRequestException(string message, Exception? innerException = null) : Exception(message, innerException)
{



}
