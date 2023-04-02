using mapping_benchmarker.Enums;

namespace mapping_benchmarker.Models;

public class ProductDetails
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Language Language { get; set; }
}