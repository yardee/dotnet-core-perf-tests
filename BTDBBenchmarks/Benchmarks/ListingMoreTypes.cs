using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmarks
{
    [MemoryDiagnoser]
    public class ListMoreTypes : BaseBtdbBenchmark
    {
        [Params(100, 20000)]
        public override int SlovakiaTotal { get; set; }

        [Benchmark]
        public IList<Person> ListAllAndLinq()
        {
            return Benchmark(personTable => personTable.ListById()
                .Where(p => p.Country == Country.Czech || p.Country == Country.Germany).ToList());
        }

        [Benchmark]
        public IList<Person> UseKeyAndConcat()
        {
            return Benchmark(personTable => personTable.ListByCountry(Country.Czech)
                .Concat(personTable.ListByCountry(Country.Germany)).ToList());
        }
    }
}