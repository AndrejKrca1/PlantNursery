using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PlantNursery.Core
{

    public class AppSettings
    {
        public string GardenName { get; set; } = "Moj vrt";
        public int ReminderHour { get; set; } = 8;
    }

    public static class DataService
    {

        public static void SaveToXml(List<PlantEntity> plants, string path)
        {
            var serializer = new XmlSerializer(typeof(List<PlantEntity>));
            using var writer = new StreamWriter(path);
            serializer.Serialize(writer, plants);
        }

        public static List<PlantEntity> LoadFromXml(string path)
        {
            if (!File.Exists(path)) return new List<PlantEntity>();
            var serializer = new XmlSerializer(typeof(List<PlantEntity>));
            using var reader = new StreamReader(path);
            return (List<PlantEntity>)serializer.Deserialize(reader);
        }

        public static void SaveSettings(AppSettings settings, string path)
            => File.WriteAllText(path,
                JsonSerializer.Serialize(settings,
                    new JsonSerializerOptions { WriteIndented = true }));

        public static AppSettings LoadSettings(string path)
            => File.Exists(path)
                ? JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(path))
                : new AppSettings();

        public static List<PlantEntity> ReadCsvCatalog(string path)
        {
            var result = new List<PlantEntity>();
            if (!File.Exists(path)) return result;

            foreach (var line in File.ReadLines(path).Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length < 4) continue;
                result.Add(new PlantEntity
                {
                    LatinName = parts[0],
                    CommonName = parts[1],
                    Type = parts[2],
                    WateringFrequencyDays = int.Parse(parts[3])
                });
            }
            return result;
        }

        public static string GenerateCareGuideHtml(Plant plant)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><head><meta charset='utf-8'><title>Vodič za njegu</title></head><body>");
            sb.AppendLine($"<h1>{plant.CommonName} ({plant.LatinName})</h1>");
            sb.AppendLine($"<p><b>Vrsta:</b> {plant.Type}</p>");
            sb.AppendLine($"<p>{plant.GetCareInstructions()}</p>");
            sb.AppendLine($"<p><b>Sljedeća njega:</b> {plant.ScheduleNextCare():dd.MM.yyyy}</p>");
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }

        public static async Task StartTipServerAsync(int port, string tip)
        {
            var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            using var client = await listener.AcceptTcpClientAsync();
            using var stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(tip);
            await stream.WriteAsync(data, 0, data.Length);
            listener.Stop();
        }
    }
}
