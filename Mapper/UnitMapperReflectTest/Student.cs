namespace MapperReflect
{
    public class Student
    {
        public string Nickname;           
        public string ImAField;
        public School Org { get; set; }
        public Course[] Courses { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public int Nr { get; set; }
    }
}
