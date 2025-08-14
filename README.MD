## JsonLocalization
小有意思的一个项目，使用json配置文件 + Options 取代资源文件做本地化

### 使用步骤
#### 1 定义你的数据类型
```c#
public class Locale
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

    // 添加本地化工具，默认语言区域为 "en"
    builder.AddLocalizer<Locale>(defaultCulture: "en");

    builder.Services.AddControllers();
    var app = builder.Build();

    // 中间件设置当前线程的语言区域
    app.Use(next => context =>
    {
        context.Request.Query.TryGetValue("culture", out var culture);
        var options = context.RequestServices.GetRequiredService<IOptions<LocalizerOptions>>();
        Thread.CurrentThread.CurrentCulture = options.Value.GetCulture(culture);
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
    /// <param name="locale">当前线程对应的本地化数据</param>
    /// <param name="localizer">本地化工具</param>
    /// <returns></returns>
    [HttpGet("/")]
    public Locale Index(
        [FromServices] Locale locale,
        [FromServices] ILocalizer<Locale> localizer)
    {
        Debug.Assert(locale == localizer.Current);
        return locale;
    }
}
```

#### 4 添加语言文件
1. 新建`cultures`目录，创建`zh.json`、`zh-CN.json`等空白语言文件
2. 启动运行你的项目
3. 在`cultures`目录找到相关的`.value`文件，粘贴到对应的`.json`语言文件并翻译修改