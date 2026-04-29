using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using MauiYaoya.Services;
using MauiYaoya.Mcp;
using ModelContextProtocol.AspNetCore;
using Microsoft.AspNetCore.Builder;

namespace MauiYaoya;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Blazor WebView
        builder.Services.AddMauiBlazorWebView();

        // MudBlazor
        builder.Services.AddMudServices();

        // 八百屋サービス（シングルトン必須！MCPとBlazorで共有するため）
        builder.Services.AddSingleton<ProductService>();

        // MCPツール登録
        builder.Services.AddSingleton<YaoyaTools>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var mauiApp = builder.Build();

        // MAUIアプリ起動後にKestrelでMCPサーバーを別スレッドで起動
        Task.Run(() => StartMcpServer(mauiApp.Services));

        return mauiApp;
    }

    private static void StartMcpServer(IServiceProvider mauiServices)
    {
        var webApp = WebApplication.CreateBuilder();

        // MAUIのDIコンテナからProductServiceを取得して共有
        var productService = mauiServices.GetRequiredService<ProductService>();
        webApp.Services.AddSingleton(productService);

        // MCPサーバー設定
        webApp.Services
            .AddMcpServer()
            .WithHttpTransport()
            .WithTools<YaoyaTools>();

        var app = webApp.Build();
        app.MapMcp("/mcp");  // SSEエンドポイント: http://localhost:5000/mcp

        app.Run("http://localhost:5000");
    }
}