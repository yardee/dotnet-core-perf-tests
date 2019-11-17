using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmarks
{
    [MemoryDiagnoser]
    public class ListingManyValues : BaseBtdbBenchmark
    {
//        [Benchmark]
//        public IList<Person> ManyValuesListAll()
//        {
//            return Benchmark(personTable => personTable.ListById()
//                .Where(p => p.Country == Country.Czech || p.Country == Country.Germany).ToList());
//        }
//
//        [Benchmark]
//        public IList<Person> ManyValuesUseKey()
//        {
//            return Benchmark(personTable => personTable.ListByCountry(Country.Czech)
//                .Concat(personTable.ListByCountry(Country.Germany)).ToList());
//        }


        [Params(100, 10000)]
        public override int SlovakiaTotal { get; set; }

        [Benchmark]
        public IList<Person> SmallNumberOfValuesUseKey()
        {
            return Benchmark(personTable => personTable.ListByCountry(Country.Slovakia).ToList());
        }

        [Benchmark]
        public IList<Person> SmallNumberOfValuesListAll()
        {
            return Benchmark(personTable =>
                personTable.ListById().Where(p => p.Country == Country.Slovakia).ToList());
        }
    }
}