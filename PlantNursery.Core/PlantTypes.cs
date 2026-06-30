namespace PlantNursery.Core
{

    public class IndoorPlant : Plant
    {
        public override string GetCareInstructions()
            => $"Sobna biljka '{CommonName}': zalijevaj svakih {WateringFrequencyDays} dana, " +
               "drži na svijetlom mjestu bez izravnog sunca.";
    }

    public class OutdoorPlant : Plant
    {
        public override string GetCareInstructions()
            => $"Vrtna biljka '{CommonName}': zalijevaj svakih {WateringFrequencyDays} dana, " +
               "pazi na vremenske uvjete i mraz.";

        public override int AssessHealth()
        {
            int daysSince = (System.DateTime.Now - LastWatered).Days;
            if (daysSince > WateringFrequencyDays * 3) Health = System.Math.Max(0, Health - 20);
            return Health;
        }
    }
}
