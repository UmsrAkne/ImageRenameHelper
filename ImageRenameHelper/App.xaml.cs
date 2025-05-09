using System;
using System.IO;
using System.Windows;
using ImageRenameHelper.ViewModels;
using ImageRenameHelper.Views;
using Prism.Ioc;

namespace ImageRenameHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<WorkingDirectoryChangePage, WorkingDirectoryChangePageViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // For UI thread
            DispatcherUnhandledException += (_, args) =>
            {
                LogFatal(args.Exception); // Log the exception when the application crashes.

                // Let the application crash. Set explicitly for clarity, although 'false' is the default.
                args.Handled = false;
            };

            // For Background thread
            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                {
                    LogFatal(ex);
                }
            };

            base.OnStartup(e);
        }

        private void LogFatal(Exception ex)
        {
            File.AppendAllText("crash.log", $"[{DateTime.Now}] {ex}\n\n");
        }
    }
}