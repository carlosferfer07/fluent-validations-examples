namespace FluentValidationExamples.Models
{
    public class Book
    {
        public DateOnly PublicationDate { get; set; }

        public int Pages { get; set; }

        public string? Name { get; set; }
    }
}
