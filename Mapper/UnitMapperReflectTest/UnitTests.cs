using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapperReflect;

namespace UnitMapperReflectTest
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void MapTest()
        {
            IMapper m = AutoMapper.Build(typeof(Student), typeof(Person));
            Student s = new Student {Nr = 27721, Name = "Ze Manel"};
            Person p = (Person) m.Map(s);
            Assert.AreEqual(s.Name, p.Name);
        }

        [TestMethod]
        public void MapCustomAttributeTest()
        {
            IMapper m = AutoMapper.Build(typeof(Student), typeof(Person))
                                  .Bind(Mapping.CustomAttributes);         
            Student s = new Student {ImAField = "ImAField", Nickname = "Zezito", Nr = 27721, Name = "Ze Manel"};
            Person p = (Person) m.Map(s);
            Assert.AreEqual(s.Nickname, p.Nickname);
        }

        [TestMethod]
        public void MapParametersTest()             
        {
            IMapper m = AutoMapper.Build(typeof(Student), typeof(Course));
            Student s = new Student {ImAField = "ImAField", Nickname = "Zezito", Nr = 27721, Name = "Ze Manel", Address = "Rua de Cima"};
            Course c = (Course) m.Map(s);
            Assert.AreEqual(s.Address, c.Address);
        }

        [TestMethod]
        public void MapByBindTest()
        {
            IMapper m = AutoMapper.Build(typeof(Student), typeof(Person))
                                  .Bind(Mapping.Fields);
            Student s = new Student { ImAField = "ImAField", Nickname = "Zezito", Nr = 27721, Name = "Ze Manel" };
            Person p = (Person)m.Map(s);
            Assert.AreEqual(s.ImAField, p.ImAField);
        }

        [TestMethod]
        public void MapByMatchTest()
        {
            int[] array = {27999, 27898, 27162};
            IMapper m = AutoMapper.Build(typeof(Student), typeof(Person))
                                  .Match("Nr","Id")
                                  .Match("Org", "Org");
            Student s = new Student { ImAField = "ImAField", Nickname = "Zezito", Nr = 27721, Name = "Ze Manel",
                                    Org = new School("Lisbon", array, "ISEL" )
                                    };
            Person p = (Person)m.Map(s);
            Assert.AreEqual(s.Nr, p.Id);
            Assert.AreEqual(s.Org.Name, p.Org.Name);
        }

        [TestMethod]
        public void MapArrayTest()
        {
            IMapper m = AutoMapper.Build(typeof(Student), typeof(Person));                       
            Student [] s = { new Student { ImAField = "ImAField", Nickname = "Zezito", Nr = 27721, Name = "Ze Manel" },
                             new Student { ImAField = "ImAField", Nickname = "Xico", Nr = 27722, Name = "Francisco" }
                            };          
            Person []  p = (Person[])m.Map(s);
            for (int i = 0; i < p.Length; ++i)         
                Assert.AreEqual(s[i].Name, p[i].Name);                           
         }

        [TestMethod]
        public void CacheTest()
        {
            IMapper mapper1 = AutoMapper.Build(typeof(Student), typeof(Person));
            IMapper mapper2 = AutoMapper.Build(typeof(Student), typeof(Person));
            Assert.AreEqual(mapper1.GetHashCode(), mapper2.GetHashCode());
        }

        [TestMethod]
        public void ValueTypeTest()
        {
            IMapper m = AutoMapper.Build(typeof(int), typeof(int));
            int x = 1;
            int i = (int) m.Map(x);
            Assert.AreEqual(x,i);
        }
    }
}