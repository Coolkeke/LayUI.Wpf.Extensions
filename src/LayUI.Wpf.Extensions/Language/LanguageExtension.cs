using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LayUI.Wpf.Extensions
{
    public class MarkupExtensionBindableBase : MarkupExtension, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);
            return true;
        }
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(propertyName);
        }
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        public  override object ProvideValue(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 多语言扩展类
    /// </summary>
    public class LanguageExtension : MarkupExtensionBindableBase
    {
        internal static LanguageExtension Instance = new LanguageExtension(); 
        private class LanguageConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (values.Length != 2 || values[0] is null || !(values[1] is ResourceDictionary)) return string.Empty;
                if (values[0].Equals(DependencyProperty.UnsetValue)) return parameter;
                var key = values[0].ToString();
                var lanugages = (ResourceDictionary)values[1];
                var value = lanugages.Contains(key) ? lanugages[key] : key;
                return value;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        private static Action action;
        private object _Key;
        public LanguageExtension() { }
        public LanguageExtension(object key) : this()
        {
            _Key = key;
        }
        /// <summary>
        /// 绑定源
        /// </summary>
        public object Key
        {
            get { return _Key; }
            set { _Key = value; }
        }
          
        private ResourceDictionary _Source;
        /// <summary>
        /// 翻译数据内存源数据
        /// </summary>
        public ResourceDictionary Source
        {
            get
            {
                if (_Source == null) _Source = new ResourceDictionary();
                return _Source;
            }
            set 
            { 
                SetProperty(ref _Source, value); 
            }
        }
        /// <summary>
        /// 获取内存中已加载的翻译结果
        /// </summary>
        /// <param name="key">唯一标识</param>
        /// <returns></returns>
        public static object GetValue(string key)
        {
            if (Instance.Source != null && Instance.Source.Contains(key)) return Instance.Source[key];
            return key;
        }
        /// <summary>
        /// 根据系统资源名称加载多语言
        /// </summary>
        /// <param name="key"></param>
        public static void LoadResourceKey(string key)
        {
            var languageDatas = Application.Current.TryFindResource(key);
            if (languageDatas is ResourceDictionary dictionary) LoadDictionary(dictionary);
            else LoadDictionary(new ResourceDictionary());
        }
        /// <summary>
        /// 根据路径加载多语言
        /// </summary>
        /// <param name="path">翻译文件路径</param>
        public static void LoadPath(string path)
        {
            ResourceDictionary languageDictionary = null;
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    languageDictionary = (ResourceDictionary)XamlReader.Load(fs);
                }
            }
            else languageDictionary = new ResourceDictionary() { Source = new Uri(path) };
            LoadDictionary(languageDictionary);
        }

        /// <summary>
        /// 根据指定的资源字典加载多语言
        /// </summary>
        /// <param name="languageDictionary"></param>
        public static void LoadDictionary(ResourceDictionary languageDictionary)
        {
            Instance.Source = languageDictionary;
            Refresh();
        }
        /// <summary>
        /// 刷新页面可视化语言
        /// </summary>
        public static void Refresh()
        {
            lock (Instance.Source)
            { 
                AddApplicationResourcesLanguage();
                action?.Invoke();
            }
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget)) return this;
            if (provideValueTarget.TargetObject.GetType().FullName == "System.Windows.SharedDp") return this;
            if (!(provideValueTarget.TargetObject is DependencyObject targetObject)) return this;
            if (!(provideValueTarget.TargetProperty is DependencyProperty targetProperty)) return this;
            try
            {
                Action LanguageEvent = null;
                LanguageEvent = async () =>
                {
                    await targetObject.Dispatcher.InvokeAsync(() =>
                     {
                         BindingOperations.SetBinding(targetObject, targetProperty, CreateBinding(Key));
                     });
                }; ;
                RoutedEventHandler loaded = null;
                RoutedEventHandler unLoaded = null;
                loaded = async (o, e) =>
                {
                    action += LanguageEvent;
                    if (o is FrameworkElement)
                    {
                        (o as FrameworkElement).Loaded -= loaded;
                        (o as FrameworkElement).Loaded += loaded;
                    }
                    if (o is FrameworkContentElement)
                    {
                        (o as FrameworkContentElement).Loaded -= loaded;
                        (o as FrameworkContentElement).Loaded += loaded;
                    }
                    await targetObject.Dispatcher.InvokeAsync(() =>
                    {
                        BindingOperations.SetBinding(targetObject, targetProperty, CreateBinding(Key));
                    });
                };
                unLoaded = (o, e) =>
                {
                    action -= LanguageEvent;
                    if (o is FrameworkElement)
                    {
                        (o as FrameworkElement).Unloaded -= unLoaded;
                        (o as FrameworkElement).Unloaded += unLoaded;
                    }
                    else if (o is FrameworkContentElement)
                    {
                        (o as FrameworkContentElement).Unloaded -= unLoaded;
                        (o as FrameworkContentElement).Unloaded += unLoaded;
                    }
                };
                if (targetObject is FrameworkElement element)
                {
                    element.Loaded += loaded;
                    element.Unloaded += unLoaded;
                    DependencyPropertyChangedEventHandler elementDataContextChanged = null;
                    elementDataContextChanged += async (o, e) =>
                    {
                        element.DataContextChanged -= elementDataContextChanged;
                        element.DataContextChanged += elementDataContextChanged;
                        await element.Dispatcher.InvokeAsync(() =>
                         {
                             BindingOperations.SetBinding(targetObject, targetProperty, CreateBinding(Key));
                         });
                    };
                    element.DataContextChanged += elementDataContextChanged;
                }
                else if (targetObject is FrameworkContentElement contentElement)
                {
                    contentElement.Loaded += loaded;
                    contentElement.Unloaded += unLoaded;
                    DependencyPropertyChangedEventHandler contentElementDataContextChanged = null;
                    contentElementDataContextChanged += (o, e) =>
                    {
                        contentElement.DataContextChanged -= contentElementDataContextChanged;
                        contentElement.DataContextChanged += contentElementDataContextChanged;
                        BindingOperations.SetBinding(targetObject, targetProperty, CreateBinding(Key));
                    };
                    contentElement.DataContextChanged += contentElementDataContextChanged;
                } 
                return CreateBinding(Key).ProvideValue(serviceProvider);
            }
            catch (Exception ex)
            {
                throw new XamlParseException(ex.Message);
            }
        }
        /// <summary>
        /// 给系统资源新增多语言文件
        /// </summary>
        private static bool AddApplicationResourcesLanguage()
        {
            try
            {
                var window = Application.Current?.MainWindow;
                if (window != null && !DesignerProperties.GetIsInDesignMode(window))
                {
                    if (Instance.Source == null) return false;
                    var lang = Application.Current.Resources?.MergedDictionaries?.Where(o => o.Source == Instance.Source.Source)?.FirstOrDefault();
                    if (lang != null) Application.Current.Resources?.MergedDictionaries?.Remove(lang);
                    Application.Current.Resources?.MergedDictionaries?.Add(Instance.Source);
                }
                return true;
            }
            catch { }
            return false;
        }
        /// <summary>
        /// 创建多语言绑定代码
        /// </summary>
        /// <param name="dependency"></param>
        /// <param name="property"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private MultiBinding CreateBinding(object key)
        {
            MultiBinding binding = new MultiBinding();
            if (key is Binding sorueBinding)
            {
                binding.ConverterParameter = sorueBinding.Path.Path;
                binding.Bindings.Add(sorueBinding);
            }
            else
            {
                binding.ConverterParameter = key;
                binding.Bindings.Add(new Binding()
                {
                    Source = key,
                    Mode = BindingMode.OneWay
                });
            }
            binding.Bindings.Add(new Binding()
            {
                Source = Instance.Source,
                Mode = BindingMode.OneWay
            });
            binding.Converter = new LanguageConverter();
            return binding;
        }

    }

}
