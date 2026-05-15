using System.Text.Json;
using MauiYaoya.Models;

namespace MauiYaoya.Services;

public class ProductService
{
    private readonly string _filePath;
    private List<Product> _products = new();

    // UIへの変更通知イベント
    public event Action? OnChanged;

    public ProductService()
    {
        var dataDir = FileSystem.AppDataDirectory;
        _filePath = Path.Combine(dataDir, "products.json");
        Load();
    }

    // ---- 読み取り ----
    public IReadOnlyList<Product> GetAll() => _products.AsReadOnly();

    // ---- 追加 ----
    public Product Add(string name, string origin, int price, ProductCategory category)
    {
        var product = new Product
        {
            Name = name,
            Origin = origin,
            Price = price,
            Category = category
        };
        _products.Add(product);
        Save();
        NotifyChanged();
        return product;
    }

    // ---- 削除 ----
    public bool Remove(Guid id)
    {
        var target = _products.FirstOrDefault(p => p.Id == id);
        if (target is null) return false;
        _products.Remove(target);
        Save();
        NotifyChanged();
        return true;
    }

    // ---- 更新 ----
    public bool Update(Guid id, string? name = null, string? origin = null,
                       int? price = null, ProductCategory? category = null)
    {
        var target = _products.FirstOrDefault(p => p.Id == id);
        if (target is null) return false;
        if (name is not null) target.Name = name;
        if (origin is not null) target.Origin = origin;
        if (price is not null) target.Price = price.Value;
        if (category is not null) target.Category = category.Value;
        Save();
        NotifyChanged();
        return true;
    }

    // ---- JSON永続化 ----
    private void Load()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                var loaded = JsonSerializer.Deserialize<List<Product>>(json);
                if (loaded != null && loaded.Count > 0)
                {
                    _products = loaded;
                    return;
                }
            }
            catch { }
        }

        // ファイルが無い or 空っぽの場合はサンプルデータを投入
        _products = new List<Product>
    {
        new() { Name = "トマト",     Origin = "愛知", Price = 150, Category = ProductCategory.Vegetable },
        new() { Name = "きゅうり",   Origin = "宮崎", Price = 80,  Category = ProductCategory.Vegetable },
        new() { Name = "玉ねぎ",     Origin = "北海道", Price = 100, Category = ProductCategory.Vegetable },
        new() { Name = "りんご",     Origin = "青森", Price = 200, Category = ProductCategory.Fruit },
        new() { Name = "みかん",     Origin = "愛媛", Price = 120, Category = ProductCategory.Fruit },
    };
        Save(); 
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(_products,
            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    private void NotifyChanged() => OnChanged?.Invoke();
}