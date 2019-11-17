using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Benchmarks.Tables;
using BTDB.KVDBLayer;
using BTDB.ODBLayer;

namespace Benchmarks.Benchmarks
{
    public abstract class BaseBtdbBenchmark
    {
        private const int TotalRecords = 100000;
        private Func<IObjectDBTransaction, IPersonTable> _creator;
        private IObjectDB _db;
        public abstract int SlovakiaTotal { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            Console.WriteLine($"GlobalSetup, SlovakiaTotal: {SlovakiaTotal}");
            _db = new ObjectDB();
            _db.Open(new InMemoryKeyValueDB(), false);

            using var tr = _db.StartTransaction();
            _creator = tr.InitRelation<IPersonTable>("Person");
            var personTable = _creator(tr);

            var nationality = Country.Czech;
            var slovakiaCount = 0;
            for (ulong i = 0; i < TotalRecords; i++)
            {
                personTable.Insert(new Person(i, $"Name{i}", nationality));

                if (i % 2 == 0 && slovakiaCount < SlovakiaTotal)
                {
                    nationality = Country.Slovakia;
                    slovakiaCount++;
                }
                else
                {
                    nationality = nationality switch
                    {
                        Country.Czech => Country.Poland,
                        Country.Poland => Country.Germany,
                        Country.Germany => Country.Czech,
                        Country.Slovakia => Country.Czech,
                        _ => nationality
                    };
                }
            }

            tr.Commit();
        }
        
        protected TResult Benchmark<TResult>(Func<IPersonTable, TResult> test)
        {
            using var tr = _db.StartTransaction();
            var personTable = _creator(tr);
            return test(personTable);
        }
    }
}