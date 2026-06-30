using System;
using System.Linq;
using System.Windows.Forms;
using PlantNursery.Core;

namespace PlantNursery.WinForms
{

    public class MainForm : Form
    {
        private readonly PlantManager _manager = new();
        private readonly CareService _careService;

        private TreeView _treeView;
        private ListView _listView;
        private ProgressBar _healthBar;
        private DateTimePicker _nextWaterPicker;
        private CheckBox _doneCheck;
        private ComboBox _seasonFilter;
        private System.Windows.Forms.Timer _timer;

        public MainForm()
        {
            _careService = new CareService(_manager);
            _careService.Notify += msg => Text = "PlantNursery - " + msg;
            SeedData();
            BuildUi();
            RefreshTree();
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
            _manager.AddPlant(new IndoorPlant
            {
                Id = 3, LatinName = "Aloe vera", CommonName = "Aloja",
                Type = PlantType.Sukulenti, WateringFrequencyDays = 14
            });
        }

        private void BuildUi()
        {
            Text = "PlantNursery";
            Width = 820;
            Height = 560;

            var menu = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("Datoteka");
            var importPhoto = new ToolStripMenuItem("Uvezi fotografiju...", null, ImportPhoto);
            var analyze = new ToolStripMenuItem("Analiziraj zdravlje", null, async (s, e) =>
            {
                int avg = await _careService.AnalyzeAllHealthAsync();
                MessageBox.Show($"Prosječno zdravlje: {avg}%");
            });
            var openCalendar = new ToolStripMenuItem("Otvori kalendar (info)", null,
                (s, e) => MessageBox.Show("Kalendar njege je u WPF aplikaciji."));
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { importPhoto, analyze, openCalendar });
            menu.Items.Add(fileMenu);
            MainMenuStrip = menu;
            Controls.Add(menu);

            _treeView = new TreeView { Left = 10, Top = 30, Width = 250, Height = 400 };
            _treeView.AfterSelect += TreeView_AfterSelect;
            Controls.Add(_treeView);

            _listView = new ListView
            {
                Left = 280, Top = 30, Width = 500, Height = 200,
                View = View.Details, FullRowSelect = true
            };
            _listView.Columns.Add("Naziv", 200);
            _listView.Columns.Add("Sljedeća njega", 150);
            _listView.Columns.Add("Zdravlje", 100);
            Controls.Add(_listView);

            var lblFilter = new Label { Text = "Sezonski filter:", Left = 280, Top = 245, Width = 100 };
            Controls.Add(lblFilter);
            _seasonFilter = new ComboBox
            {
                Left = 390, Top = 242, Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _seasonFilter.Items.AddRange(new object[] { "Sve", "Sobna", "Vrtna", "Sukulenti" });
            _seasonFilter.SelectedIndex = 0;
            _seasonFilter.SelectedIndexChanged += (s, e) => RefreshList();
            Controls.Add(_seasonFilter);

            var lblHealth = new Label { Text = "Zdravstveni status:", Left = 280, Top = 285, Width = 120 };
            Controls.Add(lblHealth);
            _healthBar = new ProgressBar { Left = 410, Top = 282, Width = 200, Maximum = 100, Value = 100 };
            Controls.Add(_healthBar);

            var lblNext = new Label { Text = "Sljedeće zalijevanje:", Left = 280, Top = 320, Width = 120 };
            Controls.Add(lblNext);
            _nextWaterPicker = new DateTimePicker { Left = 410, Top = 317, Width = 200 };
            Controls.Add(_nextWaterPicker);

            _doneCheck = new CheckBox { Text = "Njega obavljena", Left = 280, Top = 355, Width = 200 };
            Controls.Add(_doneCheck);

            _timer = new System.Windows.Forms.Timer { Interval = 60000 };
            _timer.Tick += (s, e) => _careService.StartDailyCheck();
            _timer.Start();
        }

        private void RefreshTree()
        {
            _treeView.Nodes.Clear();
            foreach (PlantType type in Enum.GetValues(typeof(PlantType)))
            {
                var node = new TreeNode(type.ToString());
                foreach (var p in _manager.FilterByType(type))
                    node.Nodes.Add(new TreeNode(p.CommonName) { Tag = p });
                _treeView.Nodes.Add(node);
            }
            _treeView.ExpandAll();
            RefreshList();
        }

        private void RefreshList()
        {
            _listView.Items.Clear();
            var plants = _manager.Plants.AsEnumerable();
            if (_seasonFilter.SelectedIndex > 0)
            {
                var sel = (PlantType)Enum.Parse(typeof(PlantType), _seasonFilter.SelectedItem.ToString());
                plants = plants.Where(p => p.Type == sel);
            }
            foreach (var p in plants)
            {
                var item = new ListViewItem(p.CommonName);
                item.SubItems.Add(p.ScheduleNextCare().ToString("dd.MM.yyyy"));
                item.SubItems.Add(p.Health + "%");
                _listView.Items.Add(item);
            }
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is Plant p)
            {
                _healthBar.Value = Math.Max(0, Math.Min(100, p.AssessHealth()));
                _nextWaterPicker.Value = p.ScheduleNextCare();
            }
        }

        private void ImportPhoto(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog { Filter = "Slike|*.jpg;*.png;*.jpeg" };
            if (dlg.ShowDialog() == DialogResult.OK && _treeView.SelectedNode?.Tag is Plant p)
            {
                p.PhotoPath = dlg.FileName;
                MessageBox.Show("Fotografija uvezena za: " + p.CommonName);
            }
        }
    }
}
