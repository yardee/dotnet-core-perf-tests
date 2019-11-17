using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmarks
{
    [MemoryDiagnoser]
    public class SingleValue: BaseBtdbBenchmark
    {
        public override int SlovakiaTotal { get; set; } = 100;
        
        [Benchmark]
        public Person ListAllAndLinq()
        {
            return Benchmark(personTable => personTable.ListById().Single(p => p.Id == 1000));
        }

        [Benchmark]
        public Person UseKey()
        {
            return Benchmark(personTable => personTable.FindById(1000));
        }
    }
}