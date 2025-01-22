namespace TongBuilder.BlazorMaui.App
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "TongBuilder.BlazorMaui.App" };
        }
    }
}
