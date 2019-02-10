using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Services;
using abremir.AllMyBricks.DataSynchronizer.Events.DataSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Device.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using Terminal.Gui;

using DataConfigurationConstants = abremir.AllMyBricks.Data.Configuration.Constants;

namespace abremir.AllMyBricks.DatabaseSeeder
{
    public static class InteractiveConsole
    {
        private static FrameView SynchronizationProgressFrame;
        private static bool CanExit = true;

        public static void Run()
        {
            Application.Init();

            var topLevel = Application.Top;

            var topLevelWindow = AddTopLevelWindow(topLevel);

            SynchronizationProgressFrame = new FrameView(new Rect(10, 1, 99, topLevel.Frame.Height - 4), "Synchronization Progress...");

            var themeLabel = new Label(4, 3, "".PadRight(70));
            var themeProgress = new ProgressBar(new Rect(4, 4, 88, 1));

            var subthemeLabel = new Label(6, 6, "".PadRight(70));
            var subthemeProgress = new ProgressBar(new Rect(6, 7, 84, 1));

            var setLabel = new Label(8, 9, "".PadRight(70));
            var setProgress = new ProgressBar(new Rect(8, 10, 80, 1));

            var lastUpdatedLabel = new Label(50, 17, "Last Updated: Never                 ");
            var totalUpdatedThemesLabel = new Label(50, 18, "Total Updated Themes: 0      ");
            var totalUpdatedSubthemesLabel = new Label(50, 19, "Total Updated Subthemes: 0      ");
            var totalUpdatedSetsLabel = new Label(50, 20, "Total Updated Sets: 0      ");

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

            var themeCount = 0f;
            var themeIndex = 0f;
            var subthemeCount = 0f;
            var subthemeIndex = 0f;
            var setCount = 0f;
            var setIndex = 0f;
            var totalUpdatedThemes = 0f;
            var totalUpdatedSubthemes = 0f;
            var totalUpdatedSets = 0f;

            var dataSynchronizerEventManager = IoC.IoCContainer.GetInstance<IDataSynchronizerEventManager>();

            Stopwatch stopwatch = null;

            dataSynchronizerEventManager.Register<DataSynchronizationStart>(_ =>
            {
                totalUpdatedThemes = 0f;
                totalUpdatedSubthemes = 0f;
                totalUpdatedSets = 0;

                SynchronizationProgressFrame.Clear();

                Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);

                stopwatch = Stopwatch.StartNew();
            });
            dataSynchronizerEventManager.Register<InsightsAcquired>(ev =>
            {
                lastUpdatedLabel.Text = $"Last Updated: {(ev.SynchronizationTimestamp.HasValue ? ev.SynchronizationTimestamp.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Never")}";

                Application.MainLoop.Invoke(lastUpdatedLabel.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<ThemeSynchronizerStart>(_ =>
            {
                themeIndex = 0;
                themeProgress.Fraction = 0;

                Application.MainLoop.Invoke(themeProgress.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<ThemesAcquired>(ev => themeCount = ev.Count);
            dataSynchronizerEventManager.Register<SynchronizingTheme>(ev =>
            {
                themeLabel.Text = $"Theme: {ev.Theme}";
                themeIndex++;
                totalUpdatedThemes++;
                themeProgress.Fraction = themeIndex / themeCount;

                Application.MainLoop.Invoke(themeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(themeProgress.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SynchronizedTheme>(_ =>
            {
                themeLabel.Text = string.Empty;

                totalUpdatedThemesLabel.Text = $"Total Updated Themes: {totalUpdatedThemes}";

                Application.MainLoop.Invoke(themeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(totalUpdatedThemesLabel.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<ThemeSynchronizerEnd>(_ =>
            {
                themeLabel.Text = string.Empty;
                themeIndex = 0f;
                themeProgress.Fraction = 0;

                Application.MainLoop.Invoke(themeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(themeProgress.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<ProcessingTheme>(ev =>
            {
                themeLabel.Text = $"Theme: {ev.Name}";

                Application.MainLoop.Invoke(themeLabel.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SubthemeSynchronizerStart>(_ =>
            {
                subthemeLabel.Text = string.Empty;
                subthemeIndex = 0;
                subthemeProgress.Fraction = 0;

                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(subthemeProgress.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SubthemesAcquired>(ev =>
            {
                subthemeCount = ev.Count;
                subthemeIndex = 0;
            });
            dataSynchronizerEventManager.Register<SynchronizingSubtheme>(ev =>
            {
                subthemeLabel.Text = $"Subtheme: {ev.Subtheme}";
                subthemeIndex++;
                totalUpdatedSubthemes++;
                subthemeProgress.Fraction = subthemeIndex / subthemeCount;

                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(subthemeProgress.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SynchronizedSubtheme>(_ =>
            {
                subthemeLabel.Text = string.Empty;

                totalUpdatedSubthemesLabel.Text = $"Total Updated Subthemes: {totalUpdatedSubthemes}";

                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(totalUpdatedSubthemesLabel.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SubthemeSynchronizerEnd>(_ =>
            {
                subthemeLabel.Text = string.Empty;
                subthemeIndex = 0f;
                subthemeProgress.Fraction = 0;

                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(subthemeProgress.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<ProcessingSubtheme>(ev =>
            {
                subthemeLabel.Text = $"Subtheme: {ev.Name}";

                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SetSynchronizerStart>(_ =>
            {
                setLabel.Text = string.Empty;
                setIndex = 0;
                setProgress.Fraction = 0;

                Application.MainLoop.Invoke(setLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(setProgress.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<AcquiringSets>(ev =>
            {
                setIndex = 0;
                setProgress.Fraction = 0;
                subthemeLabel.Text = $"Subtheme: {ev.Subtheme}, {ev.Year}";

                Application.MainLoop.Invoke(setProgress.ChildNeedsDisplay);
                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SetsAcquired>(ev =>
            {
                setCount = ev.Count;
                setIndex = 0;
                setProgress.Fraction = 0;
                if (ev.Year.HasValue)
                {
                    subthemeLabel.Text = $"Subtheme: {ev.Subtheme}, {ev.Year.Value}";

                    Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
                }

                Application.MainLoop.Invoke(setProgress.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SynchronizingSet>(ev =>
            {
                themeLabel.Text = $"Theme: {ev.Theme}";
                subthemeLabel.Text = $"Subtheme: {ev.Subtheme}{(ev.Year.HasValue ? $", {ev.Year.Value}" : string.Empty)}";
                setLabel.Text = $"Set: {ev.IdentifierShort}";
                setIndex++;
                totalUpdatedSets++;
                setProgress.Fraction = setIndex / setCount;

                Application.MainLoop.Invoke(themeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(setLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(setProgress.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SynchronizedSet>(_ =>
            {
                setLabel.Text = string.Empty;

                totalUpdatedSetsLabel.Text = $"Total Updated Sets: {totalUpdatedSets}";

                Application.MainLoop.Invoke(setLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(totalUpdatedSetsLabel.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SetSynchronizerEnd>(ev =>
            {
                if (!ev.ForSubtheme)
                {
                    themeLabel.Text = string.Empty;
                    subthemeLabel.Text = string.Empty;

                    Application.MainLoop.Invoke(themeLabel.ChildNeedsDisplay);
                    Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
                }
                setLabel.Text = string.Empty;
                setIndex = 0;
                setProgress.Fraction = 0;

                Application.MainLoop.Invoke(setLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(setProgress.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<ProcessedSubtheme>(_ =>
            {
                subthemeLabel.Text = string.Empty;
                subthemeIndex++;
                subthemeProgress.Fraction = subthemeIndex / subthemeCount;

                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(subthemeProgress.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<ProcessedTheme>(_ =>
            {
                themeLabel.Text = string.Empty;
                themeIndex++;
                themeProgress.Fraction = themeIndex / themeCount;

                if ((int)themeIndex == (int)themeCount)
                {
                    themeProgress.Fraction = 0f;
                    subthemeProgress.Fraction = 0f;

                    Application.MainLoop.Invoke(subthemeProgress.ChildNeedsDisplay);
                }

                Application.MainLoop.Invoke(themeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(themeProgress.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<DataSynchronizationEnd>(_ =>
            {
                stopwatch.Stop();

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

                Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);

                var dialog = new Dialog("Synchronization finished", 50, 8, new Button("Ok")
                {
                    Clicked = () =>
                    {
                        topLevelWindow.Remove(SynchronizationProgressFrame);

                        CanExit = true;

                        Application.RequestStop();
                    }
                });

                var totalTimeLabel = new Label($"Synchronized in {stopwatch.Elapsed.ToString(@"hh\:mm\:ss")}")
                {
                    X = Pos.Center(),
                    Y = 1
                };
                dialog.Add(totalTimeLabel);

                Application.Run(dialog);
            });

            AddMenuBar(topLevel);

            AddDatabaseSeedingButton(topLevelWindow);

            AddOnboardingUrl(topLevelWindow);

            AddCompressDatabaseFileButton(topLevelWindow);

            AddUncompressDatabaseFileButton(topLevelWindow);

            AddCompactDatabaseButton(topLevelWindow);

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

        private static void AddDatabaseSeedingButton(Window window)
        {
            var button = new Button("Start seeding the database...")
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Clicked = () =>
                {
                    CanExit = false;

                    window.Add(SynchronizationProgressFrame);

                    // HACK: since there is a bug in Application.MainLoop.Invoke(...) this is needed to force the UI to refresh!
                    Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(500), _ => true);

                    IoC.IoCContainer.GetInstance<IDataSynchronizationService>().SynchronizeAllSetData();
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

                                    AddCompressDatabaseFileButton(window);

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

        private static void AddCompressDatabaseFileButton(Window window)
        {
            var assetManagementService = IoC.IoCContainer.GetInstance<IAssetManagementService>();

            if (assetManagementService.DatabaseFilePathExists())
            {
                var button = new Button("Compress Database File")
                {
                    X = 3,
                    Y = 4,
                    Clicked = () =>
                    {
                        CanExit = false;

                        assetManagementService.CompressDatabaseFile();

                        var dialog = new Dialog("Database File Compressed", 50, 6, new Button("Ok")
                        {
                            Clicked = () =>
                            {
                                CanExit = true;

                                Application.RequestStop();
                            }
                        });

                        Application.Run(dialog);
                    }
                };

                window.Add(
                    button
                );
            }
        }

        private static void AddUncompressDatabaseFileButton(Window window)
        {
            var assetManagementService = IoC.IoCContainer.GetInstance<IAssetManagementService>();

            if (assetManagementService.CompressedDatabaseFilePathExists())
            {
                var button = new Button("Uncompress Database File")
                {
                    X = 3,
                    Y = 6,
                    Clicked = () =>
                    {
                        CanExit = false;

                        assetManagementService.UncompressDatabaseFile();

                        var dialog = new Dialog("Database File Uncompressed", 50, 6, new Button("Ok")
                        {
                            Clicked = () =>
                            {
                                CanExit = true;

                                Application.RequestStop();
                            }
                        });

                        Application.Run(dialog);
                    }
                };

                window.Add(
                    button
                );
            }
        }

        private static void AddCompactDatabaseButton(Window window)
        {
            var assetManagementService = IoC.IoCContainer.GetInstance<IAssetManagementService>();

            if (assetManagementService.DatabaseFilePathExists())
            {
                var button = new Button("Compact AllMyBricks Database")
                {
                    X = 3,
                    Y = 8,
                    Clicked = () =>
                    {
                        CanExit = false;

                        assetManagementService.CompactAllMyBricksDatabase();

                        var dialog = new Dialog("AllMyBricks Database Compacted", 50, 6, new Button("Ok")
                        {
                            Clicked = () =>
                            {
                                CanExit = true;

                                Application.RequestStop();
                            }
                        });

                        Application.Run(dialog);
                    }
                };

                window.Add(
                    button
                );
            }
        }
    }
}
