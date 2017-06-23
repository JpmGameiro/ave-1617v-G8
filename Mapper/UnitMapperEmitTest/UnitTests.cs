using System;
using System.Collections.Generic;
using MapperEmit;
using MapperReflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SettlerEmitTest;

namespace UnitMapperEmitTest
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void MapTest()
        {
            IMapperEmit m = AutoMapperEmit.Build(typeof(Student), typeof(Person));
            Student s = new Student { Nr = 27721, Name = "Ze Manel" };
            Person p = (Person)m.Map(s);
            Assert.AreEqual(s.Name, p.Name);
        }

        [TestMethod]
        public void MapCustomAttributeTest()
        {
            IMapperEmit m = AutoMapperEmit.Build(typeof(Student), typeof(Person))
                                  .Bind(MappingEmit.CustomAttributes);
            Student s = new Student { ImAField = "ImAField", Nickname = "Zezito", Nr = 27721, Name = "Ze Manel" };
            Person p = (Person)m.Map(s);
            Assert.AreEqual(s.Nickname, p.Nickname);
        }

        [TestMethod]
        public void MapParametersTest()
        {
            IMapperEmit m = AutoMapperEmit.Build(typeof(Student), typeof(Course));
            Student s = new Student { ImAField = "ImAField", Nickname = "Zezito", Nr = 27721, Name = "Ze Manel", Address = "Rua de Cima"};
            Course c = (Course)m.Map(s);
            Assert.AreEqual(s.Address, c.Address);
        }

        [TestMethod]
        public void MapByBindTest()
        {
            IMapperEmit m = AutoMapperEmit.Build(typeof(Student), typeof(Person))
                                  .Bind(MappingEmit.Fields);
            Student s = new Student { ImAField = "ImAField", Nickname = "Zezito", Nr = 27721, Name = "Ze Manel" };
            Person p = (Person)m.Map(s);
            Assert.AreEqual(s.ImAField, p.ImAField);
        }

        [TestMethod]
        public void MapByMatchTest()
        {
            int[] array = { 27999, 27898, 27162 };
            IMapperEmit m = AutoMapperEmit.Build(typeof(Student), typeof(Person))
                                  .Match("Nr", "Id")
                                  .Match("Org", "Org");
            Student s = new Student
            {
                ImAField = "ImAField",
                Nickname = "Zezito",
                Nr = 27721,
                Name = "Ze Manel",
                Org = new School("Lisbon", array, "ISEL")
            };
            Person p = (Person)m.Map(s);
            Assert.AreEqual(s.Nr, p.Id);
            Assert.AreEqual(s.Org.Name, p.Org.Name);
        }

        [TestMethod]
        public void MapArrayTest()
        {
            IMapperEmit m = AutoMapperEmit.Build(typeof(Student), typeof(Person));
            Student[] s = { new Student { ImAField = "ImAField", Nickname = "Zezito", Nr = 27721, Name = "Ze Manel" },
                             new Student { ImAField = "ImAField", Nickname = "Xico", Nr = 27722, Name = "Francisco" }
                            };
            Person[] p = (Person[])m.Map(s);
            for (int i = 0; i < p.Length; ++i)
                Assert.AreEqual(s[i].Name, p[i].Name);
        }

        [TestMethod]
        public void CacheTest()
        {
            IMapperEmit mapper1 = AutoMapperEmit.Build(typeof(Student), typeof(Person));
            IMapperEmit mapper2 = AutoMapperEmit.Build(typeof(Student), typeof(Person));
            Assert.AreEqual(mapper1.GetHashCode(), mapper2.GetHashCode());
        }

        [TestMethod]
        public void ValueTypeTest()
        {
            IMapperEmit m = AutoMapperEmit.Build(typeof(int), typeof(int));
            int x = 1;
            int i = (int)m.Map(x);
            Assert.AreEqual(x, i);
        }

        [TestMethod]
        public void TestPerformance()
        {
            Action reflectionAction = () => AutoMapper.Build(typeof(Student), typeof(Person));
            Action emitAction = () => AutoMapperEmit.Build(typeof(Student), typeof(Person));
            long reflection = Benchmark.Bench(reflectionAction, "Reflection - Person");
            long emit = Benchmark.Bench(emitAction, "Emit - Person");
            Assert.IsTrue(reflection < emit);
        }

        [TestMethod]
        public void GenericBuildTestForCollectionMap()
        {
            Student[] stds = { new Student{ Nr = 27721, Name = "Ze Manel"},
                                new Student{ Nr = 15642, Name = "Maria Papoila"}};            Mapper<Student,Person> m = (Mapper<Student,Person>)AutoMapperEmit.Build<Student, Person>()
                .Match("Nr", "Id");
            List<Person> ps = m.Map<List<Person>>(stds);
            Person[] p = ps.ToArray();

            for (int i = 0; i < p.Length; ++i)
            {
                Assert.AreEqual(stds[i].Name, p[i].Name);
                Assert.AreEqual(stds[i].Nr, p[i].Id);
            }
        }

        [TestMethod]
        public void GenericBuildTestForMap()
        {
            Student[] stds = { new Student{ Nr = 27721, Name = "Ze Manel"},
                                new Student{ Nr = 15642, Name = "Maria Papoila"}};

            Mapper<Student, Person> m = (Mapper<Student, Person>)AutoMapperEmit.Build<Student, Person>()
                .Match("Nr", "Id");
            Person [] ps = m.Map(stds);

            for (int i = 0; i < ps.Length; ++i)
            {
                Assert.AreEqual(stds[i].Name, ps[i].Name);
                Assert.AreEqual(stds[i].Nr, ps[i].Id);
            }
        }
        

        [TestMethod]
        public void MapLazyTest()
        {
            Student s1 = new Student {Nr = 1234};
            IEnumerable<Student> stds = new List<Student>()
            {
                s1,
                new Student {Nr = 3121}
            };   
            Mapper<Student, Person> m = (Mapper<Student, Person>)AutoMapperEmit.Build<Student, Person>()
                .Match("Nr", "Id");
            using (IEnumerator<Student> st = stds.GetEnumerator())
            {
                using (IEnumerator<Person> prsn = m.MapLazy(stds).GetEnumerator())
                {
                    s1.Nr = 36;
                    while (st.MoveNext() && prsn.MoveNext())
                    {
                        Assert.AreEqual(st.Current.Nr, prsn.Current.Id);
                    }
                }
            }
        }

        [TestMethod]
        public void ForMethodTest()
        {
            Student std = new Student ();
            Mapper<Student, Person> m = AutoMapperEmit.Build<Student, Person>().For("Name", () => "João");
            Person p = (Person)m.Map(std);
            Assert.AreEqual(std.Name, p.Name);
        }
    }
}