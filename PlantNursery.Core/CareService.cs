using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PlantNursery.Core
{

    public delegate void CareNotification(string message);

    public class CareService
    {
        private readonly PlantManager _manager;

        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public event CareNotification Notify;

        public CareService(PlantManager manager) => _manager = manager;

        public void StartDailyCheck()
        {
            var thread = new Thread(() =>
            {
                _manager.CheckSchedule();
                Notify?.Invoke($"Dnevna provjera obavljena: {DateTime.Now:HH:mm:ss}");
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public async Task<int> AnalyzeAllHealthAsync()
        {
            return await Task.Run(async () =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    int total = 0;
                    foreach (var plant in _manager.Plants)
                        total += plant.AssessHealth();
                    int avg = _manager.Plants.Count > 0 ? total / _manager.Plants.Count : 0;
                    Notify?.Invoke($"Prosječno zdravlje: {avg}%");
                    return avg;
                }
                finally
                {
                    _semaphore.Release();
                }
            });
        }
    }
}
