using Azure.Core;
using CheeseHub.Data;
using CheeseHub.Interfaces.Services;
using CheeseHub.Models.User;
using CheeseHub.Models.Video;
using CheeseHub.Models.Video.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Policy;

namespace CheeseHub.Services
{
    public class VideoService :  BaseService<Video>, IVideoService
    {
        ApplicationDbContext ApplicationDbContext { get; set; }


        public VideoService(ApplicationDbContext context) : base(context)
        {
            ApplicationDbContext = context; 
        }
        public async Task WriteToStream(VideoStreamModelDTO model, Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                var buffer = new byte[65536];
                using (var video = model.FileInfo.OpenRead())
                {
                    if (model.End == -1)
                    {
                        model.End = video.Length;
                    }
                    var position = model.Start;
                    var bytesLeft = model.End - model.Start + 1;
                    video.Position = model.Start;
                    while (position <= model.End)
                    {
                        var bytesRead = video.Read(buffer, 0, (int)Math.Min(bytesLeft, buffer.Length));
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        position += bytesRead;
                        bytesLeft = model.End - position + 1;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                outputStream.Close();
            }
        }
        public async Task<IQueryable< VideoListDTO>> GetVidosWithPagination(string search = null, int pageSize = 10, int page = 0)
        {
            IQueryable<Video> query = ApplicationDbContext.Videos.Include(x => x.User);
            if(!string.IsNullOrEmpty(search) )
            {
                query = query.Where(v => v.Name.ToLower().Contains(search.ToLower()));
            }

            int totalCount = await query.CountAsync();
            query= query
            .Skip(page * pageSize)
            .Take(pageSize);

            return query.Select(x => new VideoListDTO
            {
                Name = x.Name,
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.User.Name,
                Description = x.Description,
                ImagePath = "",
                CreatedAt = x.CreatedAt,
                Path = x.Path,

            });
        }
        public async Task<SingleVideoDTO> GetSingleVideo(Guid id)
        {
            IQueryable<Video> query = ApplicationDbContext.Videos.Include(x => x.User);
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(v => v.Name.ToLower().Contains(search.ToLower()));
            }

            int totalCount = await query.CountAsync();
            query = query
            .Skip(page * pageSize)
            .Take(pageSize);

            return query.Select(x => new VideoListDTO
            {
                Name = x.Name,
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.User.Name,
                Description = x.Description,
                ImagePath = "",
                CreatedAt = x.CreatedAt,
                Path = x.Path,

            });
        }

    }
}
