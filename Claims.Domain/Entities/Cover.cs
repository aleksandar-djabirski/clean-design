using Claims.Domain.Enums;

namespace Claims.Domain.Entities
{
    public class Cover
    {
        public string Id { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public CoverType Type { get; set; }

        public decimal Premium { get; set; }

        public Cover(string id, DateOnly startDate, DateOnly endDate, CoverType type, decimal premium)
        {
            if (startDate < DateOnly.FromDateTime(DateTime.Now.Date))
            { 
                throw new ArgumentException("StartDate cannot be in the past.");
            }

            if ((new DateTime(endDate.Year, endDate.Month, endDate.Day) - new DateTime(startDate.Year, startDate.Month, startDate.Day)).TotalDays > 365)
            {
                throw new ArgumentException("Total insurance period cannot exceed 1 year.");
            }

            Id = id;
            StartDate = startDate;
            EndDate = endDate;
            Type = type;
            Premium = premium;
        }

    }
}
