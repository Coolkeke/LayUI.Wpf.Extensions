using LayUI.Wpf.Global;
using Microsoft.Xaml.Behaviors;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace LayUI.Wpf.Extensions.App.ViewModels
{
    public class WindowBehavior : Behavior<Window>
    {
        public ICommand InitializedCommand
        {
            get { return (ICommand)GetValue(InitializedCommandProperty); }
            set { SetValue(InitializedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InitializedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InitializedCommandProperty =
            DependencyProperty.Register("InitializedCommand", typeof(ICommand), typeof(WindowBehavior));
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Initialized += AssociatedObject_Initialized;
        }
        private void AssociatedObject_Initialized(object sender, EventArgs e)
        {
            InitializedCommand?.Execute(null);
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Initialized -= AssociatedObject_Initialized;
        }

    }
    public class Language : BindableBase
    {
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }
        private string _Icon;
        public string Icon
        {
            get { return _Icon; }
            set { SetProperty(ref _Icon, value); }
        }
        private string _Key;
        public string Key
        {
            get { return _Key; }
            set { SetProperty(ref _Key, value); }
        }

    }
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
        private List<Language> _Languages = new List<Language>()
        {
            new Language(){ Title="中文",Icon="/Images/Svg/cn.svg",Key="zh_CN" },
            new Language(){ Title="英语",Icon="/Images/Svg/um.svg",Key="en_US" },
        };
        public List<Language> Languages
        {
            get { return _Languages; }
            set { SetProperty(ref _Languages, value); }
        }
        private Language _Language;
        public Language Language
        {
            get { return _Language; }
            set
            {
                SetProperty(ref _Language, value);
                LanguageExtension.LoadResourceKey(Language.Key);
            }
        }
        private DelegateCommand _InitializedCommand;
        public DelegateCommand InitializedCommand =>
            _InitializedCommand ?? (_InitializedCommand = new DelegateCommand(ExecuteInitializedCommand));

        void ExecuteInitializedCommand()
        {
            Language = Languages.FirstOrDefault();

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
        }
        private DelegateCommand _USCommand;
        public DelegateCommand USCommand =>
            _USCommand ?? (_USCommand = new DelegateCommand(ExecuteUSCommand));

        void ExecuteUSCommand()
        {
            LanguageExtension.LoadResourceKey("en_US");
        }
        private DelegateCommand _CNCommand;
        public DelegateCommand CNCommand =>
            _CNCommand ?? (_CNCommand = new DelegateCommand(ExecuteCNCommand));

        void ExecuteCNCommand()
        {
            LanguageExtension.LoadResourceKey("zh_CN");
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
            LayDialog.Show("MessageBox", null, "Root");
        }
    }
}
