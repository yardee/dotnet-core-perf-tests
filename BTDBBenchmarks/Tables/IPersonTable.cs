using System.Collections.Generic;

namespace Benchmarks.Tables
{
    public interface IPersonTable
    {
        void Insert(Person person);
        IEnumerable<Person> ListById();
        IEnumerable<Person> ListByCountry(Country country);
        Person FindById(ulong id);
    }
}