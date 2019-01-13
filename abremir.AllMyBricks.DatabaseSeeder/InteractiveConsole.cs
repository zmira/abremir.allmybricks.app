using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Loggers;
using abremir.AllMyBricks.DataSynchronizer.Events.DataSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Device.Interfaces;
using System;
using System.Diagnostics;
using Terminal.Gui;

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

            var themeLabel = new Label(4, 3, "                                                              ");
            var themeProgress = new ProgressBar(new Rect(4, 4, 88, 1));

            var subthemeLabel = new Label(6, 6, "                                                              ");
            var subthemeProgress = new ProgressBar(new Rect(6, 7, 84, 1));

            var setLabel = new Label(8, 9, "                                                              ");
            var setProgress = new ProgressBar(new Rect(8, 10, 80, 1));

            var synchronizeAdditionalImagesLabel = new Label(4, 16, "Synchronize Additional Images: False");
            var synchronizeInstructionsLabel = new Label(4, 17, "Synchronize Instructions: False");
            var synchronizeReviewsLabel = new Label(4, 18, "Synchronize Reviews: False");
            var synchronizeExtendedDataLabel = new Label(4, 19, "Synchronize Extended Data: False");
            var synchronizeTagsLabel = new Label(6, 20, "Synchronize Tags: False");
            var synchronizePricesLabel = new Label(6, 21, "Synchronize Prices: False");

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
            SynchronizationProgressFrame.Add(synchronizeAdditionalImagesLabel);
            SynchronizationProgressFrame.Add(synchronizeInstructionsLabel);
            SynchronizationProgressFrame.Add(synchronizeReviewsLabel);
            SynchronizationProgressFrame.Add(synchronizeExtendedDataLabel);
            SynchronizationProgressFrame.Add(synchronizeTagsLabel);
            SynchronizationProgressFrame.Add(synchronizePricesLabel);
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

            var dataSynchronizationServiceEventHandler = IoC.IoCContainer.GetInstance<DataSynchronizationServiceLogger>();
            var themeSynchronizerEventHandler = IoC.IoCContainer.GetInstance<ThemeSynchronizerLogger>();
            var subthemeSynchronizerEventHandler = IoC.IoCContainer.GetInstance<SubthemeSynchronizerLogger>();
            var setSynchronizerEventHandler = IoC.IoCContainer.GetInstance<SetSynchronizerLogger>();
            var thumbnailSynchronizerEventHandler = IoC.IoCContainer.GetInstance<ThumbnailSynchronizerLogger>();

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
                synchronizeAdditionalImagesLabel.Text = $"Synchronize Additional Images: {(Settings.SynchronizeAdditionalImages ? "True" : "False")}";
                synchronizeInstructionsLabel.Text = $"Synchronize Instructions: {(Settings.SynchronizeInstructions ? "True" : "False")}";
                synchronizeReviewsLabel.Text = $"Synchronize Reviews: {(Settings.SynchronizeReviews ? "True" : "False")}";
                synchronizeExtendedDataLabel.Text = $"Synchronize Extended Data: {(Settings.SynchronizeSetExtendedData ? "True" : "False")}";
                synchronizeTagsLabel.Text = $"Synchronize Tags: {(Settings.SynchronizeTags ? "True" : "False")}";
                synchronizePricesLabel.Text = $"Synchronize Prices: {(Settings.SynchronizePrices ? "True" : "False")}";

                Application.MainLoop.Invoke(lastUpdatedLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(synchronizeAdditionalImagesLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(synchronizeInstructionsLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(synchronizeReviewsLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(synchronizeExtendedDataLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(synchronizeTagsLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(synchronizePricesLabel.ChildNeedsDisplay);

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<ThemeSynchronizerStart>(_ =>
            {
                themeIndex = 0;
                themeProgress.Fraction = 0;

                Application.MainLoop.Invoke(themeProgress.ChildNeedsDisplay);

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
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

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SynchronizedTheme>(_ =>
            {
                themeLabel.Text = string.Empty;

                totalUpdatedThemesLabel.Text = $"Total Updated Themes: {totalUpdatedThemes}";

                Application.MainLoop.Invoke(themeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(totalUpdatedThemesLabel.ChildNeedsDisplay);

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<ThemeSynchronizerEnd>(_ =>
            {
                themeLabel.Text = string.Empty;
                themeIndex = 0f;
                themeProgress.Fraction = 0;

                Application.MainLoop.Invoke(themeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(themeProgress.ChildNeedsDisplay);

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<ProcessingTheme>(ev =>
            {
                themeLabel.Text = $"Theme: {ev.Name}";

                Application.MainLoop.Invoke(themeLabel.ChildNeedsDisplay);

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SubthemeSynchronizerStart>(_ =>
            {
                subthemeLabel.Text = string.Empty;
                subthemeIndex = 0;
                subthemeProgress.Fraction = 0;

                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(subthemeProgress.ChildNeedsDisplay);

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
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

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SynchronizedSubtheme>(_ =>
            {
                subthemeLabel.Text = string.Empty;

                totalUpdatedSubthemesLabel.Text = $"Total Updated Subthemes: {totalUpdatedSubthemes}";

                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(totalUpdatedSubthemesLabel.ChildNeedsDisplay);

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SubthemeSynchronizerEnd>(_ =>
            {
                subthemeLabel.Text = string.Empty;
                subthemeIndex = 0f;
                subthemeProgress.Fraction = 0;

                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(subthemeProgress.ChildNeedsDisplay);

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<ProcessingSubtheme>(ev =>
            {
                subthemeLabel.Text = $"Subtheme: {ev.Name}";

                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SetSynchronizerStart>(_ =>
            {
                setLabel.Text = string.Empty;
                setIndex = 0;
                setProgress.Fraction = 0;

                Application.MainLoop.Invoke(setLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(setProgress.ChildNeedsDisplay);

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<AcquiringSets>(ev =>
            {
                setIndex = 0;
                setProgress.Fraction = 0;
                subthemeLabel.Text = $"Subtheme: {ev.Subtheme}, {ev.Year}";

                Application.MainLoop.Invoke(setProgress.ChildNeedsDisplay);
                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
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

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
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

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<SynchronizedSet>(_ =>
            {
                setLabel.Text = string.Empty;

                totalUpdatedSetsLabel.Text = $"Total Updated Sets: {totalUpdatedSets}";

                Application.MainLoop.Invoke(setLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(totalUpdatedSetsLabel.ChildNeedsDisplay);

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
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

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
            });
            dataSynchronizerEventManager.Register<ProcessedSubtheme>(_ =>
            {
                subthemeLabel.Text = string.Empty;
                subthemeIndex++;
                subthemeProgress.Fraction = subthemeIndex / subthemeCount;

                Application.MainLoop.Invoke(subthemeLabel.ChildNeedsDisplay);
                Application.MainLoop.Invoke(subthemeProgress.ChildNeedsDisplay);

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
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

                //Application.MainLoop.Invoke(SynchronizationProgressFrame.ChildNeedsDisplay);
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

            AddButton(topLevelWindow);

            AddOnboardingUrl(topLevelWindow);

            AddSynchronizationOptions(topLevelWindow);

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

        private static void AddButton(Window window)
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

                    var dataSynchronizationService = IoC.IoCContainer.GetInstance<IDataSynchronizationService>();

                    dataSynchronizationService.SynchronizeAllSetData();
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

        private static void AddSynchronizationOptions(Window window)
        {
            var synchronizeAdditionalImagesCheckbox = new CheckBox(3, 4, "Synchronize additional images", Settings.SynchronizeAdditionalImages);
            synchronizeAdditionalImagesCheckbox.Toggled += (sender, _) => Settings.SynchronizeAdditionalImages = ((CheckBox)sender).Checked;

            var synchronizeInstructionsCheckbox = new CheckBox(3, 5, "Synchronize instructions", Settings.SynchronizeInstructions);
            synchronizeInstructionsCheckbox.Toggled += (sender, _) => Settings.SynchronizeInstructions = ((CheckBox)sender).Checked;

            var synchronizeReviewsCheckbox = new CheckBox(3, 6, "Synchronize reviews", Settings.SynchronizeReviews);
            synchronizeReviewsCheckbox.Toggled += (sender, _) => Settings.SynchronizeReviews = ((CheckBox)sender).Checked;

            var synchronizeExtendedDataCheckbox = new CheckBox(3, 7, "Synchronize sets' extended data", Settings.SynchronizeSetExtendedData);

            var synchronizeTagsCheckbox = new CheckBox(5, 8, "Synchronize tags", Settings.SynchronizeTags);
            synchronizeTagsCheckbox.Toggled += (sender, _) => Settings.SynchronizeTags = ((CheckBox)sender).Checked;

            var synchronizePricesCheckbox = new CheckBox(5, 9, "Synchronize prices", Settings.SynchronizePrices);
            synchronizePricesCheckbox.Toggled += (sender, _) => Settings.SynchronizePrices = ((CheckBox)sender).Checked;

            synchronizeExtendedDataCheckbox.Toggled += (sender, _) =>
            {
                Settings.SynchronizeSetExtendedData = ((CheckBox)sender).Checked;

                SetExtendedDataOptions(window, ((CheckBox)sender).Checked, synchronizeTagsCheckbox, synchronizePricesCheckbox);
            };

            window.Add(synchronizeAdditionalImagesCheckbox);
            window.Add(synchronizeInstructionsCheckbox);
            window.Add(synchronizeReviewsCheckbox);
            window.Add(synchronizeExtendedDataCheckbox);

            SetExtendedDataOptions(window, Settings.SynchronizeSetExtendedData, synchronizeTagsCheckbox, synchronizePricesCheckbox);
        }

        private static void SetExtendedDataOptions(Window window, bool parentCheckedState, params View[] extendedDataOptions)
        {
            if (!parentCheckedState)
            {
                foreach (var view in extendedDataOptions)
                {
                    ((CheckBox)view).Checked = false;

                    window.Remove(view);
                }

                return;
            }

            window.Add(extendedDataOptions);
        }
    }
}
