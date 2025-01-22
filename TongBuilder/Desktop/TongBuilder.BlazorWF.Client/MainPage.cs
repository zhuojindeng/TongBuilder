using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebView;
using TongBuilder.RazorLib;

namespace TongBuilder.BlazorWF.Client
{
    public partial class MainPage : Form
    {
        public MainPage()
        {
            InitializeComponent();
            //_content/{PACKAGE ID/ASSEMBLY NAME}/{PATH}/{FILE NAME}
            blazorWebView1.HostPage = "wwwroot\\index.html";
            blazorWebView1.Services = Program.ServiceCollection.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<Routes>("#app");

            blazorWebView1.UrlLoading += (sender, urlLoadingEventArgs) =>
            {
                if (urlLoadingEventArgs.Url.Host != "0.0.0.0")
                {
                    urlLoadingEventArgs.UrlLoadingStrategy = UrlLoadingStrategy.OpenInWebView;
                }
            };
            //blazorWebView1.StartPath = "/welcome";
            //blazorWebView2.HostPage = "wwwroot\\index.html";
            //blazorWebView2.Services = Program.ServiceCollection.BuildServiceProvider();
            //blazorWebView2.RootComponents.Add<Routes>("#app");
        }
    }
}
