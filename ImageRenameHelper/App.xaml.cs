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
    }
}