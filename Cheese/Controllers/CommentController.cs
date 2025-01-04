﻿using CheeseHub.Enums;
using CheeseHub.Interfaces.Services;
using CheeseHub.Models.Comment.DTOs;
using CheeseHub.Models.SharedDtos;
using CheeseHub.Models.Video;
using CheeseHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheeseHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ICommentReactionService _commentReactionService;
        private readonly IWebHostEnvironment _environment;
        private readonly IJwtService _jwtService;

        public CommentController(ICommentService commentService , ICommentReactionService commentReactionService)
        {
            _commentService = commentService;
            _commentReactionService = commentReactionService;
        }
        [HttpGet("{VideoId}", Name = "GetComments")]

        public async Task<IActionResult> Get(Guid VideoId)
        {
            Guid? userId = null;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = Guid.Parse((User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()).Value);
            }
            IQueryable<GetCommentDTO> comments = await _commentService.GetAllForVideo(VideoId, userId);
            

            return Ok(comments);
        }
        [HttpPost("", Name = "CreateComment")]

        [Authorize]
        public async Task<IActionResult> Post(CreateCommentDTO model)
        {
            try
            {
                Guid userId = Guid.Parse((User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()).Value);

               GetCommentDTO comment  = await _commentService.Add(model,userId);
                return Ok(
                   new  { response = comment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpDelete("{Id}", Name = "DeleteComment")]

        [Authorize]
        public async Task<IActionResult> Delete(Guid Id)
        {
            try
            {
                Guid userId = Guid.Parse((User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()).Value);
                try
                {
                    await _commentService.Delete(userId,Id);

                }
                catch (UnauthorizedAccessException ex) 
                {
                    return Unauthorized();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpPost("ToggleCommentReaction", Name = "ToggleCommentReaction")]

        [Authorize]
        public async Task<IActionResult> ToggleCommentReaction(ToggleReactionDTO model)
        {
            try
            {
                Guid userId = Guid.Parse((User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()).Value);

                _commentReactionService.ToggleReaction(userId, model.TargetId, (ToggleReactionEnum)model.ToggleType);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
    }
}
