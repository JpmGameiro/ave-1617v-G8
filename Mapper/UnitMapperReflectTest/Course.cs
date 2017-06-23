using System;

namespace MapperReflect
{
    public class Course
    {
        public string Address { get; set; }

        public Course(string address)
        {
            this.Address = address;
        }
    }
}