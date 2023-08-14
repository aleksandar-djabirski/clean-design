using Claims.Domain.Enums;

namespace Claims.Domain.Entities
{
    public class Claim
    {
        public string Id { get; set; }

        public string CoverId { get; set; }

        public DateTime Created { get; set; }

        public string Name { get; set; }

        public ClaimType Type { get; set; }

        public decimal DamageCost { get; set; }

        public Claim(string coverId, DateTime created, string name, ClaimType type, decimal damageCost)
        {
            if (damageCost > 100000)
                throw new ArgumentException("DamageCost cannot exceed 100,000.");

            CoverId = coverId;
            Created = created;
            Name = name;
            Type = type;
            DamageCost = damageCost;
        }

    }
}
