using LayUI.Wpf.Global;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LayUI.Wpf.Extensions.App.ViewModels
{
    public class MessageBoxViewModel : BindableBase, ILayDialogAware
    {
        private string _Title= "Remind";
        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }
        public MessageBoxViewModel()
        {

        }

        public event Action<ILayDialogResult> RequestClose;

        public void OnDialogClosed()
        {
             
        }

        public void OnDialogOpened(ILayDialogParameter parameters)
        {
             
        }
        private DelegateCommand _CloseCommand;
        public DelegateCommand CloseCommand =>
            _CloseCommand ?? (_CloseCommand = new DelegateCommand(ExecuteCloseCommand));

        void ExecuteCloseCommand()
        {
            RequestClose?.Invoke(new LayDialogResult());
        }
    }
}
