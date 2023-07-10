using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace CurriculumConstructor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string profileNumber;
        private string profileName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void FileSelectClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            (sender as Button).Content = "Загружется";

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Excel files (*.xls, *.xlsx)|*.xls;*.xlsx";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePathName = openFileDialog.FileName;

                FilePathNameTextBlock.Text = filePathName;

                DataGridDisciplines.ItemsSource = await Task.Run(() => LoadExcelDataGrid(filePathName));

                this.Title = profileNumber + " - " + profileName;
            }

            (sender as Button).IsEnabled = true;
            (sender as Button).Content = "Выберите файл (*.xlsx, *.xls)";
        }

        private List<RowElement> LoadExcelDataGrid(string filePathName)
        {
            Excel.Application app = new Excel.Application();
            Excel.Workbook workbook = app.Workbooks.Open(filePathName);

            {
                var titularWorksheet = workbook.Worksheets.Item["Титул"];

                profileNumber = (titularWorksheet.Cells[16, 2] as Excel.Range).Value2;
                profileName = (titularWorksheet.Cells[19, 2] as Excel.Range).Value2;
            }

            var worksheet = workbook.Worksheets.Item["ПланСвод"];

            List<RowElement> rowElements = new List<RowElement>();

            int rowNumber = 6, number = 1;
            while (true)
            {
                string ANum = (worksheet.Cells[rowNumber, 1] as Excel.Range).Value2;

                if (ANum != "+" && number == 2)
                {
                    break;
                }
                else if (ANum != "+")
                {
                    while (ANum != "+")
                    {
                        rowNumber++;

                        ANum = (worksheet.Cells[rowNumber, 1] as Excel.Range).Value2;
                    }

                    number = 2;
                }

                RowElement rowElement = new RowElement();

                rowElement.rowNumber = rowNumber;
                rowElement.Discipline = (worksheet.Cells[rowNumber, 3] as Excel.Range).Value2;
                rowElement.Exam = (worksheet.Cells[rowNumber, 4] as Excel.Range).Value2;
                rowElement.Offset = (worksheet.Cells[rowNumber, 5] as Excel.Range).Value2;
                rowElement.OffsetWithMark = (worksheet.Cells[rowNumber, 6] as Excel.Range).Value2;
                rowElement.Expert = (worksheet.Cells[rowNumber, 8] as Excel.Range).Value2;
                rowElement.Actual = (worksheet.Cells[rowNumber, 9] as Excel.Range).Value2;
                rowElement.Semester1 = (worksheet.Cells[rowNumber, 16] as Excel.Range).Value2;
                rowElement.Semester2 = (worksheet.Cells[rowNumber, 17] as Excel.Range).Value2;
                rowElement.Semester3 = (worksheet.Cells[rowNumber, 18] as Excel.Range).Value2;
                rowElement.Semester4 = (worksheet.Cells[rowNumber, 19] as Excel.Range).Value2;
                rowElement.Semester5 = (worksheet.Cells[rowNumber, 20] as Excel.Range).Value2;
                rowElement.Semester6 = (worksheet.Cells[rowNumber, 21] as Excel.Range).Value2;
                rowElement.Semester7 = (worksheet.Cells[rowNumber, 22] as Excel.Range).Value2;
                rowElement.Semester8 = (worksheet.Cells[rowNumber, 23] as Excel.Range).Value2;
                rowElement.Department = (worksheet.Cells[rowNumber, 25] as Excel.Range).Value2;

                rowElements.Add(rowElement);

                rowNumber++;
            }

            workbook.Close();

            return rowElements;
        }

        private void DataGridDisciplines_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            int selectedIndex = dataGrid.SelectedIndex;

            if(selectedIndex == -1)
            {
                return;
            }

            RowElement rowElement = dataGrid.Items[selectedIndex] as RowElement;

            DocumentReplaceObject.Discipline = rowElement.Discipline;

            SettingMenuWindow settingMenuWindow = new SettingMenuWindow(profileNumber, rowElement);

            Hide();
            settingMenuWindow.ShowDialog();
            Show();
        }
    }

    public class RowElement
    {
        public int rowNumber;
        public string Discipline { get; set; }
        public string Exam { get; set; }
        public string Offset { get; set; }
        public string OffsetWithMark { get; set; }
        public string Expert { get; set; }
        public string Actual { get; set; }
        public string Semester1 { get; set; }
        public string Semester2 { get; set; }
        public string Semester3 { get; set; }
        public string Semester4 { get; set; }
        public string Semester5 { get; set; }
        public string Semester6 { get; set; }
        public string Semester7 { get; set; }
        public string Semester8 { get; set; }
        public string Department { get; set; }
    }
}
