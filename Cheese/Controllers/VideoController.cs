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

namespace CheeseHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]


    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;

        public VideoController(IVideoService videoService, IWebHostEnvironment environment, IUserService userService, )
        {
            _videoService = videoService;
            _environment = environment;
            _userService = userService; 
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
        public async Task<IActionResult> GetVideos([FromQuery] int page , [FromQuery] int pageSize, [FromQuery] string search = null)
        {
            IQueryable<VideoListDTO> videos = await _videoService.GetVidosWithPagination(search : search,pageSize : pageSize, page : page);

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
        [HttpPost]
        [Authorize]
        public  async Task<IActionResult> Post([FromForm] CreateVideoDTO video)
        {
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
                    ImagePath = coverPath
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
    }
}
