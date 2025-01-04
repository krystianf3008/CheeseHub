using CheeseHub.Interfaces.Services;
using CheeseHub.Models.Video;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using CheeseHub.Extensions;
using CheeseHub.Models.Video.DTOs;
using CheeseHub.Models.User;
using CheeseHub.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using CheeseHub.Models.SharedDtos;
using CheeseHub.Enums;
using CheeseHub.Models.Video.Validators;
using System.ComponentModel.DataAnnotations;
using CheeseHub.Models.Category;
using CheeseHub.Models.Shared;


namespace CheeseHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]


    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;
        private readonly IVideoViewService _videoViewService;
        private readonly IVideoReactionService _videoReactionService;
        private readonly ICommentReactionService _commentReactionService;
        private readonly ICategoryService _categoryService;

        public VideoController(IVideoService videoService, IWebHostEnvironment environment, IUserService userService, IVideoViewService videoViewService, IVideoReactionService videoReactionService, ICommentReactionService commentReactionService, ICategoryService categoryService)
        {
            _videoService = videoService;
            _environment = environment;
            _userService = userService;
            _videoViewService = videoViewService;
            _videoReactionService = videoReactionService;
            _commentReactionService = commentReactionService;
            _categoryService = categoryService;
        }
        [HttpGet("{id}", Name = "GetVideo")]
        public async Task<IActionResult> Get([FromBody] Guid Id)
        {

            Video video = await _videoService.GetById(Id);
            if (video == null)
            {
                return NotFound("Video is null");
            }

            return Ok(video);
        }
        [HttpGet(Name = "GetVideos")]
        public async Task<IActionResult> GetVideos([FromQuery] int page , [FromQuery] int pageSize, [FromQuery] string search = null, [FromQuery] string? categoryId = null, [FromQuery] bool? my = null, [FromQuery] bool? liked = null)
        {
            Guid? userId = null;
            bool isAdmin = User.IsInRole("Admin");
            if(my == true)
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    userId = Guid.Parse((User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()).Value);
                }
                else
                {
                    return Unauthorized();
                }
            }
            
            IQueryable<VideoListDTO> videos = await _videoService.GetVidosWithPagination(search : search,pageSize : pageSize, page : page, categoryId: categoryId, userId:userId, liked : liked, admin: isAdmin);

            return Ok(videos);
        }
        [HttpGet("GetStream/{id}", Name = "GetStream")]
        public async Task<IActionResult> GetStream(Guid Id)
        {

            Video video = await _videoService.GetById(Id);
            if (video == null)
            {
                return NotFound("Video is null");
            }
            var fileInfo = new FileInfo(video.Path);

            var totalLength = fileInfo.Length;
            var rangeHeader = Request.Headers["Range"].ToString();
            long start = 0;
            long end = totalLength - 1;

            if (!string.IsNullOrEmpty(rangeHeader) && rangeHeader.StartsWith("bytes="))
            {
                var ranges = rangeHeader.Substring("bytes=".Length).Split('-');
                if (ranges.Length == 2)
                {
                    if (!string.IsNullOrEmpty(ranges[0]) && long.TryParse(ranges[0], out var rangeStart))
                    {
                        start = rangeStart;
                    }

                    if (!string.IsNullOrEmpty(ranges[1]) && long.TryParse(ranges[1], out var rangeEnd))
                    {
                        end = rangeEnd;
                    }
                }

                // Validate the range
                if (start >= totalLength || end >= totalLength || start > end)
                {
                    return BadRequest("Invalid Range");
                }

                Response.StatusCode = StatusCodes.Status206PartialContent;
                Response.Headers.ContentRange = new ContentRangeHeaderValue(start, end, totalLength).ToString();
            }
            else
            {
                Response.StatusCode = StatusCodes.Status200OK;
            }

            // Add Accept-Ranges header
            Response.Headers.AcceptRanges = "bytes";

            // Return the file stream with range support
            return File(
                new FileStream(video.Path, FileMode.Open, FileAccess.Read, FileShare.Read),
                 MimeType.GetMimeType(fileInfo.Extension),
                fileInfo.Name,
                enableRangeProcessing: true
            );
        }
        [HttpGet("GetImage/{id}", Name = "GetImage")]
        public async Task<IActionResult> GetImage(Guid Id)
        {

            Video video = await _videoService.GetById(Id);
            if (video == null)
            {
                return NotFound("Video is null");
            }
            var fileInfo = new FileInfo(video.ImagePath);
            return File(
                new FileStream(video.ImagePath, FileMode.Open, FileAccess.Read, FileShare.Read),
                 MimeType.GetMimeType(fileInfo.Extension),
                fileInfo.Name
            );
        }

        [HttpGet("All", Name = "GetAll")]
        public async Task<IActionResult> GetAll()
        {
            List<Video> video = await _videoService.GetAll();
            if (video == null)
            {
                return NotFound("Video is null");
            }

            return Ok(video);
        }
        [HttpGet("Watch/{id}", Name = "Watch")]
        public async Task<IActionResult> GetWatch(Guid Id)
        {
            Guid? userId = null;
            bool isAdmin = User.IsInRole("Admin");

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                 userId = Guid.Parse((User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()).Value);
            }
            try
            {
                SingleVideoDTO? singleVideoDTO = await _videoService.GetSingleVideo(Id, userId, admin: isAdmin);
                if (singleVideoDTO == null)
                {
                    return NotFound();
                }
                await _videoViewService.AddView(Id, userId);

                return Ok(singleVideoDTO);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized();
            }
           
        }

        [HttpPost]
        [Authorize]
        //TODO move logic to service
        public  async Task<IActionResult> Post([FromForm] CreateVideoDTO video)
        {
            CreateVideoDTOValidator validator = new CreateVideoDTOValidator(_categoryService);
            var result =  await validator.ValidateAsync(video);
            if (! result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            if (video == null || video.File == null)
            {
                return BadRequest("Invalid video data");
            }

            Guid videoId = Guid.NewGuid();

            string uploadsFolder = Path.Combine(_environment.ContentRootPath, "UploadedVideos");
            string coversUploadsFolder = Path.Combine(_environment.ContentRootPath, "UploadedVideosCovers");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            if (!Directory.Exists(coversUploadsFolder))
            {
                Directory.CreateDirectory(coversUploadsFolder);
            }
            string fileName = $"{videoId}{Path.GetExtension(video.File.FileName)}";
            string filePath = Path.Combine(uploadsFolder, fileName);
            string coverName = $"{videoId}{Path.GetExtension(video.Image.FileName)}";
            string coverPath = Path.Combine(coversUploadsFolder, coverName);
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    video.File.CopyTo(stream);
                }
                using (FileStream stream = new FileStream(coverPath, FileMode.Create))
                {
                    video.Image.CopyTo(stream);
                }
                User? user = await _userService.GetWithRole(id: Guid.Parse((User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()).Value));
                if (user == null)
                    {
                    return BadRequest("User not found");
                }
                Video newVideo = new Video
                {
                    Id = videoId,
                    Name = video.Name,
                    Description = video.Description,
                    UserId = user.Id,
                    Path = filePath,
                    ImagePath = coverPath,
                    CreatedAt = DateTime.Now,
                    Status = (char)Status.New,
                    CategoryId = Guid.Parse( video.CategoryId),
                };

                await _videoService.Add(newVideo);

                return CreatedAtRoute(
                    "GetVideo",
                    new { Id = newVideo.Id },
                    newVideo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("ToggleVideoReaction", Name = "ToggleVideoReaction")]

        [Authorize]
        public async Task<IActionResult> ToggleVideoReaction(ToggleReactionDTO model)
        {
            try
            {
                Guid userId = Guid.Parse((User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()).Value);

                await _videoReactionService.ToggleReaction(userId, model.TargetId, (ToggleReactionEnum)model.ToggleType);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [Authorize(Roles="Admin")]
        [HttpPost("Ban", Name = "BanVideo")]
        public async Task<IActionResult> Ban(ModelId model)
        {

            try
            {
                await _videoService.Ban(Guid.Parse(model.Id));

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized();
            }

            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("Unban", Name = "UnbanVideo")]
        public async Task<IActionResult> Unban([FromBody] ModelId model)
        {

            try
            {
                await _videoService.Unban(Guid.Parse(model.Id));

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized();
            }

            return Ok();
        }
        [Authorize]
        [HttpPost("Show", Name = "ShowVideo")]
        public async Task<IActionResult> Show([FromBody] ModelId model)
        {

            try
            {
                Guid userId = Guid.Parse((User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()).Value);

                await _videoService.Show(Guid.Parse(model.Id), userId);

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized();
            }

            return Ok();
        }
        [Authorize]
        [HttpPost("Hide", Name = "HideVideo")]
        public async Task<IActionResult> Hide([FromBody] ModelId model)
        {

            try
            {
                Guid userId = Guid.Parse((User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()).Value);

                await _videoService.Hide(Guid.Parse(model.Id),userId);

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized();
            }

            return Ok();
        }


    }
}
