namespace PMDDesktop.Server.Users;

public sealed record class UserCreationOptions
{

	public required string DisplayName {get; set;}
	public string? PlainTextPassword {get; set;}
	public string? LoginHandle {get; set;}

}
