using mapping_benchmarker.Enums;

namespace mapping_benchmarker.Models;

public class ProductView
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ListingType ListingType { get; set; }

    public decimal Price { get; set; }

    public int ProductDetailsId { get; set; }

    public Language Language { get; set; }

    public static List<Product> ToDomain(List<ProductView> products)
    {
        return products.GroupBy(p => p.Id).Select(g => new Product
        {
            Id = g.First().Id,
            Price = g.First().Price,
            ListingType = g.First().ListingType,
            Categories = new List<Category>(),
            Details = g.Select(item => new ProductDetails
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Language = item.Language
            })
        }).ToList();
    }
}
