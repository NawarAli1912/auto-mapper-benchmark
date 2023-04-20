
using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Bogus;
using mapping_benchmarker.Dtos;
using mapping_benchmarker.Enums;
using mapping_benchmarker.Models;

Console.WriteLine("Hello, World");

var result = BenchmarkRunner.Run<Benchmark>();


[MemoryDiagnoser]
public class Benchmark
{
    private IMapper _mapper;
    private Product _model;
    private List<Product> _products;
    private List<ProductView> _productsView = new();

    [GlobalSetup]
    public void Setup()
    {
        MapperConfiguration config = new(cfg =>
        {
            cfg.CreateMap<Product, ProductDto>()
                .ForMember(
                    dest => dest.Categories,
                    opt => opt.MapFrom(src => src.Categories
                                                 .Select(c => new CategoryDto
                                                 {
                                                     Id = c.Id,
                                                     Name = c.Name,
                                                     Description = c.Description
                                                 })))
                .ForMember(
                    dest => dest.Details,
                    opt => opt.MapFrom(src => src.Details
                                                 .Select(d => new ProductDetailsDto
                                                 {
                                                     Id = d.Id,
                                                     Name = d.Name,
                                                     Description = d.Description,
                                                     Language = d.Language
                                                 })));
            cfg.CreateMap<IEnumerable<ProductView>, IEnumerable<Product>>()
                .ConvertUsing<CustomConverter>();
        });

        _mapper = config.CreateMapper();


        var productFaker = new Faker<Product>()
            .RuleFor(p => p.Id, f => f.Random.Int(0, 20000))
            .RuleFor(p => p.ListingType, f => f.PickRandom<ListingType>())
            .RuleFor(p => p.Price, f => f.Random.Decimal(0, 5000))
            .RuleFor(p => p.Categories, f => f.Random.ListItems(
                    Enumerable.Range(0, 5).Select(_ => new Category
                    {
                        Id = f.Random.Int(0, 20000),
                        Name = f.Random.Word(),
                        Description = f.Lorem.Paragraph()
                    }).ToList()))
            .RuleFor(p => p.Details, f => f.Random.ListItems(
                    Enumerable.Range(1, 5).Select(_ => new ProductDetails
                    {
                        Id = f.Random.Int(0, 2000),
                        Name = f.Random.Word(),
                        Description = f.Lorem.Paragraph(),
                        Language = f.PickRandom<Language>()
                    }).ToList()));

        var productViewFaker = new Faker<ProductView>()
            .RuleFor(p => p.Id, f => f.Random.Int(0, 20000))
            .RuleFor(p => p.ProductDetailsId, 1)
            .RuleFor(p => p.Name, f => f.Random.Word())
            .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
            .RuleFor(p => p.ListingType, f => f.PickRandom<ListingType>())
            .RuleFor(p => p.Price, f => f.Random.Decimal(0, 5000))
            .RuleFor(p => p.Language, f => Language.English)
            .RuleFor(p => p.Price, f => f.Random.Decimal(1000, 5000));




        _model = productFaker.Generate();
        _products = productFaker.Generate(1000);
        var temp = productViewFaker.Generate(500);

        foreach (var product in temp)
        {
            // Create a new product with the same Id but different Language, Name, and Description
            var duplicatedProduct = new ProductView
            {
                Id = product.Id,
                Name = $"{product.Name} - Duplicate",
                Description = $"{product.Description} - Duplicate",
                ListingType = product.ListingType,
                Price = product.Price,
                ProductDetailsId = product.ProductDetailsId,
                Language = product.Language == Language.English ? Language.Arabic : Language.English
            };

            _productsView.Add(duplicatedProduct);
        }

        _productsView.AddRange(temp);
    }

    [Benchmark]
    [IterationCount(100)]
    public ProductDto ManualMappingSingle()
    {
        return ProductDto.FromModel(_model);
    }

    [Benchmark]
    [IterationCount(100)]
    public ProductDto AutoMapperSingle()
    {
        return _mapper.Map<ProductDto>(_model);
    }

    [Benchmark]
    [IterationCount(100)]
    public List<ProductDto> ManualMappingList()
    {
        return ProductDto.FromModel(_products).ToList();
    }

    [Benchmark]
    [IterationCount(100)]
    public List<ProductDto> AutoMapperList()
    {
        return _mapper.Map<List<ProductDto>>(_products);
    }

    [Benchmark]
    [IterationCount(100)]
    public List<Product> ManualViewList()
    {
        return ProductView.ToDomain(_productsView);
    }


    [Benchmark]
    [IterationCount(100)]
    public List<Product> AutoMapperViewList()
    {
        return _mapper.Map<List<Product>>(_productsView);
    }
}

/*
|              Method |            Mean |         Error |         StdDev |          Median |     Gen0 |    Gen1 | Allocated |
|-------------------- |----------------:|--------------:|---------------:|----------------:|---------:|--------:|----------:|
| ManualMappingSingle |        27.18 ns |      1.540 ns |       4.393 ns |        26.03 ns |   0.0268 |       - |     168 B |
|    AutoMapperSingle |     1,740.13 ns |     27.197 ns |      77.595 ns |     1,728.83 ns |   0.1030 |       - |     664 B |
|   ManualMappingList |    49,815.33 ns |    565.988 ns |   1,605.613 ns |    49,892.40 ns |  29.3579 |  9.7656 |  184696 B |
|      AutoMapperList | 1,739,535.17 ns | 46,630.333 ns | 133,038.964 ns | 1,695,421.88 ns | 103.5156 | 50.7813 |  660442 B |
|      ManualViewList |   105,145.40 ns |  1,767.581 ns |   5,099.876 ns |   104,419.47 ns |  23.9258 |  5.8594 |  151344 B |
|  AutoMapperViewList |   110,377.45 ns |  2,622.411 ns |   7,439.345 ns |   107,863.04 ns |  23.9258 |  5.8594 |  151405 B |
*/