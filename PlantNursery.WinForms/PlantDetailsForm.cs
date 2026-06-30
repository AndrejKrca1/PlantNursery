using System.Windows.Forms;
using PlantNursery.Core;

namespace PlantNursery.WinForms
{

    public class PlantDetailsForm : Form
    {
        public PlantDetailsForm(Plant plant)
        {
            Text = "Detalji - " + plant.CommonName;
            Width = 400;
            Height = 300;

            var lbl = new Label
            {
                Left = 10, Top = 10, Width = 360, Height = 200,
                Text = plant.DisplayName() + "\r\n\r\n" + plant.GetCareInstructions() +
                       $"\r\n\r\nZdravlje: {plant.AssessHealth()}%"
            };
            Controls.Add(lbl);

            var pic = new PictureBox
            {
                Left = 10, Top = 210, Width = 100, Height = 50,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            if (!string.IsNullOrEmpty(plant.PhotoPath) && System.IO.File.Exists(plant.PhotoPath))
                pic.ImageLocation = plant.PhotoPath;
            Controls.Add(pic);
        }
    }
}
