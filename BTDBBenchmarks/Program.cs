using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BTDB.KVDBLayer;
using BTDB.ODBLayer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
//            var l = new Listing();
//            l.GlobalSetup();
//            while (true)
//            {
//                var p1 = l.SmallNumberOfValuesUseKeyListBy();
//                Console.WriteLine($"Count: {p1.Count}");
//            }
        }
    }

    
    public interface IPersonTable
    {
        void Insert(Person person);
        IEnumerable<Person> ListById();
        IEnumerable<Person> ListByNationality(Nationality nationality);
    }
    
    [MemoryDiagnoser]
    public class Listing {
        private Func<IObjectDBTransaction, IPersonTable> _creator;
        private IObjectDB _db;
        private const int TotalRecords = 100000;
        private const int SlovakiaTotal = 100;
            
        [GlobalSetup]
        public void GlobalSetup()
        {
            _db = new ObjectDB();
            _db.Open(new InMemoryKeyValueDB(), false);
                
            using var tr = _db.StartTransaction();
            _creator = tr.InitRelation<IPersonTable>("Person");
            var personTable = _creator(tr);

            var nationality = Nationality.Czech;
            var slovakiaCount = 0;
            for (ulong i = 0; i < TotalRecords; i++)
            {
                personTable.Insert(new Person(i, $"Name{i}", nationality));

                if (i % 2 == 0 && slovakiaCount < SlovakiaTotal)
                {
                    nationality = Nationality.Slovakia;
                    slovakiaCount++;
                }
                else
                {
                    nationality = nationality switch
                    {
                        Nationality.Czech => Nationality.Dutch,
                        Nationality.Dutch => Nationality.German,
                        Nationality.German => Nationality.Czech,
                        Nationality.Slovakia => Nationality.Czech,
                        _ => nationality
                    };
                }
            }
            
            tr.Commit();
        }

        [Benchmark]
        public IList<Person> MoreValuesListAll()
        {
            using var tr = _db.StartTransaction();
            var personTable = _creator(tr);
            return personTable.ListById()
                .Where(p => p.Nationality == Nationality.Czech || p.Nationality == Nationality.German).ToList();
        }
        
        [Benchmark]
        public IList<Person> MoreValuesUseKey()
        {
            using var tr = _db.StartTransaction();
            var personTable = _creator(tr);
            var persons = personTable.ListByNationality(Nationality.Czech).Concat(personTable.ListByNationality(Nationality.German)).ToList();
            return persons;
        }

        [Benchmark]
        public IList<Person> SmallNumberOfValuesUseKeyListBy()
        {
            using var tr = _db.StartTransaction();
            var personTable = _creator(tr);
            return personTable.ListByNationality(Nationality.Slovakia).ToList();;
        }

        [Benchmark]
        public IList<Person> SmallNumberOfValuesListAll()
        {
            using var tr = _db.StartTransaction();
            var personTable = _creator(tr);
            return personTable.ListById().Where(p => p.Nationality == Nationality.Slovakia).ToList();
        }

        [Benchmark]
        public IList<Person> SingleValueListAll()
        {
            using var tr = _db.StartTransaction();
            var personTable = _creator(tr);
            return personTable.ListById()
                .Where(p => p.Nationality == Nationality.Czech).ToList();
        }
        
        [Benchmark]
        public IList<Person> SingleValueUseKey()
        {
            using var tr = _db.StartTransaction();
            var personTable = _creator(tr);
            var persons = personTable.ListByNationality(Nationality.Czech).ToList();
            return persons;
        }
    }

    public class Person
    {
        public Person(ulong id, string name, Nationality nationality)
        {
            Id = id;
            Name = name;
            Nationality = nationality;
        }

        public Person()
        {
            
        }

        [PrimaryKey(1)]
        public ulong Id { get; set; }
        public string Name { get; set; }
        
        [SecondaryKey("Nationality")]
        public Nationality Nationality { get; set; }
        
    }

    public enum Nationality
    {
        Czech, German, Dutch, Slovakia
    }
}