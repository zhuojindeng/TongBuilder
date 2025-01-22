using Microsoft.AspNetCore.Components.WebView;
using TongBuilder.BlazorMaui.App.ViewModels;

namespace TongBuilder.BlazorMaui.App
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            blazorWebView.UrlLoading +=
                (sender, urlLoadingEventArgs) =>
                {
                    if (urlLoadingEventArgs.Url.Host != "0.0.0.0")
                    {
                        urlLoadingEventArgs.UrlLoadingStrategy =
                            UrlLoadingStrategy.OpenInWebView;
                    }
                };
            //blazorWebView.StartPath = "/welcome";
            rootComponent.Parameters =
                new Dictionary<string, object>
                {
                    { "KeypadViewModel", new KeypadViewModel() }
                };
        }
    }
}
