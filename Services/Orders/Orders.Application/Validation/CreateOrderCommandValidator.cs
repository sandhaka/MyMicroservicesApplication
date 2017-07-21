using FluentValidation;
using Orders.Application.Commands;

namespace Orders.Application.Validation
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(c => c.OrderItems).NotNull().NotEmpty();
        }
    }
}