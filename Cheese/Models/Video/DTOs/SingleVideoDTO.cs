﻿namespace CheeseHub.Models.Video.DTOs
{
    public class SingleVideoDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TotalLikes { get; set; }
        public int TotalViews{ get; set; }
        public int TotalDisLikes { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool? IsLiked { get; set; }
        public bool? IsDisLiked { get; set; }
        public char Status{ get; set; }
    }
}
