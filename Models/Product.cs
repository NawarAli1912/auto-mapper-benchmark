using mapping_benchmarker.Enums;

namespace mapping_benchmarker.Models;

public class Product
{
    public int Id { get; set; }

    public ListingType ListingType { get; set; }

    public decimal Price { get; set; }

    public IEnumerable<ProductDetails> Details { get; set; } = default!;

    public IEnumerable<Category> Categories { get; set; } = default!;
}
