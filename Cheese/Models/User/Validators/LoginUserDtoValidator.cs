using CheeseHub.Interfaces.Services;
using CheeseHub.Models.User.DTOs;
using FluentValidation;

namespace CheeseHub.Models.User.Validators
{
    public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserDtoValidator(IUserService userService)
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();

        }
    }
}
