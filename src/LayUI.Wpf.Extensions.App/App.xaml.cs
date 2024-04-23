using LayUI.Wpf.Extensions.App.ViewModels;
using LayUI.Wpf.Extensions.App.Views;
using LayUI.Wpf.Global;
using Prism.DryIoc;
using Prism.Ioc;
using System.Windows;
using MessageBox = LayUI.Wpf.Extensions.App.Views.MessageBox;

namespace LayUI.Wpf.Extensions.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App:PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            LayDialog.RegisterDialog<MessageBox, MessageBoxViewModel>("MessageBox");
        }
    }
}
