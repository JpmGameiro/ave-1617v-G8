namespace MapperEmit
{
    public class School
    {
        public string Location { get; set; }
        public int [] MembersIds { get; set; }
        public string Name { get; set; }

        public School(string Location, int[] MemberIds, string Name)
        {
            this.Location = Location;
            this.MembersIds = MemberIds;
            this.Name = Name;
        }
    }
}
