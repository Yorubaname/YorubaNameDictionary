using Core.Dto.Response;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(u => u.Email).EmailAddress();
            RuleFor(u => u.Password).NotEmpty();
            RuleFor(u => u.Username).NotEmpty();

            // TODO Hafiz: Validate role is in valid list of roles.
            RuleFor(u => u.Roles).NotEmpty().WithMessage("No role is selected");
        }
    }
}
