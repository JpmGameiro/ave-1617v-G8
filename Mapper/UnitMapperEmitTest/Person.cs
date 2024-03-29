﻿using System;

namespace MapperEmit
{
    public class Person
    {
        public string ImAField;
        public Person () { }                        
        public Person(string IMAFIeld)
        {
            this.ImAField = IMAFIeld;
        }
        [ToMapAttribute]
        public string Nickname;                     
        public Organization Org { get; set; }       
        public int Id { get; set; }
        public string Name { get; set; }
        public Subject [] Subjects { get; set; }
    }
}
