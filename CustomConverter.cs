using AutoMapper;
using mapping_benchmarker.Models;

public class CustomConverter : ITypeConverter<IEnumerable<ProductView>, IEnumerable<Product>>
{
    public IEnumerable<Product> Convert(IEnumerable<ProductView> source, IEnumerable<Product> destination, ResolutionContext context)
    {
        var result = new List<Product>();

        var groups = source.GroupBy(pv => pv.Id);

        return groups.Select(g => new Product
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