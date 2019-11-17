using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmarks
{
    [MemoryDiagnoser]
    public class ListingSingleType : BaseBtdbBenchmark
    {
        [Params(100, 20000)]
        public override int SlovakiaTotal { get; set; }

        [Benchmark]
        public IList<Person> UseKey()
        {
            return Benchmark(personTable => personTable.ListByCountry(Country.Slovakia).ToList());
        }

        [Benchmark]
        public IList<Person> ListAllAndConcat()
        {
            return Benchmark(personTable =>
                personTable.ListById().Where(p => p.Country == Country.Slovakia).ToList());
        }
    }
}