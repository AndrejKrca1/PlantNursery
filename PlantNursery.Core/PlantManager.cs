using System;
using System.Collections.Generic;
using System.Linq;

namespace PlantNursery.Core
{

    public class CareTask
    {
        public Plant Plant { get; set; }
        public CareType Type { get; set; }
        public DateTime DueDate { get; set; }

        public override string ToString()
            => $"{DueDate:dd.MM.yyyy} - {Type} za {Plant.CommonName}";
    }

    public class PlantFilter<T> where T : Plant
    {
        public IEnumerable<T> Filter(IEnumerable<T> plants, Func<T, bool> predicate)
            => plants.Where(predicate);
    }

    public class PlantManager
    {

        public List<Plant> Plants { get; } = new();
        public Dictionary<CareType, List<Plant>> ByCareType { get; } = new();
        public Queue<CareTask> ScheduledTasks { get; } = new();
        public SortedList<DateTime, Plant> Schedule { get; } = new();

        public event EventHandler<CareTask> OnCareTaskDue;

        public PlantManager()
        {
            foreach (CareType ct in Enum.GetValues(typeof(CareType)))
                ByCareType[ct] = new List<Plant>();
        }

        public void AddPlant(Plant plant)
        {
            Plants.Add(plant);
            ByCareType[CareType.Zalijevanje].Add(plant);

            DateTime next = plant.ScheduleNextCare();
            while (Schedule.ContainsKey(next)) next = next.AddSeconds(1);
            Schedule[next] = plant;

            ScheduledTasks.Enqueue(new CareTask
            {
                Plant = plant,
                Type = CareType.Zalijevanje,
                DueDate = next
            });
        }

        public void CheckSchedule()
        {
            foreach (var task in ScheduledTasks.ToList())
            {
                if (task.DueDate <= DateTime.Now)
                    OnCareTaskDue?.Invoke(this, task);
            }
        }

        public IEnumerable<Plant> FilterByType(PlantType type)
        {
            var filter = new PlantFilter<Plant>();
            return filter.Filter(Plants, p => p.Type == type);
        }
    }
}
