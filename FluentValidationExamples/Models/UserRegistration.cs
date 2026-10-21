namespace FluentValidationExamples.Models
{
    public class UserRegistration
    {
        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

        public DateOnly BirthDate { get; set; }

        public bool WantsNewsletter { get; set; }

        public string? NewsletterFrequency { get; set; }
    }
}
