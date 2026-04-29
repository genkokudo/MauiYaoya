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
        if (!File.Exists(_filePath)) return;
        try
        {
            var json = File.ReadAllText(_filePath);
            _products = JsonSerializer.Deserialize<List<Product>>(json) ?? new();
        }
        catch { _products = new(); }
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(_products,
            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    private void NotifyChanged() => OnChanged?.Invoke();
}