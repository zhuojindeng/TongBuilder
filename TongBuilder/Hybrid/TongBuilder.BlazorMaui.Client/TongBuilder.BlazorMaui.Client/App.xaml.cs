﻿namespace TongBuilder.BlazorMaui.Client
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "TongBuilder.BlazorMaui.Client" };
        }
    }
}
