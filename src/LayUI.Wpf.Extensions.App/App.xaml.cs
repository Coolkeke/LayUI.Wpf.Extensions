using LayUI.Wpf.Extensions.App.Views;
using Prism.Ioc;
using System.Windows;

namespace LayUI.Wpf.Extensions.App
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
