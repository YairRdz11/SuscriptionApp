namespace SuscriptionApp.DTOs
{
    public class KeyDTO
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public bool Enable { get; set; }
        public string KeyType { get; set; }
        public List<DomainRestrictionDTO> DomainRestrictions { get; set; }
    }
}
