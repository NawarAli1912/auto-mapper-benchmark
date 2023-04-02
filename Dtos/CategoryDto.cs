using mapping_benchmarker.Models;

namespace mapping_benchmarker.Dtos;

public class CategoryDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public static CategoryDto FromModel(Category model)
    {
        return new CategoryDto
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
        };
    }

    public static IEnumerable<CategoryDto> FromModel(IEnumerable<Category> categories)
    {
        foreach (var model in categories)
        {
            yield return FromModel(model);
        }
    }

    public Category ToModel()
    {
        return new Category
        {
            Id = Id,
            Name = Name,
            Description = Description
        };
    }
}
