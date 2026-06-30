using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PlantNursery.Core;

namespace PlantNursery.WPF
{

    public partial class CalendarWindow : Window
    {
        private readonly PlantManager _manager = new();

        public CalendarWindow()
        {
            InitializeComponent();
            SeedData();
            ShowAllTasks();
        }

        private void SeedData()
        {
            _manager.AddPlant(new IndoorPlant
            {
                Id = 1, LatinName = "Monstera deliciosa", CommonName = "Monstera",
                Type = PlantType.Sobna, WateringFrequencyDays = 7
            });
            _manager.AddPlant(new OutdoorPlant
            {
                Id = 2, LatinName = "Rosa", CommonName = "Ruža",
                Type = PlantType.Vrtna, WateringFrequencyDays = 5
            });
        }

        private void ShowAllTasks()
        {
            TaskList.Items.Clear();
            foreach (var task in _manager.ScheduledTasks)
                TaskList.Items.Add(CreateTaskItem(task));
        }

        private ListBoxItem CreateTaskItem(CareTask task)
        {
            var item = new ListBoxItem { Content = task.ToString() };
            item.Background = task.Type switch
            {
                CareType.Zalijevanje => Brushes.LightBlue,
                CareType.Gnojenje => Brushes.LightGreen,
                CareType.Presadivanje => Brushes.Wheat,
                _ => Brushes.White
            };
            return item;
        }

        private void CareCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CareCalendar.SelectedDate is not DateTime date) return;
            TaskList.Items.Clear();
            foreach (var task in _manager.ScheduledTasks
                         .Where(t => t.DueDate.Date == date.Date))
                TaskList.Items.Add(CreateTaskItem(task));

            if (TaskList.Items.Count == 0)
                TaskList.Items.Add(new ListBoxItem { Content = "Nema zadataka za ovaj dan." });
        }
    }
}
