namespace MapperEmit
{
    public class Person
    {
        public string ImAField;
        public Person () { }                        //comentar para testar -> MapParametersTest()
        public Person(string IMAFIeld)
        {
            this.ImAField = IMAFIeld;
        }
        [ToMapAttribute]
        public string Nickname;                     //to test Field
        public Organization Org { get; set; }       
        public int Id { get; set; }
        public string Name { get; set; }
        public Subject [] Subjects { get; set; }
    }
}
