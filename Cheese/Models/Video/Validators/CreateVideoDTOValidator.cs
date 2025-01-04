using CheeseHub.Interfaces.Services;
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
        private readonly string[] _allowedImageMimeTypes =
        {
            "image/jpeg",   
            "image/png",    
            "image/webp",   
        };
        //TODO - lepsza walidacja typów plików, może liczby kontrolne
        public CreateVideoDTOValidator(ICategoryService categoryService)
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(4000);
            RuleFor(x => x.File).Must(x => _allowedVideoMimeTypes.Contains(x.ContentType)).WithMessage("Invalid file format. Supported: MKV, MP4, AVI");
            RuleFor(x => x.File)
                .Must(x => x.Length <= 100 * 1024 * 1024) // 100 MB
                .WithMessage("Video file size must be less than 100 MB");
            RuleFor(x => x.Image).Must(x => _allowedImageMimeTypes.Contains(x.ContentType)).WithMessage("Invalid file format Supported: JPEG, PNG,WebP");
            RuleFor(x => x.Image)
               .Must(x => x.Length <= 5 * 1024 * 1024) // 5 MB
               .WithMessage("Image file size must be less than 5 MB");
            RuleFor(x => x.CategoryId).MustAsync(async (x, cancellation) => x != null &&  await categoryService.GetById( Guid.Parse(x)) != null).WithMessage("Category not exists");


        }
    }
}
