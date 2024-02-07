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
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using System.IO;

namespace CurriculumConstructor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        TitleDataClass TitleData = new TitleDataClass();
        Dictionary<int, (string, string)> ParentGroupId_Block_Part = new Dictionary<int, (string, string)>();
        private Excel.Application app;

        public MainWindow()
        {
            InitializeComponent();
        }

        ~MainWindow()
        {
            if(app != null)
            {
                app.Quit();
            }
        }

        private async void FileSelectClickAsync(object sender, RoutedEventArgs e)
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

                this.Title = TitleData.ProfileNumber + " - " + TitleData.ProfileName;
            }

            (sender as Button).IsEnabled = true;
            (sender as Button).Content = "Выберите файл (*.xlsx, *.xls)";
        }

        private List<DisciplineRow> LoadExcelDataGrid(string filePathName)
        {
            app = new Excel.Application();
            Excel.Workbook workbook = app.Workbooks.Open(filePathName);
            Dictionary<string, List<int>> discipline_courseworkSemesters = new Dictionary<string, List<int>>();

            var GetCell = (Worksheet worksheet, int lineNumber, int columnNumber) =>
            {
                return Convert.ToString((worksheet.Cells[lineNumber, columnNumber] as Excel.Range).Value2);
            };
            
            {
                var titularWorksheet = workbook.Worksheets.Item["Титул"];

                TitleData.ProfileNumber = GetCell(titularWorksheet, 16, 2);
                TitleData.ProfileName = GetCell(titularWorksheet, 19, 2);
                TitleData.DepartmentName = GetCell(titularWorksheet, 26, 2);
                TitleData.StartYear = GetCell(titularWorksheet, 29, 20);

                TitleData.EducationForm = GetCell(titularWorksheet, 31, 1);
                TitleData.EducationPeriod = GetCell(titularWorksheet, 32, 1);
                TitleData.Qualification = GetCell(titularWorksheet, 29, 1);

                TitleData.EducationForm = TitleData.EducationForm.Substring(TitleData.EducationForm.IndexOf(": ") + 2);
                TitleData.EducationPeriod = TitleData.EducationPeriod.Substring(TitleData.EducationPeriod.IndexOf(": ") + 2);
                TitleData.Qualification = TitleData.Qualification.Substring(TitleData.Qualification.IndexOf(": ") + 2);
            }

            int rowNumber = 2;

            {
                var courseworkWorksheet = workbook.Worksheets.Item["Курсовые"];

                string Value = GetCell(courseworkWorksheet, rowNumber, 1); // A2

                while (Value != null)
                {
                    string disciplineName = Value;

                    List<int> courseworkSemesters = new List<int>();
                    rowNumber++;

                    while (GetCell(courseworkWorksheet, rowNumber, 1) == null
                        && GetCell(courseworkWorksheet, rowNumber, 2) != null)
                    {
                        int courseNumber = Convert.ToInt32(GetCell(courseworkWorksheet, rowNumber, 3)),
                            courseSemesterNumber = Convert.ToInt32(GetCell(courseworkWorksheet, rowNumber, 4));

                        courseworkSemesters.Add((courseNumber - 1) * 2 + courseSemesterNumber);

                        rowNumber++;
                    }

                    discipline_courseworkSemesters.Add(disciplineName, courseworkSemesters);

                    Value = GetCell(courseworkWorksheet, rowNumber, 1);
                }
            }

            rowNumber = 3;

            {
                var competencyWorksheet = workbook.Worksheets.Item["Компетенции"];
                var competencyCodeNames = TitleData.CompetencyCode_Names;

                string Value = GetCell(competencyWorksheet, rowNumber, 2); ; // B3

                while(Value != null)
                {
                    GeneralModel.CompetencyCode_Name code_Name = new GeneralModel.CompetencyCode_Name();

                    code_Name.Code = GetCell(competencyWorksheet, rowNumber, 2);
                    code_Name.CodeName = GetCell(competencyWorksheet, rowNumber, 4);

                    competencyCodeNames.Add(code_Name);

                    rowNumber++;

                    while (GetCell(competencyWorksheet, rowNumber, 2) == null
                        && GetCell(competencyWorksheet, rowNumber, 3) != null)
                        rowNumber++;

                    Value = GetCell(competencyWorksheet, rowNumber, 2);
                };
            }

            var worksheet = workbook.Worksheets.Item["План"];
            int semestersCount = Convert.ToInt32(
                string.Join("", TitleData.EducationPeriod.TakeWhile(x => char.IsDigit(x)))
                ) * 2; // Years * semesters count in one year

            List<DisciplineRow> disciplineRows = new List<DisciplineRow>();

            int parentGroupId = 0;
            rowNumber = 4;

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
                rowElement.Exam = GetCell(worksheet, rowNumber, 4) ?? "";
                rowElement.Offset = GetCell(worksheet, rowNumber, 5) ?? "";
                rowElement.OffsetWithMark = GetCell(worksheet, rowNumber, 6) ?? "";
                rowElement.Control = GetCell(worksheet, rowNumber, 7) ?? "0";
                rowElement.Expert = GetCell(worksheet, rowNumber, 8) ?? "0";
                rowElement.Actual = GetCell(worksheet, rowNumber, 9) ?? "0";
                rowElement.HoursPerCreditUnit = Convert.ToInt32(GetCell(worksheet, rowNumber, 10)) ?? 0;
                rowElement.ContansHours = Convert.ToInt32(GetCell(worksheet, rowNumber, 13)) ?? 0;

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

                if (discipline_courseworkSemesters.ContainsKey(rowElement.DisciplineName))
                {
                    rowElement.CourseworkSemesters = discipline_courseworkSemesters[rowElement.DisciplineName].ToArray();
                }
                else
                {
                    rowElement.CourseworkSemesters = new int[0];
                }

                disciplineRows.Add(rowElement);

                rowNumber++;
            }

            workbook.Close();
            app.Quit();

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

            SettingMenuWindow settingMenuWindow = new SettingMenuWindow(ParentGroupId_Block_Part[rowElement.ParentGroup], TitleData, rowElement);

            Hide();
            settingMenuWindow.ShowDialog();
            Show();
        }

        private async void FileParamsSelectClickAsync(object sender, RoutedEventArgs e)
        {
            var openFileDiallog = new OpenFileDialog();

            if (openFileDiallog.ShowDialog() != true)
                return;

            string path = openFileDiallog.FileName;

            string? jsonString = "";

            using (StreamReader reader = new StreamReader(path))
            {
                jsonString = await reader.ReadLineAsync();
            }

            if(jsonString != null)
            {
                var options = new JsonSerializerOptions
                {
                    IncludeFields = true,
                };

                GeneralModel? generalModel = JsonSerializer.Deserialize<GeneralModel>(jsonString, options);

                if (generalModel == null)
                    return;

                SettingMenuWindow settingMenuWindow = new SettingMenuWindow(generalModel);

                Hide();
                settingMenuWindow.ShowDialog();
                Show();
            }
        }
    }

    public class TitleDataClass
    {
        public string ProfileNumber { get; set; } = "";
        public string ProfileName { get; set; } = "";

        public string Qualification { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string EducationForm { get; set; } = "";
        public string EducationPeriod { get; set; } = "";
        public string StartYear { get; set; } = "";

        public List<GeneralModel.CompetencyCode_Name> CompetencyCode_Names { get; set; } = new List<GeneralModel.CompetencyCode_Name>();
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
        public int HoursPerCreditUnit { get; set; }
        public int ContansHours { get; set; }
        

        public List<Semester> Semesters { get; set; } = new List<Semester>();
        public int Code { get; set; }
        public string DepartmentName { get; set; }
        public string[] Competencies { get; set; }
        public int[] CourseworkSemesters { get; set; }
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
