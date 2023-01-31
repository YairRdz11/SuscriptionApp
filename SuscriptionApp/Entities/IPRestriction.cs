namespace SuscriptionApp.Entities
{
    public class IPRestriction
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public int KeyId { get; set; }
        public KeyAPI Key { get; set; }
    }
}
