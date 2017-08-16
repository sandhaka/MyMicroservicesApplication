using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Orders.Application.Commands;
using Orders.Application.Controllers;

namespace Orders.Application.Validation
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(c => c.OrderItems).Must(ContainOrderItems).WithMessage("No order items found");
            RuleFor(c => c.CardNumber).NotEmpty().Length(12, 19);
            RuleFor(c => c.CardHolderName).NotEmpty();
            RuleFor(c => c.CardExpiration).NotEmpty().Must(BeValidExpirationDate)
                .WithMessage("Please specify a valid card expiration date");
            RuleFor(c => c.CardSecurityNumber).NotEmpty().Length(3);
        }
        
        private bool BeValidExpirationDate(DateTime dateTime)
        {
            return dateTime >= DateTime.UtcNow;
        }
        
        private bool ContainOrderItems(IEnumerable<OrderItemDto> orderItems)
        {
            return orderItems.Any();
        }
    }
}