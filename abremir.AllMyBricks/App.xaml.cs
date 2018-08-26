using abremir.AllMyBricks.Configuration;
using abremir.AllMyBricks.Device.Interfaces;
using Xamarin.Forms;

namespace abremir.AllMyBricks
{
    public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}

		protected override void OnStart ()
		{
            // Handle when your app starts
            IoC.Configure();

            ClearThumbnailCacheIfRequired();
        }

        protected override void OnSleep ()
		{
            // Handle when your app sleeps
            ClearThumbnailCacheIfRequired();
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
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