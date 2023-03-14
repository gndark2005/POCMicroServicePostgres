namespace BuildingLink.ModuleServiceTemplate.Events
{
    public class BookCreated
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Status { get; set; }
    }
}