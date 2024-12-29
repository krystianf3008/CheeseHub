using CheeseHub.Models.Comment.DTOs;
using CheeseHub.Models.Video.DTOs;
using FluentValidation;

namespace CheeseHub.Models.Comment.Validators
{
    public class CreateCommentDTOValidator : AbstractValidator<CreateCommentDTO>
    {

        public CreateCommentDTOValidator()
        {
            RuleFor(x => x.Content).NotEmpty().MaximumLength(500);
        }
    }
}
