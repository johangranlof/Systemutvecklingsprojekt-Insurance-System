namespace Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int ZoneId { get; set; }
        public Zone? Zone { get; set; }
    }
}
