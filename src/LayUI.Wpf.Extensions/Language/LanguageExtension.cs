using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;

namespace LayUI.Wpf.Extensions
{
    /// <summary>
    /// 多语言扩展类
    /// </summary>
    public class LanguageExtension : MarkupExtension
    {
        private static Action action;
        private Binding _Key;
        public LanguageExtension() { }
        public LanguageExtension(Binding key) : this() => Key = key;
        /// <summary>
        /// 绑定源
        /// </summary>
        public Binding Key
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
            object value = null;
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget)) return this;
            if (provideValueTarget.TargetObject.GetType().FullName == "System.Windows.SharedDp") return this;
            if (!(provideValueTarget.TargetObject is DependencyObject targetObject)) return this;
            if (!(provideValueTarget.TargetProperty is DependencyProperty targetProperty)) return this;
            if (targetObject is FrameworkElement element && element.DataContext != null)
            {
                action += () =>
                {
                    BindingOperations.SetBinding(targetObject, targetProperty, new Binding
                    {
                        Path = _Key.Path,
                        Source = element.DataContext,
                        UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
                        Mode = BindingMode.OneWay
                    });
                    value = element.GetValue(targetProperty) as string;
                    value = Source[value] == null ? value : Source[value];
                    element.SetValue(targetProperty, value);
                };
                Refresh();
                value = Source[value] == null ? value : Source[value];
            }
            return value;
        }
    }
}
