namespace QHH.Data.Models
{
    public class FAQ
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public ulong OwnerId { get; set; }
    }
}
