using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StarDebris.Avalonia.MessageBox;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class MainWindow : Window
    {
        CSV_ImportExport csv_ie = new CSV_ImportExport();

        public MainWindow()
        {
            PropertyChanged += Center_window;

            InitializeComponent();

            this.FindControl<Button>("getFileBtn").Click += GetFileBtn_Click;
        }

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
        }

        private void Center_window(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "ClientSize")
            {
                var screen_geomtry = Screens.Primary.Bounds;
                var newBounds = new Rect(((Size)e.NewValue));
                Position = new Point(screen_geomtry.Center.X - newBounds.Center.X, screen_geomtry.Center.Y - newBounds.Center.Y);
            }
        }

		protected async Task<string> GetInputFileFullPath()
        {
			OpenFileDialog dlg = new OpenFileDialog() { Title = "Open file", AllowMultiple = false };
			dlg.Filters.Add(new FileDialogFilter() {
				Name = "CSV", Extensions = new List<string>() { "csv" }
			});

			var files = await dlg.ShowAsync(this);
			csv_ie.path2File = files == null || files.Length == 0 ? "" : files[0];
            return csv_ie.path2File;
        }

        public async Task<List<oneStringStructure>> ImportRawFromCSV()
        {
			var file = await GetInputFileFullPath();
            return string.IsNullOrEmpty(file) ? null : CSV_ImportExport.ImportRawFromCSV(file);
        }

        async void GetFileBtn_Click(object sender, RoutedEventArgs e)
        {
            List<oneStringStructure> result = await ImportRawFromCSV();

            if ((result != null) && (result.Count > 0))
            {
               MainActionp(result);
            }
            else
            {
				var mb = new MessageBox("Входной файл не содержит полезных данных",
					MessageBoxStyle.Error, MessageBoxButtons.Ok);
				mb.Show();
            }
        }

        private void MainActionp(List<oneStringStructure> res)
        {
			MessageBox mb;
			MainAction ma = new MainAction();
            bool result = ma.Do(res, csv_ie);
            if (result)
            {
				mb = new MessageBox("Файл отчёта сохранён", MessageBoxStyle.Info, MessageBoxButtons.Ok);
            }
            else
            {
				mb = new MessageBox("Файл отчёта НЕ сохранён!\nВозможно, он открыт в другой программе",
					MessageBoxStyle.Error, MessageBoxButtons.Ok);
            }
			mb.Show();
        }
    }
}
