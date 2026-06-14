using FluentValidation;
using AbuAshraf.Application.DTOs;

namespace AbuAshraf.Application.Validators
{
    /// <summary>
    /// Validator for user login
    /// </summary>
    public class LoginUserValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.EmailOrPhone)
                .NotEmpty().WithMessage("Email or phone number is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .Length(8, 255).WithMessage("Password must be at least 8 characters");
        }
    }
}
