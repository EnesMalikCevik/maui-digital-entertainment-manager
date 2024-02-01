namespace MauiApp4;
using MauiApp4.AuthServices;

public class AzureB2CPage : AuthPage
{
	public AzureB2CPage(AuthServiceB2C authServiceB2C) : base(authServiceB2C)
	{
		Title = "Welcome to OmniScreenHub";
	}
}