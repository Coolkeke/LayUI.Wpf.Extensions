# LayUI.WPF.Extensions
这是组件库扩展包
## 使用

步骤一 : 添加LayUI.Wpf.Extensions Nuget包，可以在Nuget管理工具中搜索“LayUI.Wpf.Extensions”，也可以用命令;
```XML
 NuGet\Install-Package LayUI.Wpf.Extensions -Version 1.0.2.240422
```
![image](https://github.com/Coolkeke/LayUI.Wpf.Extensions/assets/37786276/c47c3340-c3c4-4d9a-81bb-a837986f3a78)

步骤二: 加载需要翻译的资源字典 "记住你接下来所有的翻译数据都将会保存在内存中，主要减轻程序反复读取文件给机器造成不必要的压力"

 ![image](https://github.com/Coolkeke/LayUI.Wpf.Extensions/assets/37786276/73b13136-a2de-42a9-8b37-644620204d4f)
 
 这一步至关重要，直接影响到程序运行起来能否直接进行成功的翻译，切记要将目标文件的“生成操作”改成“资源”，如果你想直接读取外部的文件也是可以的，插件内置读取外部XAML文件方法
 
![image](https://github.com/Coolkeke/LayUI.Wpf.Extensions/assets/37786276/37502a0c-ea19-4aad-9774-c1afcb41c96d)
 
步骤三: 跳转到App.xaml下面，将目标的翻译文件加入xaml代码中，让程序启动加载目标翻译文件: 
```XML
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary x:Key="lang">
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="/languages/en_US.xaml" />
                <ResourceDictionary Source="/languages/zh_CN.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </ResourceDictionary>
</Application.Resources>
``` 
![image](https://github.com/Coolkeke/LayUI.Wpf.Extensions/assets/37786276/b829a619-cd8c-4838-866f-001b77286267)

步骤四: 在对应的页面使用资源查找的方式，找到当前定义存储翻译文件集合名字为“lang”资源字典，"lang"这个是步骤三中定义好的，下面是查找目标翻译文件代码:

```XML
LanguageExtension.LoadDictionary(((ResourceDictionary)Application.Current.FindResource("lang")).MergedDictionaries[0]);
```  
步骤五: 在目标翻译界面引用多语言扩展路径: 

```XML
xmlns:lay="clr-namespace:LayUI.Wpf.Extensions;assembly=LayUI.Wpf.Extensions"
```  
 步骤六: 在目标需要翻译的控件上使用多语言扩展标记进行多语言翻译，下面列出几种翻译方式: 
 
```XML
<TextBlock  Text="{lay:Language {Binding Name}}" />
<TextBlock  Text="{lay:Language Key='123'}" />
```

步骤七: 完成

备注：LanguageExtension扩展类中提供代码读取翻译文字等等相关方法，列如

```XML
LanguageExtension.LoadPath("xaml文件路径");
LanguageExtension.LoadResourceKey("查找当前程序内置资源文件名称");
LanguageExtension.LoadDictionary(new ResourceDictionary()); 
LanguageExtension.Refresh();
``` 
![Layui多语言翻译](https://github.com/Coolkeke/LayUI.Wpf.Extensions/assets/37786276/f1878e3f-b777-408d-a7d5-118bb48a3993)
