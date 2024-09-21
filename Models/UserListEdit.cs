namespace ChatApp.Models
{
    public class UserListEdit
    {
        public short UserId { get; set; }

        public string FullName { get; set; } = null!;

        public string EmailAddress { get; set; } = null!;

        public string UserPassword { get; set; } = null!;

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public DateTime? LastUpdated { get; set; }

        public string EncId { get; set; } = null!;
    }
}
