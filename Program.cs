
using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Bogus;
using mapping_benchmarker.Dtos;
using mapping_benchmarker.Enums;
using mapping_benchmarker.Models;

Console.WriteLine("Hello, World");

var result = BenchmarkRunner.Run<Benchmark>();


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
|              Method |            Mean |        Error |        StdDev |          Median |
|-------------------- |----------------:|-------------:|--------------:|----------------:|
| ManualMappingSingle |        16.50 ns |     0.057 ns |      0.160 ns |        16.49 ns |
|    AutoMapperSingle |     1,404.34 ns |     3.613 ns |     10.248 ns |     1,406.20 ns |
|   ManualMappingList |    35,925.42 ns |   109.094 ns |    300.478 ns |    36,004.57 ns |
|      AutoMapperList | 1,220,364.46 ns | 3,584.858 ns |  9,933.624 ns | 1,220,637.11 ns |
|      ManualViewList |   111,707.04 ns | 5,044.992 ns | 14,555.956 ns |   108,664.64 ns |
|  AutoMapperViewList |   104,618.12 ns | 2,817.795 ns |  7,947.640 ns |   101,539.36 ns |
*/