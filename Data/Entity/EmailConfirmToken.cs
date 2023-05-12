namespace ASP_201.Data.Entity
{
    public class EmailConfirmToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public String UserEmail { get; set; }
        public DateTime Moment { get; set; }
        public int Used { get; set; }
    }
}
