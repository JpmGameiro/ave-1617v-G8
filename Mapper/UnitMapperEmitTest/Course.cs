using System;

namespace MapperEmit
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