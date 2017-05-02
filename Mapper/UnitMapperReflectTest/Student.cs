namespace MapperReflect
{
    public class Student
    {
        public string Nickname;             //to test Field
        public string ImAField;
        public School Org { get; set; }
        public Course[] Courses { get; set; }
        public string Name { get; set; }
        public int Nr { get; set; }
    }
}
