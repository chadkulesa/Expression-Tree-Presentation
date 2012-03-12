using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System.Reflection;
using TCCC12.LinqFilter;

namespace TestTCCC12.LinqFilter
{
    [TestClass]
    public class LinqFilterBuilderTest
    {
        [TestMethod]
        public void Simple1()
        {
            var people = from person in _people
                         where person.Name == "Bill"
                         select person;
            PrintResults(people);
        }

        [TestMethod]
        public void Simple2()
        {
            var people = _people.Where(x => x.Name == "Bill");                         
            PrintResults(people);
        }

        [TestMethod]
        public void Simple3()
        {
            Expression<Func<Person, bool>> filter = x => x.Name == "Bill";
            var people = _people.Where(filter);
            PrintResults(people);
        }

        [TestMethod]
        public void Simple4()
        {
            var x = Expression.Parameter(typeof (Person), "x");
            Expression<Func<Person, bool>> filter = Expression.Lambda<Func<Person, bool>>(
                body: Expression.Equal(
                    left: Expression.MakeMemberAccess(
                        expression: x,
                        member: typeof (Person).GetProperty("Name")),
                    right: Expression.Constant("Bill")),
                parameters: x);

            var people = _people.Where(filter);
            PrintResults(people);
        }

        [TestMethod]
        public void Builder1()
        {
            var filter = LinqFilterBuilder.CreateFalseLambda<int>();
            filter = filter.Or(x => x == 5);
            filter = filter.Or(x => x == 10);

            var filterMethod = filter.Compile();

            Assert.IsTrue(filterMethod(5));
            Assert.IsTrue(filterMethod(10));
            Assert.IsFalse(filterMethod(1));            
        }

        [TestMethod]
        public void Builder2()
        {
            var filter = LinqFilterBuilder.CreateFalseLambda<int>();
            filter = filter.Or(bob => bob == 5);
            filter = filter.Or(whatever => whatever == 10);

            var filterMethod = filter.Compile();

            Assert.IsTrue(filterMethod(5));
            Assert.IsTrue(filterMethod(10));
            Assert.IsFalse(filterMethod(1));
        }

        [TestMethod]
        public void Builder3()
        {
            var filter = LinqFilterBuilder.CreateFalseLambda<Person>();
            filter = filter.Or(x => x.Name == "Bill");
            filter = filter.Or(x => x.Birthday.Month == 5);

            var people = _people.Where(filter);
            PrintResults(people);
        }

        private void PrintResults(IEnumerable<Person> people)
        {
            foreach (var person in people)
            {
                Console.WriteLine(person.Name);
            }
        }
        [TestInitialize]
        public void TestInitialize()
        {
            var people = new List<Person>()
                             {
                                 new Person()
                                     {
                                         Name = "Bill",
                                         Birthday = new DateTime(1962, 2, 11)
                                     },
                                 new Person()
                                     {
                                         Name = "Bob",
                                         Birthday = new DateTime(1938, 11, 25)
                                     },
                                 new Person()
                                     {
                                         Name = "Jane",
                                         Birthday = new DateTime(2001, 5, 23)
                                     },
                                 new Person()
                                     {
                                         Name = "Mary",
                                         Birthday = new DateTime(2011, 2, 4)
                                     }
                             };
            _people = people.AsQueryable();
        }

        private IQueryable<Person> _people;
 
        private class Person
        {
            public string Name { get; set; }
            public DateTime Birthday { get; set; }
        }
    }
}
