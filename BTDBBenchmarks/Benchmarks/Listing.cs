using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Benchmarks.Tables;
using BTDB.KVDBLayer;
using BTDB.ODBLayer;

namespace Benchmarks.Benchmarks
{
    [MemoryDiagnoser]
    public class Listing
    {
        private const int TotalRecords = 100000;
        private Func<IObjectDBTransaction, IPersonTable> _creator;
        private IObjectDB _db;
        private const int SlovakiaTotal = 2000;
        private const int CzechTotal = 50000;

        [GlobalSetup]
        public void GlobalSetup()
        {
            
            _db = new ObjectDB();
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "BTDBDist");
            Directory.CreateDirectory(directory);
            
            Console.WriteLine($"GlobalSetup, db: {directory}");
            _db.Open(new KeyValueDB(new OnDiskFileCollection(directory)), true);

            using var tr = _db.StartTransaction();
            _creator = tr.InitRelation<IPersonTable>("Person");
            var personTable = _creator(tr);
            if (personTable.CountByCountry(Country.Czech) > 0)
            {
                return;
            }
            var nationality = Country.Czech;
            var slovakiaCount = 0;
            var czechCount = 0;
            for (ulong i = 0; i < TotalRecords; i++)
            {
                personTable.Insert(new Person(i, $"Name{i}", nationality, 0));

                if (i % 2 == 0 && slovakiaCount < SlovakiaTotal)
                {
                    nationality = Country.Slovakia;
                    slovakiaCount++;
                }
                else if (czechCount < CzechTotal)
                {
                    nationality = Country.Czech;
                    czechCount++;
                }
                else
                {
                    nationality = nationality switch
                    {
                        Country.Czech => Country.Poland,
                        Country.Poland => Country.Germany,
                        Country.Germany => Country.Poland,
                        Country.Slovakia => Country.Poland,
                        _ => nationality
                    };
                }
            }

            foreach (var countryType in Enum.GetValues(typeof(Country)))
            {
                var count = personTable.CountByCountry((Country) countryType);
                Console.WriteLine($"{(Country) countryType} Total: {count}");
            }
            
            tr.Commit();
        }
        
        [Benchmark]
        public IList<Person> ListSingleType_2000FromTotal_UseSecondaryKey()
        {
            return Benchmark(personTable => personTable.ListByCountry(Country.Slovakia).ToList());
        }
        
        [Benchmark]
        public IList<Person> ListSingleType_2000FromTotal_UseLinq()
        {
            return Benchmark(personTable => personTable.ListById().Where(p => p.Country == Country.Slovakia).ToList());
        }
        
        [Benchmark]
        public IList<Person> ListSingleType_24000FromTotal_UseSecondaryKey()
        {
            return Benchmark(personTable => personTable.ListByCountry(Country.Poland).ToList());
        }
        
        [Benchmark]
        public IList<Person> ListSingleType_24000FromTotal_UseLinq()
        {
            return Benchmark(personTable => personTable.ListById().Where(p => p.Country == Country.Poland).ToList());
        }
        
//        [Benchmark]
        public IList<Person> ListSingleType_50000FromTotal_UseSecondaryKey()
        {
            return Benchmark(personTable => personTable.ListByCountry(Country.Czech).ToList());
        }
        
//        [Benchmark]
        public IList<Person> ListSingleType_50000FromTotal_UseLinq()
        {
            return Benchmark(personTable => personTable.ListById().Where(p => p.Country == Country.Czech).ToList());
        }
        
        private TResult Benchmark<TResult>(Func<IPersonTable, TResult> test)
        {
            using var tr = _db.StartTransaction();
            var personTable = _creator(tr);
            return test(personTable);
        }
    }
}