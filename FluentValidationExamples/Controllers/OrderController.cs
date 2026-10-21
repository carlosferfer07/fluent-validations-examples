using FluentValidation;
using FluentValidationExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace FluentValidationExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IValidator<Order> _validator;

        public OrderController(IValidator<Order> validator)
        {
            _validator = validator;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Create(Order order)
        {
            var result = await _validator.ValidateAsync(order);
            if (!result.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(result.ToDictionary()));
            }

            return order;
        }
    }
}
