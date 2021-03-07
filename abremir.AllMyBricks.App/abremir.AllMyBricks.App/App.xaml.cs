using abremir.AllMyBricks.App.Services;
using abremir.AllMyBricks.App.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace abremir.AllMyBricks.App
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
