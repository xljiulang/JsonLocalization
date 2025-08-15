## OptionsLocalization
小有意思的一个项目，使用json配置文件 + Options 取代资源文件做本地化

### 使用步骤
#### 1 定义你的选项
```c#
public class AppOptions
{
    public string Key1 { get; set; } = "Hello";

    public string Key2 { get; set; } = "World";

    public Dictionary<string, string> DynamicData { get; set; } = new Dictionary<string, string>
    {
        { "Data1", "hello" },
        { "Data2", "world" }        
    };
}
```

#### 2 服务注册
```c#
static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Localizer.LocalizationRoot = "localizations";

    // 添加本地化工具，默认语言区域为 "en"
    builder.AddLocalizer(defaultCulture: "en").Configure<AppOptions>();

    builder.Services.AddControllers();
    var app = builder.Build();

    // 中间件设置当前线程的语言区域
    app.Use(next => context =>
    {
        context.Request.Query.TryGetValue("culture", out var culture);
        Localizer.SetCurrentThreadCulture(culture, "zh");
        return next(context);
    });

    app.MapControllers();
    app.Run();
} 
```


#### 3 本地化数据注入
```c#
public class HomeController
{
    /// <summary>
    /// 本地化工具示例
    /// </summary>
    /// <param name="appOptions">当前线程对应的本地化选项</param>
    /// <param name="localizer">本地化工具</param>
    /// <returns></returns>
    [HttpGet("/")]
    public Locale Index(
        [FromServices] AppOptions appOptions,
        [FromServices] ILocalizer<AppOptions> localizer)
    {
        Debug.Assert(appOptions == localizer.Current);
        return locale;
    }
}
```

#### 4 添加语言文件
1. 打开到`localizations`目录
2. 新建`AppOptions`子目录(目录名与数据类型名保持一致)
3. 在子目录创建`zh.json`、`zh-CN.json`等包含`{}`内容的语言文件
4. 启动运行你的项目
5. 在`AppOptions`子目录找到相关的`.value`文件，粘贴到对应的`.json`语言文件并翻译修改