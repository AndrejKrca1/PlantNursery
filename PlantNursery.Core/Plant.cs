using System;

namespace PlantNursery.Core
{

    public interface ICareSchedulable
    {
        void Water();
        void Fertilize();
        void Repot();
        DateTime ScheduleNextCare();
    }

    public abstract class Plant : ICareSchedulable
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string CommonName { get; set; }
        public PlantType Type { get; set; }
        public string PhotoPath { get; set; }

        private int _wateringFrequencyDays = 7;
        public int WateringFrequencyDays
        {
            get => _wateringFrequencyDays;
            set
            {
                if (value <= 0)
                    throw new InvalidCareFrequencyException(
                        "Frekvencija zalijevanja mora biti veća od 0 dana.");
                _wateringFrequencyDays = value;
            }
        }

        public DateTime LastWatered { get; set; } = DateTime.Now;
        public int Health { get; set; } = 100;

        public abstract string GetCareInstructions();

        public virtual int AssessHealth()
        {
            int daysSince = (DateTime.Now - LastWatered).Days;
            if (daysSince > WateringFrequencyDays * 2) Health = Math.Max(0, Health - 30);
            else if (daysSince > WateringFrequencyDays) Health = Math.Max(0, Health - 10);
            return Health;
        }

        public void Water()
        {
            LastWatered = DateTime.Now;
            Health = Math.Min(100, Health + 10);
        }

        public void Fertilize() => Health = Math.Min(100, Health + 5);

        public void Repot() => Health = Math.Min(100, Health + 3);

        public DateTime ScheduleNextCare()
            => LastWatered.AddDays(WateringFrequencyDays);

        public string DisplayName()
            => $"{CommonName} ({LatinName}) - zadnja njega: {LastWatered:dd.MM.yyyy}";
    }
}
