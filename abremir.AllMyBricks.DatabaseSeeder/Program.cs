using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Loggers;
using abremir.AllMyBricks.DataSynchronizer.Events.DataSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Device.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Terminal.Gui;

namespace abremir.AllMyBricks.DatabaseSeeder
{
    class Program
    {
        private static ILogger Logger;
        private static FrameView SynchronizationProgressFrame;
        private static bool CanExit = true;

        static void Main(string[] args)
        {
            Logging.Configure();

            Logger = Logging.CreateLogger<Program>();

            Logger.LogInformation("Starting database seeder...");

            Application.Init();

            var topLevel = Application.Top;

            var topLevelWindow = AddTopLevelWindow(topLevel);

            SynchronizationProgressFrame = new FrameView(new Rect(10, 1, 99, topLevel.Frame.Height - 4), "Synchronization Progress...");

            var themeLabel = new Label(4, 3, "                                                    ");
            var themeProgress = new ProgressBar(new Rect(4, 4, 88, 1));

            var subthemeLabel = new Label(6, 6, "                                                    ");
            var subthemeProgress = new ProgressBar(new Rect(6, 7, 84, 1));

            var setLabel = new Label(8, 9, "                                                    ");
            var setProgress = new ProgressBar(new Rect(8, 10, 80, 1));

            var lastUpdatedLabel = new Label(4, 15, "Last Updated: Never                                                   ");
            var totalUpdatedThemesLabel = new Label(4, 16, "Total Updated Themes: 0                                                   ");
            var totalUpdatedSubthemesLabel = new Label(4, 17, "Total Updated Subthemes: 0                                                   ");
            var totalUpdatedSetsLabel = new Label(4, 18, "Total Updated Sets: 0                                                   ");

            SynchronizationProgressFrame.Add(themeLabel);
            SynchronizationProgressFrame.Add(themeProgress);
            SynchronizationProgressFrame.Add(subthemeLabel);
            SynchronizationProgressFrame.Add(subthemeProgress);
            SynchronizationProgressFrame.Add(setLabel);
            SynchronizationProgressFrame.Add(setProgress);
            SynchronizationProgressFrame.Add(lastUpdatedLabel);
            SynchronizationProgressFrame.Add(totalUpdatedThemesLabel);
            SynchronizationProgressFrame.Add(totalUpdatedSubthemesLabel);
            SynchronizationProgressFrame.Add(totalUpdatedSetsLabel);

            IoC.Configure();
            IoC.ConfigureOnboarding(Settings.OnboardingUrl);

            var themeCount = 0f;
            var themeIndex = 0f;
            var subthemeCount = 0f;
            var subthemeIndex = 0f;
            var setCount = 0f;
            var setIndex = 0f;
            var totalUpdatedThemes = 0f;
            var totalUpdatedSubthemes = 0f;
            var totalUpdatedSets = 0f;

            var dataSynchronizationServiceEventHandler = IoC.IoCContainer.GetInstance<DataSynchronizationServiceLogger>();
            var themeSynchronizerEventHandler = IoC.IoCContainer.GetInstance<ThemeSynchronizerLogger>();
            var subthemeSynchronizerEventHandler = IoC.IoCContainer.GetInstance<SubthemeSynchronizerLogger>();
            var setSynchronizerEventHandler = IoC.IoCContainer.GetInstance<SetSynchronizerLogger>();
            var thumbnailSynchronizerEventHandler = IoC.IoCContainer.GetInstance<ThumbnailSynchronizerLogger>();

            var dataSynchronizerEventHandler = IoC.IoCContainer.GetInstance<IDataSynchronizerEventManager>();

            dataSynchronizerEventHandler.Register<DataSynchronizationStart>(_ => SynchronizationProgressFrame.Clear());
            dataSynchronizerEventHandler.Register<DataSynchronizationEnd>(_ =>
            {
                themeLabel.Text = string.Empty;
                subthemeLabel.Text = string.Empty;
                setLabel.Text = string.Empty;

                themeCount = 0f;
                subthemeCount = 0f;
                setCount = 0f;

                themeIndex = 0f;
                subthemeIndex = 0f;
                setIndex = 0f;

                themeProgress.Fraction = 0;
                subthemeProgress.Fraction = 0;
                setProgress.Fraction = 0;

                SynchronizationProgressFrame.Clear();

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<InsightsAcquired>(ev =>
            {
                lastUpdatedLabel.Text = $"Last Updated: {(ev.SynchronizationTimestamp.HasValue ? ev.SynchronizationTimestamp.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Never")}";

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<ThemeSynchronizerStart>(_ =>
            {
                themeIndex = 0;
                themeProgress.Fraction = 0;

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<ThemesAcquired>(ev =>
            {
                themeCount = ev.Count;

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<SynchronizingTheme>(ev =>
            {
                themeLabel.Text = $"Theme: {ev.Theme}";
                themeIndex++;
                totalUpdatedThemes++;
                themeProgress.Fraction = themeIndex / themeCount;

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<SynchronizedTheme>(_ =>
            {
                themeLabel.Text = string.Empty;

                totalUpdatedThemesLabel.Text = $"Total Updated Themes: {totalUpdatedThemes}";

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<ThemeSynchronizerEnd>(_ =>
            {
                themeLabel.Text = string.Empty;
                themeIndex = 0f;
                themeProgress.Fraction = 0;

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<ProcessingTheme>(ev =>
            {
                themeLabel.Text = $"Theme: {ev.Name}";

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<SubthemeSynchronizerStart>(_ =>
            {
                subthemeLabel.Text = string.Empty;
                subthemeIndex = 0;
                subthemeProgress.Fraction = 0;

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<SubthemesAcquired>(ev =>
            {
                subthemeCount = ev.Count;
                subthemeIndex = 0;

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<SynchronizingSubtheme>(ev =>
            {
                subthemeLabel.Text = $"Subtheme: {ev.Subtheme}";
                subthemeIndex++;
                totalUpdatedSubthemes++;
                subthemeProgress.Fraction = subthemeIndex / subthemeCount;

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<SynchronizedSubtheme>(ev =>
            {
                subthemeLabel.Text = string.Empty;

                totalUpdatedSubthemesLabel.Text = $"Total Updated Subthemes: {totalUpdatedSubthemes}";

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<SubthemeSynchronizerEnd>(_ =>
            {
                subthemeLabel.Text = string.Empty;
                subthemeIndex = 0f;
                subthemeProgress.Fraction = 0;

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<ProcessingSubtheme>(ev =>
            {
                subthemeLabel.Text = $"Subtheme: {ev.Name}";

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<SetSynchronizerStart>(ev =>
            {
                setLabel.Text = string.Empty;
                setIndex = 0;
                setProgress.Fraction = 0;

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<AcquiringSets>(ev =>
            {
                setIndex = 0;
                setProgress.Fraction = 0;
                subthemeLabel.Text = $"Subtheme: {ev.Subtheme}, {ev.Year}";

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<SetsAcquired>(ev =>
            {
                setCount = ev.Count;
                setIndex = 0;
                setProgress.Fraction = 0;
                if (ev.Year.HasValue)
                {
                    subthemeLabel.Text = $"Subtheme: {ev.Subtheme}, {ev.Year.Value}";
                }

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<SynchronizingSet>(ev =>
            {
                themeLabel.Text = $"Theme: {ev.Theme}";
                subthemeLabel.Text = $"Subtheme: {ev.Subtheme}{(ev.Year.HasValue ? $", {ev.Year.Value}" : string.Empty)}";
                setLabel.Text = $"Set: {ev.IdentifierShort}";
                setIndex++;
                totalUpdatedSets++;
                setProgress.Fraction = setIndex / setCount;

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<SynchronizedSet>(_ =>
            {
                setLabel.Text = string.Empty;

                totalUpdatedSetsLabel.Text = $"Total Updated Sets: {totalUpdatedSets}";

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<SetSynchronizerEnd>(ev =>
            {
                if (!ev.ForSubtheme)
                {
                    themeLabel.Text = string.Empty;
                    subthemeLabel.Text = string.Empty;
                }
                setLabel.Text = string.Empty;
                setIndex = 0;
                setProgress.Fraction = 0;

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<ProcessedSubtheme>(_ =>
            {
                subthemeLabel.Text = string.Empty;
                subthemeIndex++;
                subthemeProgress.Fraction = subthemeIndex / subthemeCount;

                Application.Refresh();
            });
            dataSynchronizerEventHandler.Register<ProcessedTheme>(_ =>
            {
                themeLabel.Text = string.Empty;
                themeIndex++;
                themeProgress.Fraction = themeIndex / themeCount;

                if((int)themeIndex == (int)themeCount)
                {
                    themeProgress.Fraction = 0f;
                    subthemeProgress.Fraction = 0f;
                }

                Application.Refresh();
            });

            var fileSystem = IoC.IoCContainer.GetInstance<IFileSystemService>();
            fileSystem.EnsureLocalDataFolder();

            AddMenuBar(topLevel);

            AddButton(topLevel, topLevelWindow);

            AddOnboardingUrl(topLevelWindow);

            Application.Run();
        }

        private static Window AddTopLevelWindow(Toplevel topLevel)
        {
            var window = new Window("All My Bricks Database Seeder")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            topLevel.Add(
                window
            );

            return window;
        }

        private static void AddMenuBar(Toplevel topLevel)
        {
            topLevel.Add(
                new MenuBar(new MenuBarItem[]
                {
                    new MenuBarItem("_File", new MenuItem[]
                    {
                        new MenuItem("E_xit", "", () => topLevel.Running &= !CanExit)
                    })
                })
            );
        }

        private static void AddButton(Toplevel topLevel, Window window)
        {
            var button = new Button("Start seeding the database...")
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Clicked = () =>
                {
                    CanExit = false;

                    window.Add(SynchronizationProgressFrame);

                    var dataSynchronizationService = IoC.IoCContainer.GetInstance<IDataSynchronizationService>();

                    Task.Run(() =>
                    {
                        dataSynchronizationService.SynchronizeAllSetData();

                        var dialog = new Dialog("Synchronization finished", 50, 6, new Button("Ok")
                        {
                            Clicked = () =>
                            {
                                window.Remove(SynchronizationProgressFrame);

                                CanExit = true;

                                Application.RequestStop();
                            }
                        });

                        Application.Run(dialog);
                    });
                }
            };

            window.Add(
                button
            );
        }

        private static void AddOnboardingUrl(Window window)
        {
            var onboardingUrlLabel = new Label(3, 2, "Onboarding URL:");
            var onboardingUrlValue = new Label(Settings.OnboardingUrl)
            {
                X = Pos.Right(onboardingUrlLabel) + 1,
                Y = onboardingUrlLabel.Frame.Y
            };

            window.Add(
                onboardingUrlLabel,
                onboardingUrlValue,
                new Button("Edit")
                {
                    X = Pos.Right(onboardingUrlValue) + 5,
                    Y = onboardingUrlLabel.Frame.Y,
                    Clicked = () =>
                    {
                        var onboardingUrlTextField = new TextField(Settings.OnboardingUrl);

                        var dialog = new Dialog(
                            "Onboarding URL",
                            50,
                            7,
                            new Button("Ok", false)
                            {
                                Clicked = () =>
                                {
                                    var onboardingUrl = onboardingUrlTextField.Text.ToString();

                                    Settings.OnboardingUrl = onboardingUrl;
                                    onboardingUrlValue.Text = onboardingUrl;

                                    IoC.ReplaceOnboarding(onboardingUrl);

                                    Application.RequestStop();
                                }
                            },
                            new Button("Cancel", false)
                            {
                                Clicked = Application.RequestStop
                            })
                        {
                            onboardingUrlTextField
                        };

                        Application.Run(dialog);
                    }
                }
            );
        }
    }
}