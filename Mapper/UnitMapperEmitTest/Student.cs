using System;

namespace MapperEmit
{
    public class Student
    {
        public string Nickname;             //to test Field
        public string ImAField;
        public School Org { get; set; }
        public Course[] Courses { get; set; }
        public string Name { get; set; }
        public int Nr { get; set; }

        public string GenerateString()
        {
            Random rand = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[rand.Next(1, chars.Length)];
            for (int j = 0; j < stringChars.Length; j++)
            {
                stringChars[j] = chars[rand.Next(chars.Length)];
            }
            return new string(stringChars);
        }
    }
}
