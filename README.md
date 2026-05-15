# MauiYaoya
とりあえずデスクトップアプリで、尚且つMCPによってClaudeから操作することが可能。  
WPFのように描画スレッド問題もなく、Claudeによるデータ更新がかかった後ちゃんと画面に反映される。  
画面の見た目も悪くない。

開発には以下のプロジェクトテンプレートを使用。
```
dotnet new maui-blazor -n MyApp
```

## HTTP/SSEで接続する
あらかじめアプリを起動した状態で、Claudeを起動すること。  
Claude DesktopはローカルHTTP/SSEに直接対応していないため、mcp-remoteを使用する。  
mcp-remoteを使用するにはNode.jsのインストールが必要。
```
{
  "mcpServers": {
    "yaoya": {
      "command": "npx",
      "args": [
        "mcp-remote",
        "http://localhost:5000/"
      ]
    }
  }
}
```

## 欠点？
この方法の欠点は、Android/iOS互換にするための余分なコードがプロジェクト作成時についてくるので、必要に応じてこれらを削ぎ落す必要があること。  
最近はWinUIが良くなったらしいので、デスクトップアプリに関してはそちらで作った方が一番良さそう。  
勿論Webの場合はBlazor。
