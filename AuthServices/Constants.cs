namespace MauiApp4.AuthServices;

public static class Constants
{
	public static readonly string ClientId = "null";
	public static readonly string[] Scopes = { "offline_access", "openid" };
	public static readonly string TenantName = "null";
	public static readonly string TenantId = "null";
	public static readonly string SignInPolicy = "null";
    public static readonly string AuthorityBase = $"https://{TenantName}.b2clogin.com/tfp/{TenantId}/";
	public static readonly string AuthoritySignIn = $"{AuthorityBase}{SignInPolicy}";
	public static readonly string IosKeychainSecurityGroups = "com.microsoft.adalcache";
}