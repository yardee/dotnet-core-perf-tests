using BTDB.ODBLayer;

namespace Benchmarks
{
    public class Person
    {
        public Person(ulong id, string name, Country country)
        {
            Id = id;
            Name = name;
            Country = country;
        }

        public Person() {}

        [PrimaryKey(1)]
        public ulong Id { get; set; }
        public string Name { get; set; }
        
        [SecondaryKey("Country")]
        public Country Country { get; set; }
        
    }
}