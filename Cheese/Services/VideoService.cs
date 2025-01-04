using Azure.Core;
using CheeseHub.Data;
using CheeseHub.Enums;
using CheeseHub.Interfaces.Services;
using CheeseHub.Models.User;
using CheeseHub.Models.Video;
using CheeseHub.Models.Video.DTOs;
using CheeseHub.Models.VideoReaction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IQueryable< VideoListDTO>> GetVidosWithPagination(string search = null, int pageSize = 10, int page = 0, string categoryId = null, Guid? userId = null,bool? liked = null, bool admin = false)
        {
            char bannedStatus = (char)Status.Banned;
            char hiddenStatus = (char)Status.Hidden;

            IQueryable<Video> query = ApplicationDbContext.Videos.Include(x => x.User).Include(x => x.Reactions).Include(x => x.Views);
            if(!admin)
            {
                query = query.Where(v => v.Status != bannedStatus);
            }
            if(userId != null)
            {
                if(liked == true)
                {
                    query = query.Include(q => q.Reactions).Where( q=> q.Reactions.Any(r => r.UserId == userId && r.IsLike) );
                }
                query = query.Where(v => v.UserId == userId);
            }
            else
            {
                if(!admin)
                {
                    query = query.Where(v => v.Status != hiddenStatus);

                }
            }

            if (!string.IsNullOrEmpty(search) )
            {
                query = query.Where(v => v.Name.ToLower().Contains(search.ToLower()));
            }
            if(!string.IsNullOrEmpty(categoryId) )
            {
                Guid categoryGuid = Guid.Parse(categoryId);
                query = query.Where(v => v.CategoryId == categoryGuid);

            }

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
                Status = x.Status,
                TotalDisLikes = x.Reactions.Where(r => !r.IsLike).Count(),
                TotalLikes = x.Reactions.Where(r => r.IsLike).Count(),
                TotalViews = x.Views.Count(),

            }).OrderByDescending(x => x.CreatedAt);
        }
        public async Task<SingleVideoDTO?> GetSingleVideo(Guid id,Guid? userId, bool admin = false)
        {
            Video model= await ApplicationDbContext.Videos.Include(x => x.Views).Include(x => x.User).FirstOrDefaultAsync(v => v.Id == id );
            if(model == null)
            {
                return null;

            }
            if(model.Status == (char)Status.Banned && !admin)
            {
                throw new UnauthorizedAccessException();
            }
            IQueryable<VideoReaction> reactionsQuery =  ApplicationDbContext.VideoReactions.Where(x => x.TargetId == id);
            int totalLikes = await reactionsQuery.Where(x => x.IsLike).CountAsync();
            int totalDisLikes = await reactionsQuery.Where(x => !x.IsLike).CountAsync();
            int totalViews = await ApplicationDbContext.VideoViews.Where(x => x.VideoId == id).CountAsync();
            SingleVideoDTO singleVideoDTO = new SingleVideoDTO
            {
                CreatedAt = model.CreatedAt,
                Description = model.Description,
                Name = model.Name,
                Id = model.Id,
                UserId = model.UserId,
                UserName = model.User.Name,
                TotalLikes = totalLikes,
                TotalDisLikes = totalDisLikes,
                TotalViews = totalViews,
                Status = model.Status

            };
            if (userId != null)
            {
                VideoReaction? videoReaction = await ApplicationDbContext.VideoReactions.FirstOrDefaultAsync(v => v.TargetId == id && v.UserId == userId);
                if(videoReaction != null)
                {
                    singleVideoDTO.IsLiked = videoReaction.IsLike;
                    singleVideoDTO.IsDisLiked = !videoReaction.IsLike;
                }
                if(model.Status == (char)Status.Hidden )
                {
                    if (model.UserId == userId) 
                    {
                        return singleVideoDTO;
                    }
                }
            }
            if(model.Status == (char)Status.Hidden)
            {
                throw new UnauthorizedAccessException();
            }
            return singleVideoDTO;


        }
        
        public async Task Ban(Guid id)
        {
            Video model = await ApplicationDbContext.Videos.Include(x => x.Views).Include(x => x.User).FirstOrDefaultAsync(v => v.Id == id);
            if (model == null)
            {
                throw new Exception("Not found");
            }
            model.Status = (char)Status.Banned;
            ApplicationDbContext.Update(model);
            ApplicationDbContext.SaveChanges();
        }
        public async Task Unban(Guid id)
        {
            Video model = await ApplicationDbContext.Videos.Include(x => x.Views).Include(x => x.User).FirstOrDefaultAsync(v => v.Id == id);
            if (model == null)  
            {
                throw new Exception("Not found");
            }
            model.Status = (char)Status.New;
            ApplicationDbContext.Update(model);
            ApplicationDbContext.SaveChanges();
        }
        public async Task Hide(Guid id,Guid UserId)
        {
            Video model = await ApplicationDbContext.Videos.Include(x => x.Views).Include(x => x.User).FirstOrDefaultAsync(v => v.Id == id);
            if (model == null)
            {
                throw new Exception("Not found");
            }
            if (model.UserId != UserId)
            {
                throw new UnauthorizedAccessException();
            }
            model.Status = (char)Status.Hidden;
            ApplicationDbContext.Update(model);
            ApplicationDbContext.SaveChanges();
        }
        public async Task Show(Guid id, Guid UserId)
        {
            Video model = await ApplicationDbContext.Videos.Include(x => x.Views).Include(x => x.User).FirstOrDefaultAsync(v => v.Id == id);
            if (model == null)
            {
                throw new Exception("Not found");
            }
            if (model.UserId != UserId)
            {
                throw new UnauthorizedAccessException();
            }
            model.Status = (char)Status.Banned;
            ApplicationDbContext.Update(model);
            ApplicationDbContext.SaveChanges();

        }
    }
}
