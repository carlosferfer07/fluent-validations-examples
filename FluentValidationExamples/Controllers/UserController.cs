using FluentValidation;
using FluentValidationExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace FluentValidationExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IValidator<UserRegistration> _validator;

        public UserController(IValidator<UserRegistration> validator)
        {
            _validator = validator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistration registration)
        {
            var result = await _validator.ValidateAsync(registration);
            if (!result.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(result.ToDictionary()));
            }

            return Ok(new { registration.Username, registration.Email });
        }
    }
}
