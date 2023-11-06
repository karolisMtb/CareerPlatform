namespace CareerPlatform.DataAccess.Entities
{
    public class ResetPasswordEntry
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public byte[] HashedToken { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public User User { get; set; }
    }
}
