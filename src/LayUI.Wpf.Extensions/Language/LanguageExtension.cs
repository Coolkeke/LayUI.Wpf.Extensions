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
using System.Runtime.InteropServices;

namespace LayUI.Wpf.Extensions
{
    /// <summary>
    /// 多语言扩展类
    /// </summary>
    public class LanguageExtension : MarkupExtension
    {
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
        private static ResourceDictionary source;
        /// <summary>
        /// 翻译数据内存源数据
        /// </summary>
        private static ResourceDictionary Source
        {
            get
            {
                if (source == null) source = new ResourceDictionary();
                return source;
            }
        }
        /// <summary>
        /// 获取内存中已加载的翻译结果
        /// </summary>
        /// <param name="key">唯一标识</param>
        /// <returns></returns>
        public object GetValue(string key)
        {
            if (Source != null && Source.Contains(key)) return Source[key];
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
            source = languageDictionary;
            Refresh();
        }
        /// <summary>
        /// 刷新页面可视化语言
        /// </summary>
        public static void Refresh()
        {
            if (action != null) action.Invoke();
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget)) return this;
            if (provideValueTarget.TargetObject.GetType().FullName == "System.Windows.SharedDp") return this;
            if (!(provideValueTarget.TargetObject is DependencyObject targetObject)) return this;
            if (!(provideValueTarget.TargetProperty is DependencyProperty targetProperty)) return this;
            Action LanguageEvent = null;
            LanguageEvent = () =>
            {
                BindingOperations.SetBinding(targetObject, targetProperty, CreateBinding(Key));
            };
            RoutedEventHandler loaded = null;
            RoutedEventHandler unLoaded = null;
            loaded = (o, e) =>
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
                elementDataContextChanged += (o, e) =>
                {
                    element.DataContextChanged -= elementDataContextChanged;
                    element.DataContextChanged += elementDataContextChanged;
                    BindingOperations.SetBinding(targetObject, targetProperty, CreateBinding(Key));
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
                Source = Source,
                Mode = BindingMode.OneWay
            });
            binding.Converter = new LanguageConverter();
            return binding;
        }

    }

}
