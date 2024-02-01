namespace MauiApp4;

using MauiApp4.AuthServices;
using Microsoft.Identity.Client;

public static class DependencyExtensions
{
	public static void RegisterServices(this IServiceCollection services)
	{

#if ANDROID
        var b2cClientApplicationBuilder = PublicClientApplicationBuilder.Create(Constants.ClientId).WithParentActivityOrWindow(() => Platform.CurrentActivity);

        services.AddSingleton(new AuthServiceB2C(
            b2cClientApplicationBuilder
                .WithIosKeychainSecurityGroup(Constants.IosKeychainSecurityGroups)
                .WithB2CAuthority(Constants.AuthoritySignIn)
                .Build()));
#endif
#if WINDOWS
        var b2cClientApplicationBuilder = PublicClientApplicationBuilder.Create(Constants.ClientId).WithRedirectUri($"msal{Constants.ClientId}://auth");

        services.AddSingleton(new AuthServiceB2C(
			b2cClientApplicationBuilder
				.WithIosKeychainSecurityGroup(Constants.IosKeychainSecurityGroups)
				.WithB2CAuthority(Constants.AuthoritySignIn)
				.Build()));
#endif

      

	}
}