using CheeseHub.Models.User;

namespace CheeseHub.Models.RefreshToken
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User.User User { get; set; }
    }

}
