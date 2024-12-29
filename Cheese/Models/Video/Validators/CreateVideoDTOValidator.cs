using CheeseHub.Models.Video.DTOs;
using FluentValidation;

namespace CheeseHub.Models.Video.Validators
{
    public class CreateVideoDTOValidator : AbstractValidator<CreateVideoDTO>
    {

        private readonly string[] _allowedVideoMimeTypes =
        {
            "video/mp4",
            "video/x-matroska",
            "video/quicktime",
            "video/x-msvideo" 
        };
        public CreateVideoDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(4000);
            RuleFor(x => x.File).Must(x => _allowedVideoMimeTypes.Contains(x.ContentType)).WithMessage("Invalid file format");
        }
    }
}
