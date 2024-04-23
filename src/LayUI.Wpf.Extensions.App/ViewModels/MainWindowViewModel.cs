using LayUI.Wpf.Global;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;

namespace LayUI.Wpf.Extensions.App.ViewModels
{
    public class Data : BindableBase
    {
        private string _Value;
        public string Value
        {
            get { return _Value; }
            set { SetProperty(ref _Value, value); }
        }
    }

    public class MainWindowViewModel : BindableBase
    {
        private List<ResourceDictionary> _Languages = new List<ResourceDictionary>()
        {
            ((ResourceDictionary)Application.Current.FindResource("lang")).MergedDictionaries[0],
            ((ResourceDictionary)Application.Current.FindResource("lang")).MergedDictionaries[1],
        };
        public List<ResourceDictionary> Languages
        {
            get { return _Languages; }
            set { SetProperty(ref _Languages, value); }
        }

        private ObservableCollection<Data> _Items;
        public ObservableCollection<Data> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }

        private string _title = nameof(Title);
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _Name = nameof(Name);
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }
        public MainWindowViewModel()
        {
            Items = new ObservableCollection<Data>
            {
                new Data(){ Value="Test" },
                new Data(){ Value="Test" },
                new Data(){ Value="Test" },
                new Data(){ Value="Test" },
                new Data(){ Value="Test" },
                new Data(){ Value="Test" },
                new Data(){ Value="Test" },
                new Data(){ Value="Test" },
            };
            LanguageExtension.LoadDictionary(Languages[1]);
        }
        private DelegateCommand _USCommand;
        public DelegateCommand USCommand =>
            _USCommand ?? (_USCommand = new DelegateCommand(ExecuteUSCommand));

        void ExecuteUSCommand()
        {
            LanguageExtension.LoadDictionary(Languages[0]);
        }
        private DelegateCommand _CNCommand;
        public DelegateCommand CNCommand =>
            _CNCommand ?? (_CNCommand = new DelegateCommand(ExecuteCNCommand));

        void ExecuteCNCommand()
        {
            LanguageExtension.LoadDictionary(Languages[1]);
        }
        private DelegateCommand _LoadItemsCommand;
        public DelegateCommand LoadItemsCommand =>
            _LoadItemsCommand ?? (_LoadItemsCommand = new DelegateCommand(ExecuteLoadItemsCommand));

        void ExecuteLoadItemsCommand()
        {
            Items = new ObservableCollection<Data>
            {
                new Data(){ Value=nameof(Name) },
                new Data(){ Value=nameof(Name) },
                new Data(){ Value=nameof(Name) },
                new Data(){ Value=nameof(Name) },
                new Data(){ Value=nameof(Name) },
                new Data(){ Value=nameof(Name) },
                new Data(){ Value=nameof(Name) },
                new Data(){ Value=nameof(Name) },
            };
        }
        private DelegateCommand _TextChangedCommand;
        public DelegateCommand TextChangedCommand =>
            _TextChangedCommand ?? (_TextChangedCommand = new DelegateCommand(ExecuteTextChangedCommand));

        void ExecuteTextChangedCommand()
        { 
            LanguageExtension.Refresh();
        }
        private DelegateCommand _MessageBoxCommand;
        public DelegateCommand MessageBoxCommand =>
            _MessageBoxCommand ?? (_MessageBoxCommand = new DelegateCommand(ExecuteMessageBoxCommand));

        void ExecuteMessageBoxCommand()
        {
            LayDialog.Show("MessageBox",null,"Root");
        }
    }
}
