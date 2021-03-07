using abremir.AllMyBricks.App.Configuration;
using abremir.AllMyBricks.App.Services;
using abremir.AllMyBricks.Platform.Interfaces;
using LightInject;
using Xamarin.Forms;

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
            var allMyBricksOnboardingUrl = Settings.AllMyBricksOnboardingUrl;

            IoC.Configure(allMyBricksOnboardingUrl);

            ClearThumbnailCacheIfRequired();
        }

        protected override void OnSleep()
        {
            ClearThumbnailCacheIfRequired();
        }

        protected override void OnResume()
        {
        }

        private void ClearThumbnailCacheIfRequired()
        {
            var preferencesService = IoC.IoCContainer.GetInstance<IPreferencesService>();

            if (preferencesService.ClearThumbnailCache)
            {
                var fileSystemService = IoC.IoCContainer.GetInstance<IFileSystemService>();

                if (fileSystemService.ClearThumbnailCache())
                {
                    preferencesService.ClearThumbnailCache = false;
                }
            }
        }
    }
}
