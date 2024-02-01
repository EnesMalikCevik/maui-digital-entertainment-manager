namespace MauiApp4;
using Application = Microsoft.Maui.Controls.Application;

public partial class App : Application
{
    public App(AzureB2CPage mainPage)
	{
		InitializeComponent();

        //MainPage = new AppShell();
        MainPage = new NavigationPage(mainPage);
    }
}
    