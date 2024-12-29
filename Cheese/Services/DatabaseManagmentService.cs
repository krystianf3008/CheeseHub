using CheeseHub.Data;
using Microsoft.EntityFrameworkCore;

namespace CheeseHub.Services
{
    public static class DatabaseManagmentService
    {
        public static async void MigrationInitialisation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                 var service = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                    await service.Database.MigrateAsync();
            }
        }
    }
}
