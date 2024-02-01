using System.IdentityModel.Tokens.Jwt;
using System.Text;
using MauiApp4.AuthServices;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Identity.Client;
using System.Diagnostics;
using Microsoft.Maui.Graphics;
using MauiApp4.Views;
using MauiApp4.ViewModels;


namespace MauiApp4;

public abstract class AuthPage : ContentPage
{
	private readonly IAuthService authService;
	private readonly Button loginButton;
	private readonly Button logoutButton;
    private readonly Button contentsButton;
    private readonly Image logoImage;

    protected AuthPage(IAuthService authService)
	{
		this.authService = authService;

        logoImage = new Image
        {
            Source = "application_logo.png",
            Aspect = Aspect.AspectFit,
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 30, 0, 0),
            WidthRequest = 450,
            HeightRequest = 270,
        };

        loginButton = new Button
        {
            Text = "Login",
            
            FontSize = 30,
			Command = new AsyncRelayCommand(OnLoginClicked),
            BackgroundColor = new Color(148, 0, 211),
            WidthRequest = 160,
            HeightRequest = 64
        };
		logoutButton = new Button
		{
			Text = "Logout",
			IsVisible = false,
            BackgroundColor = new Color(205, 92, 92),
            Command = new AsyncRelayCommand(OnLogoutClicked),
            WidthRequest = 100,
            HeightRequest = 40
        };
        contentsButton = new Button
        {
            Text = "Contents",
            FontSize = 30,
            IsVisible = false,
            BackgroundColor = new Color(0, 180, 0),
        Command = new AsyncRelayCommand(OnContentsClicked),
            WidthRequest = 160,
            HeightRequest = 64
        };
     

        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        Grid.SetRow(logoImage, 0);
        grid.Children.Add(logoImage);
        Grid.SetRow(loginButton, 1);
        grid.Children.Add(loginButton);
        Grid.SetRow(logoutButton, 2);
        grid.Children.Add(logoutButton);
        Grid.SetRow(contentsButton, 1);
        grid.Children.Add(contentsButton);

        Content = grid;
        BackgroundColor = new Color(0,139,139);
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		var result = await authService.AcquireTokenSilent(CancellationToken.None);
		await GetResult(result);
	}

	private async Task GetResult(AuthenticationResult? result)
	{
		var token = result?.IdToken;
		if (token != null)
		{
			var handler = new JwtSecurityTokenHandler();
			var data = handler.ReadJwtToken(token);
			if (data != null)
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"Name: {data.Claims.FirstOrDefault(x => x.Type.Equals("name"))?.Value}");
				await Toast.Make(stringBuilder.ToString()).Show();

                foreach (var claim in data.Claims)
                {
                    Debug.WriteLine($"{claim.Type}: {claim.Value}");
                }
                var userIdClaim = data.Claims.FirstOrDefault(x => x.Type.Equals("oid"));
                var userId = userIdClaim?.Value;

                Global.UserId = userId;
                Debug.WriteLine($"Global.UserId: {userId}");
                
                loginButton.IsVisible = false;
				logoutButton.IsVisible = true;
                contentsButton.IsVisible = true;
            }
		}
	}

	private async Task OnLoginClicked()
	{
		try
		{
			var result = await authService.SignInInteractively(CancellationToken.None);
			await GetResult(result);
		}
        catch (MsalClientException ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

	private async Task OnLogoutClicked()
	{
		await authService.LogoutAsync(CancellationToken.None);
        Global.UserId = null;
        loginButton.IsVisible = true;
		logoutButton.IsVisible = false;
        contentsButton.IsVisible = false;
    }

    
    private async Task OnContentsClicked()
    {
        try
        {
            var contentsPage = new ContentsPage();
            await Navigation.PushAsync(contentsPage);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in OnContentsClicked: {ex}");
        }
    }

    
}