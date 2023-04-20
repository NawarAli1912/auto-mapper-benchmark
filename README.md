# auto-mapper-benchmark
|              Method |            Mean |         Error |         StdDev |          Median |     Gen0 |    Gen1 | Allocated |
|-------------------- |----------------:|--------------:|---------------:|----------------:|---------:|--------:|----------:|
| ManualMappingSingle |        27.18 ns |      1.540 ns |       4.393 ns |        26.03 ns |   0.0268 |       - |     168 B |
|    AutoMapperSingle |     1,740.13 ns |     27.197 ns |      77.595 ns |     1,728.83 ns |   0.1030 |       - |     664 B |
|   ManualMappingList |    49,815.33 ns |    565.988 ns |   1,605.613 ns |    49,892.40 ns |  29.3579 |  9.7656 |  184696 B |
|      AutoMapperList | 1,739,535.17 ns | 46,630.333 ns | 133,038.964 ns | 1,695,421.88 ns | 103.5156 | 50.7813 |  660442 B |
|      ManualViewList |   105,145.40 ns |  1,767.581 ns |   5,099.876 ns |   104,419.47 ns |  23.9258 |  5.8594 |  151344 B |
|  AutoMapperViewList |   110,377.45 ns |  2,622.411 ns |   7,439.345 ns |   107,863.04 ns |  23.9258 |  5.8594 |  151405 B |
