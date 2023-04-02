using mapping_benchmarker.Enums;
using mapping_benchmarker.Models;

namespace mapping_benchmarker.Dtos;

public class ProductDto
{
    public int Id { get; set; }

    public ListingType ListingType { get; set; }

    public decimal Price { get; set; }

    public IEnumerable<ProductDetailsDto> Details { get; set; } = default!;

    public IEnumerable<CategoryDto> Categories { get; set; } = default!;

    public Product ToModel()
    {
        return new Product
        {
            Id = Id,
            Price = Price,
            ListingType = ListingType,
            Categories = Categories.Select(c => c.ToModel()),
            Details = Details.Select(d => d.ToModel())
        };
    }

    public static ProductDto FromModel(Product model)
    {
        return new ProductDto
        {
            Id = model.Id,
            Price = model.Price,
            ListingType = model.ListingType,
            Details = ProductDetailsDto.FromModel(model.Details),
            Categories = CategoryDto.FromModel(model.Categories)
        };
    }

    public static IEnumerable<ProductDto> FromModel(IEnumerable<Product> models)
    {
        foreach (var model in models)
        {
            yield return ProductDto.FromModel(model);
        }
    }
}
