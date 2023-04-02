using mapping_benchmarker.Enums;
using mapping_benchmarker.Models;

namespace mapping_benchmarker.Dtos;

public class ProductDetailsDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Language Language { get; set; }

    public static ProductDetailsDto FromModel(ProductDetails model)
    {
        return new ProductDetailsDto
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Language = model.Language
        };
    }

    public static IEnumerable<ProductDetailsDto> FromModel(IEnumerable<ProductDetails> details)
    {
        foreach (var model in details)
        {
            yield return FromModel(model);
        }
    }

    public ProductDetails ToModel()
    {
        return new ProductDetails
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Language = Language
        };
    }
}