using System.Collections.Generic;
using System.Linq;
using BTDB.ODBLayer;

namespace Benchmarks
{
    public class Person
    {
        public Person(ulong id, string name, Country country, int childLevel)
        {
            Id = id;
            Name = name;
            Country = country;

            if (childLevel < 3)
            {
                foreach (var i in Enumerable.Range(0, 10))
                {
                    Childs.Add(new Child(name + $"-child{childLevel}", childLevel + 1));
                }
            }
        }

        public Person() {}

        [PrimaryKey(1)]
        public ulong Id { get; set; }
        public string Name { get; set; }
        
        [SecondaryKey("Country")]
        public Country Country { get; set; }
        
        public IList<Child> Childs = new List<Child>();
    }
    
    public class Child
    {
        public Child(string name, int childLevel)
        {
            Name = name;

            if (childLevel < 3)
            {
                foreach (var i in Enumerable.Range(0, 10))
                {
                    Childs.Add(new Child( name + $"-child{childLevel}", childLevel + 1));
                }
            }
        }

        public Child() {}
        public string Name { get; set; }
        
        public IList<Child> Childs = new List<Child>();
    }
}