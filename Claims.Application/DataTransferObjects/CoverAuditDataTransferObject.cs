namespace Claims.Application.DataTransferObjects
{
    public class CoverAuditDataTransferObject
    {
        public int Id { get; set; }
        public string? CoverId { get; set; }
        public DateTime Created { get; set; }
        public string? HttpRequestType { get; set; }
    }
}
