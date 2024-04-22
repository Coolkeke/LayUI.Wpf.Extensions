using LayUI.Wpf.Extensions.NugetApp.Views;
using Prism.Ioc;
using System.Windows;

namespace LayUI.Wpf.Extensions.NugetApp
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

        }
    }
}
