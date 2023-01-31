namespace SuscriptionApp.Entities
{
    public class DomainRestriction
    {
        public int Id { get; set; }
        public string Domain { get; set; }
        public int KeyId { get; set; }
        public KeyAPI Key { get; set; }
    }
}
