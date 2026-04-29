namespace MauiYaoya.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Origin { get; set; } = "";
    public int Price { get; set; }
    public ProductCategory Category { get; set; }
}

public enum ProductCategory
{
    Vegetable,  // 野菜
    Fruit       // 果物
}