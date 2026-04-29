using ModelContextProtocol.Server;
using System.ComponentModel;
using MauiYaoya.Models;
using MauiYaoya.Services;

namespace MauiYaoya.Mcp;

[McpServerToolType]
public class YaoyaTools
{
    private readonly ProductService _productService;

    public YaoyaTools(ProductService productService)
    {
        _productService = productService;
    }

    [McpServerTool, Description("八百屋の商品一覧を取得する")]
    public string GetProducts()
    {
        var products = _productService.GetAll();
        if (!products.Any()) return "商品はありません。";

        var lines = products.Select(p =>
            $"[{p.Id}] {p.Name}（{p.Origin}産）¥{p.Price} - " +
            $"{(p.Category == ProductCategory.Vegetable ? "野菜" : "果物")}");
        return string.Join("\n", lines);
    }

    [McpServerTool, Description("商品を追加する")]
    public string AddProduct(
        [Description("商品名")] string name,
        [Description("産地")] string origin,
        [Description("価格（円）")] int price,
        [Description("カテゴリ: Vegetable or Fruit")] string category)
    {
        if (!Enum.TryParse<ProductCategory>(category, true, out var cat))
            return $"カテゴリが不正です。'Vegetable' か 'Fruit' を指定してください。";

        var product = _productService.Add(name, origin, price, cat);
        return $"追加しました: [{product.Id}] {product.Name}";
    }

    [McpServerTool, Description("商品をIDで削除する")]
    public string RemoveProduct(
        [Description("削除する商品のID（GUIDの文字列）")] string id)
    {
        if (!Guid.TryParse(id, out var guid))
            return "IDの形式が不正です。";

        return _productService.Remove(guid)
            ? $"削除しました: {id}"
            : $"該当する商品が見つかりません: {id}";
    }

    [McpServerTool, Description("商品情報を更新する（変更したい項目だけ指定）")]
    public string UpdateProduct(
        [Description("更新する商品のID")] string id,
        [Description("新しい商品名（省略可）")] string? name = null,
        [Description("新しい産地（省略可）")] string? origin = null,
        [Description("新しい価格（省略可）")] int? price = null,
        [Description("新しいカテゴリ Vegetable/Fruit（省略可）")] string? category = null)
    {
        if (!Guid.TryParse(id, out var guid))
            return "IDの形式が不正です。";

        ProductCategory? cat = null;
        if (category is not null)
        {
            if (!Enum.TryParse<ProductCategory>(category, true, out var parsed))
                return "カテゴリが不正です。";
            cat = parsed;
        }

        return _productService.Update(guid, name, origin, price, cat)
            ? $"更新しました: {id}"
            : $"該当する商品が見つかりません: {id}";
    }
}