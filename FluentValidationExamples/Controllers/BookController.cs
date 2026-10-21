using FluentValidation;
using FluentValidationExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace FluentValidationExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "The Bee Sting", "Biography of X", "Frank and Bert", "Yellow Face", "1984", "Frankeinstane"
        };

        private readonly ILogger<BookController> _logger;
        private readonly IValidator<Book> _validator;

        public BookController(ILogger<BookController> logger, IValidator<Book> validator)
        {
            _logger = logger;
            _validator = validator;
        }

        [HttpGet(Name = "Book")]
        public IEnumerable<Book> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new Book
            {
                PublicationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Pages = Random.Shared.Next(1, 500),
                Name = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost(Name = "Book")]
        public async Task<ActionResult<Book>> Create(Book book)
        {
            var result = await _validator.ValidateAsync(book);
            if (!result.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(result.ToDictionary()));
            }

            return book;
        }

        [HttpPut(Name = "Book")]
        public async Task<ActionResult<Book>> Update(Book book)
        {
            var result = await _validator.ValidateAsync(book);
            if (!result.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(result.ToDictionary()));
            }

            return book;
        }
        
    }
}
