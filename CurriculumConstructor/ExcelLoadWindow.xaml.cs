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
using CurriculumConstructor.SettingMenu.Model;
using System.Drawing;
using Microsoft.Office.Interop.Excel;
using Button = System.Windows.Controls.Button;

namespace CurriculumConstructor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        Dictionary<int, (string, string)> ParentGroupId_Block_Part = new Dictionary<int, (string, string)>();

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

                this.Title = TitleModel.Title.ProfileNumber + " - " + TitleModel.Title.ProfileName;
            }

            (sender as Button).IsEnabled = true;
            (sender as Button).Content = "Выберите файл (*.xlsx, *.xls)";
        }

        private List<DisciplineRow> LoadExcelDataGrid(string filePathName)
        {
            Excel.Application app = new Excel.Application();
            Excel.Workbook workbook = app.Workbooks.Open(filePathName);
            
            var GetCell = (Worksheet worksheet, int lineNumber, int columnNumber) =>
            {
                return (string)(worksheet.Cells[lineNumber, columnNumber] as Excel.Range).Value2;
            };
            
            {
                var titularWorksheet = workbook.Worksheets.Item["Титул"];

                TitleModel.Title.ProfileNumber = GetCell(titularWorksheet, 16, 2);
                TitleModel.Title.ProfileName = GetCell(titularWorksheet, 19, 2);
                TitleModel.Title.DepartmentName = GetCell(titularWorksheet, 26, 2);
                TitleModel.Title.StartYear = GetCell(titularWorksheet, 29, 20);

                TitleModel.Title.EducationForm = GetCell(titularWorksheet, 31, 1);
                TitleModel.Title.EducationPeriod = GetCell(titularWorksheet, 32, 1);
                TitleModel.Title.Qualification = GetCell(titularWorksheet, 29, 1);

                TitleModel.Title.EducationForm = TitleModel.Title.EducationForm.Substring(TitleModel.Title.EducationForm.IndexOf(": ") + 2);
                TitleModel.Title.EducationPeriod = TitleModel.Title.EducationPeriod.Substring(TitleModel.Title.EducationPeriod.IndexOf(": ") + 2);
                TitleModel.Title.Qualification = TitleModel.Title.Qualification.Substring(TitleModel.Title.Qualification.IndexOf(": ") + 2);
            }

            var worksheet = workbook.Worksheets.Item["План"];
            int semestersCount = Convert.ToInt32(
                string.Join("", TitleModel.Title.EducationPeriod.TakeWhile(x => char.IsDigit(x)))
                ) * 2; // Years * semesters count in one year

            List<DisciplineRow> disciplineRows = new List<DisciplineRow>();

            int rowNumber = 4, parentGroupId = 0;
            while (true)
            {
                string AValue = GetCell(worksheet, rowNumber, 1) ?? "";

                if (AValue != "+" )
                {
                    if (AValue != "")
                    {
                        string NextAValue = GetCell(worksheet, rowNumber + 1, 1) ?? "";

                        if (NextAValue != "+") // If it is new block
                        {
                            ParentGroupId_Block_Part.Add(++parentGroupId, (AValue, NextAValue));

                            rowNumber += 2;
                        }
                        else // If it is subblock
                        {
                            ParentGroupId_Block_Part.Add(parentGroupId + 1, (
                                    ParentGroupId_Block_Part[parentGroupId].Item1,
                                    AValue)
                            );

                            parentGroupId += 1;

                            rowNumber += 1;
                        }

                        continue;
                    }
                    else
                    {
                        int skippedCells = 1;

                        while (skippedCells < 8)
                        {
                            AValue = GetCell(worksheet, rowNumber + skippedCells, 1) ?? "";

                            if (AValue != "")
                            {
                                break;
                            }

                            skippedCells++;
                        }

                        if (skippedCells >= 8)
                        {
                            break;
                        }

                        rowNumber += skippedCells;

                        continue;
                    }
                }
                else if (GetCell(worksheet, rowNumber, 16 + 7 * semestersCount) is null)
                {
                    rowNumber++;
                    continue;
                }
                

                DisciplineRow rowElement = new DisciplineRow();

                rowElement.ParentGroup = parentGroupId;
                rowElement.rowNumber = rowNumber;

                rowElement.Index = GetCell(worksheet, rowNumber, 2);
                rowElement.DisciplineName = GetCell(worksheet, rowNumber, 3);
                rowElement.Exam = GetCell(worksheet, rowNumber, 4) ?? "0";
                rowElement.Offset = GetCell(worksheet, rowNumber, 5) ?? "0";
                rowElement.OffsetWithMark = GetCell(worksheet, rowNumber, 6) ?? "0";
                rowElement.Control = GetCell(worksheet, rowNumber, 7) ?? "0";
                rowElement.Expert = GetCell(worksheet, rowNumber, 8) ?? "0";
                rowElement.Actual = GetCell(worksheet, rowNumber, 9) ?? "0";

                for (int semesterNumber = 1; semesterNumber <= semestersCount; semesterNumber++)
                {
                    int semesterColumnStartPosition = 16 + 7 * (semesterNumber - 1);

                    if (GetCell(worksheet, rowNumber, semesterColumnStartPosition + 1) is null)
                        continue;

                    Semester semester = new Semester();

                    semester.SemesterNumber = semesterNumber;

                    semester.CreditUnits = GetCell(worksheet, rowNumber, semesterColumnStartPosition) ?? "0";
                    semester.Total = GetCell(worksheet, rowNumber, semesterColumnStartPosition + 1) ?? "0";
                    semester.Lectures = GetCell(worksheet, rowNumber, semesterColumnStartPosition + 2) ?? "0";
                    semester.LaboratoryWorks = GetCell(worksheet, rowNumber, semesterColumnStartPosition + 3) ?? "0";
                    semester.PracticeWorks = GetCell(worksheet, rowNumber, semesterColumnStartPosition + 4) ?? "0";
                    semester.IndependentWork = GetCell(worksheet, rowNumber, semesterColumnStartPosition + 5) ?? "0";
                    semester.Control = GetCell(worksheet, rowNumber, semesterColumnStartPosition + 6) ?? "0";

                    rowElement.Semesters.Add(semester);
                }

                rowElement.Code = Convert.ToInt32(GetCell(worksheet, rowNumber, 16 + 7 * semestersCount));
                rowElement.DepartmentName = GetCell(worksheet, rowNumber, 16 + 7 * semestersCount + 1);
                rowElement.Competencies = GetCell(worksheet, rowNumber, 16 + 7 * semestersCount + 2).Split("; ");

                disciplineRows.Add(rowElement);

                rowNumber++;
            }

            workbook.Close();

            return disciplineRows;
        }

        private void DataGridDisciplines_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            int selectedIndex = dataGrid.SelectedIndex;

            if(selectedIndex == -1)
            {
                return;
            }

            DisciplineRow rowElement = dataGrid.Items[selectedIndex] as DisciplineRow;

            DocumentReplaceObject.Discipline = rowElement.DisciplineName;

            SettingMenuWindow settingMenuWindow = new SettingMenuWindow(rowElement);

            Hide();
            settingMenuWindow.ShowDialog();
            Show();
        }
    }

    public class DisciplineRow
    {
        public int ParentGroup { get; set; }
        public int rowNumber;
        public string Index { get; set; }
        public string DisciplineName { get; set; }
        public string Exam { get; set; }
        public string Offset { get; set; }
        public string OffsetWithMark { get; set; }
        public string Control { get; set; }
        public string Expert { get; set; }
        public string Actual { get; set; }
        public List<Semester> Semesters { get; set; } = new List<Semester>();
        public int Code { get; set; }
        public string DepartmentName { get; set; }
        public string[] Competencies { get; set; }
    }

    public class Semester
    {
        public int SemesterNumber { get; set; }
        public string CreditUnits { get; set; }
        public string Total { get; set; }
        public string Lectures { get; set; }
        public string LaboratoryWorks { get; set; }
        public string PracticeWorks { get; set; }
        public string IndependentWork { get; set; }
        public string Control { get; set; }
    }
}
