using System;
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
        public void MapParametersTest()             //comentar construtor sem parametros de Person
        {
            IMapperEmit m = AutoMapperEmit.Build(typeof(Student), typeof(Person));
            Student s = new Student { ImAField = "ImAField", Nickname = "Zezito", Nr = 27721, Name = "Ze Manel" };
            Person p = (Person)m.Map(s);
            Assert.AreEqual(s.ImAField, p.ImAField);
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
            IMapperEmit m = AutoMapperEmit.Build(typeof(Student), typeof(Person));
            Student s = new Student { Nr = 27721, Name = "Ze Manel" };
            Person p = (Person)m.Map(s);
            Person p1 = (Person)m.Map(s);
            Assert.AreEqual(s.Name, p1.Name);
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
    }
}