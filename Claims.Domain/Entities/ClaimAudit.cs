namespace Claims.Domain.Entities
{
    public class ClaimAudit
    {
        public int Id { get; set; }

        public string? ClaimId { get; set; }

        public DateTime Created { get; set; }

        public string? HttpRequestType { get; set; }
    }
}
