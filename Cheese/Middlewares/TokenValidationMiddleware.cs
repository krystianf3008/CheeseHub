using CheeseHub.Data;
using CheeseHub.Models.RefreshToken;
using CheeseHub.Models.User;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CheeseHub.Middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next, ILogger<TokenValidationMiddleware> logger)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ApplicationDbContext dbContext)
        {
                if (context.User.Identity?.IsAuthenticated == true)
            {
                Guid userId = new Guid();
                try
                {
                     userId = Guid.Parse((context.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()).Value);

                }
                catch
                {
                    await _next(context);

                }

                User? user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

                if (user != null && user.IsTokenValid == false)
                {
                    IQueryable<RefreshToken> userTokens = dbContext.RefreshTokens.Where(rt => rt.UserId == userId);
                    dbContext.RefreshTokens.RemoveRange(userTokens);
                    user.IsTokenValid = true;
                    await dbContext.SaveChangesAsync();

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.Headers.Add("WWW-Authenticate", "Bearer error=\"invalid_token\", error_description=\"The token expired\"");
                    return;
                }
            }

            await _next(context);
        }
    }
}
