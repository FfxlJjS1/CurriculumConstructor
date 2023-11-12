using CurriculumConstructor;
using CurriculumConstructor.SettingMenu.Model;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Ink;
using static CurriculumConstructor.SettingMenu.Model.GeneralModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Word = Microsoft.Office.Interop.Word;

namespace TestWord
{
    class WordHelper
    {
        private FileInfo _fileInfo;
        private Word.Application app;
        private Word._Document wordDocument;

        //данные
        GeneralModel generalModel;

        public WordHelper(string fileName, ref GeneralModel generalModel)
        {
            if (File.Exists(fileName))
            {
                _fileInfo = new FileInfo(fileName);

                this.generalModel = generalModel;
            }
            else
            {
                throw new ArgumentException("File not found");
            }
        }

        ~WordHelper()
        {
            try
            {
                app.ActiveDocument.Close(Word.WdSaveOptions.wdDoNotSaveChanges);

            }
            catch (Exception ex) { }

            if (app is not null)
            {
                app.Quit();
            }
        }

        internal bool Process(bool forPreview, string nameForSave = "shablon_1.docx")
        {
            try
            {
                Object file = _fileInfo.FullName;

                app = new Word.Application();
                wordDocument = app.Documents.Open(file, ReadOnly: false);

                replaceText(new Dictionary<string, string>());

                CreateDisciplineThematicPlanTable(); // 4.1.
                CreateAcquiredCompetenciesAsDisciplineMasteringResultTable(); // 1.; Annotation
                CreateDisciplineContentTable(); // 4.2.
                // createTable5(); // Have questions
                CreateAcquiredCompetenciesWithEvaluationCriteriesTable(); // 6.2.

                CreateCompetenciesFormingLevelEvaluationTestTasksTable(); // 6.3.1.2.
                CreateAssesmentToolsContentTable(); // 6.3.4.3.
                CreateRatingPointsDisctributionByDisciplineTables(); // 6.4.

                CreateExamTestTasksVariantTemplateTable();

                CreateEducationLiteratureTable(); // 6.
                CreateProffectionalBasesTable(); // 8.
                CreateSoftwareInfoTable(); // 10.
                CreateMaterialTechnicalBaseTable(); // 11.
                CreateAcquiredCompetenciesAsDisciplineMasteringResultTable();

                if (forPreview)
                    PreviewView(nameForSave);
                else
                    saveWord();

                app.ActiveDocument.Close(Word.WdSaveOptions.wdDoNotSaveChanges);

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка! " + ex.Message + "\n" +
                    ex.StackTrace + "\n" +
                    ex.TargetSite + "\n" +
                    ex.HelpLink);
                Console.WriteLine(ex.Message);

                app.ActiveDocument.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
                app.Quit();
            }
            finally
            {
                if (app is not null)
                {
                    app.Quit();
                }
            }

            return false;
        }

        internal void PreviewView(string nameForTempSave)
        {
            //Если нужно, через FileInfo можно получить другие данные
            FileInfo targetDir = new FileInfo("./" + nameForTempSave);

            string pathToFolder = targetDir.FullName + "";
            string name_folder = targetDir.Name;

            Object newFileName = System.IO.Path.Combine(@pathToFolder.ToString(), nameForTempSave);
            app.ActiveDocument.SaveAs2(newFileName);
        }

        private void saveWord()
        {
            //путь и название будущего ФАЙЛА
            String name = DateTime.Now.ToString("dd-MM-yyyy HHmmss ") + _fileInfo.Name;
            String pathMain = "";

            //выбор пути и сохранение
            using (var path_dialog = new FolderBrowserDialog())
                if (path_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Путь к директории
                    Object path = path_dialog.SelectedPath;

                    //Если нужно, через FileInfo можно получить другие данные
                    FileInfo targetDir = new FileInfo((string)path);

                    string pathToFolder = targetDir.FullName + "";
                    string name_folder = targetDir.Name;

                    pathMain = pathToFolder.ToString() + "\\" + name;

                    Object newFileName = System.IO.Path.Combine(@pathToFolder.ToString(), name);
                    app.ActiveDocument.SaveAs2(newFileName);

                    System.Windows.MessageBox.Show("Успешное сохранение!");
                };

            //открытие сохраненного файла
            if (pathMain != "")
                System.Diagnostics.Process.Start(new ProcessStartInfo(@pathMain) { UseShellExecute = true });
        }

        private void replaceText(Dictionary<string, string> items)
        {
            List<Semester> semesters = generalModel.Semesters;

            if (items.Count <= 0)
            {
                items = new Dictionary<string, string>
                {
                    //EXCEL ИЛИ ПРОГРАММНО РАССЧИТАТЬ
                    //0-1
                    {"<YEAR>", DateTime.Now.Year.ToString() },
                    {"<INDEX>", generalModel.Index },
                    {"<DISCIPLINE>", generalModel.DisciplineName }, //6, 6.3.1.1 (ЛАБЫ), 6.4
                    { "<DISCIPLINE_UP_CASE>", generalModel.DisciplineName.ToUpper()}, // 11, аннотация
                    {"<DIRECTION>", generalModel.ProfileNumber + " – " +  generalModel.ProfileName }, //2, 6.4, 12, аннотация
                    {"<PROFILE>", generalModel.ProfileName }, //2, 12, аннотация
                    {"<QUALIFICATION>", generalModel.Qualification },
                    {"<FORM_STUDY>", generalModel.EducationForm.ToLower() },
                    {"<LANGUAGE_STUDY>", "русский" },
                    {"<YEAR_START>", generalModel.StartYear },
                    //2
                    {"<BLOCK_1>", generalModel.ParentBlock_1}, // "Блока 1 \"Дисциплины (модули)\""
                    {"<BLOCK_2>", generalModel.ParentSubBlock_1}, // "обязательной части"
                    {"<COURSE_SEMESTER>",  string.Join(", " ,semesters.Select(semester =>  ((int)((semester.SemesterNumber - 1) / 2) + 1).ToString() + " курсе в " + semester.SemesterNumber + " семестре"))},
                    //3
                    {"<TOILSOMENESS>", generalModel.Actual + " зачетных единиц, "
                        + (Convert.ToInt32(generalModel.Actual) * Convert.ToInt32(generalModel.HoursPerCreditUnit)).ToString()
                        + " часов"},
                    {"<CREDIT_UNIT_COUNT>", generalModel.Actual },
                    {"<EDUCATION_HOURS>", generalModel.DisciplineThematicPlan.Sum(x => x.Value.DisciplineThematicPlan.Sum(y => y.AllHour)).ToString() },
                    {"<CONTACT_WORK_HOUR_WITH_TEACHER>", generalModel.ContansHours.ToString() },
                    { "<LECTURE_HOURS>", semesters.Sum(semester => Convert.ToInt32(semester.Lectures)).ToString()},
                    {"<PRACTICE_HOURS>" , semesters.Sum(semester => Convert.ToInt32(semester.PracticeWorks)).ToString()},
                    {"<LABORATORY_HOURS>" , semesters.Sum(semester => Convert.ToInt32(semester.LaboratoryWorks)).ToString()},
                    {"<INDEPENDENT_HOURS>" , semesters.Sum(semester => Convert.ToInt32(semester.IndependentWork)).ToString()},
                    {"<CONTROL_HOURS>" ,semesters.Sum(semester => Convert.ToInt32(semester.Control)).ToString()},
                    {"<LEARNING_THEMES>", string.Join("\n" , generalModel.DisciplineThematicPlan.SelectMany(x => x.Value.DisciplineThematicPlan.Select(theme => theme.ThemeName))) },
                    {"<ATTESTATION>", "экзамен в 4 семестре"}, //6.4 //зачет с оценкой в 1, 2, 3 семестрах, экзамен в 4 семестре
                    //6
                    {"<ATTESTATION_2>", "экзамена"}, //зачета с оценкой (1, 2, 3 семестры) и экзамена (4 семестр)
                    //

                    //ВВОДИМЫЕ ДАННЫЕ
                    {"<AUTHOR>", generalModel.Author },
                    {"<AUTHOR_IN_THE_INSTRUMENTAL_CASE>", generalModel.AuthorInTheInstrumentalCase }, // Before 1 in the same page // Автор в творительном падеже
                    {"<REVIEWER>", generalModel.Reviewer },
                    {"<DEPARTMENT_CHAIR>", generalModel.DepartmentChair },
                    //5
                    {"<METHOD_BOOK>", generalModel.MethodBook }

                };
            }


            //замена простого текст
            foreach (var item in items)
            {
                ReplaceTextToTag(item.Key, item.Value);
            }
        }

        private void ReplaceTextToTag(string tag, string text)
        {
            Object missing = Type.Missing;

            string textForLongerReplace = "<FRT>";
            //замена простого текст

            string forReplace = text;

            int textCount = text.Length;
            int index = 0;
            bool isLonger = textCount - index > 255;

            do
            {
                Word.Find find = app.Selection.Find;

                if (isLonger)
                {
                    forReplace = text.Substring(index, 255 - textForLongerReplace.Length) + textForLongerReplace;
                }
                else
                {
                    forReplace = text.Substring(index);
                }

                find.Text = index == 0 ? tag : textForLongerReplace;
                find.Replacement.Text = forReplace;

                Object wrap = Word.WdFindWrap.wdFindContinue;
                Object replace = Word.WdReplace.wdReplaceAll;

                find.Execute(FindText: Type.Missing,
                    MatchCase: false,
                    MatchWholeWord: false,
                    MatchWildcards: false,
                    MatchSoundsLike: missing,
                    MatchAllWordForms: false,
                    Forward: true,
                    Wrap: wrap,
                    Format: false,
                    ReplaceWith: missing, Replace: replace
                    );

                index += (isLonger ? 255 - textForLongerReplace.Length : 255);

                isLonger = textCount - index > 255;
            } while (index < textCount);
        }

        // 4.1.
        private void CreateDisciplineThematicPlanTable()
        {
            List<(int semesterNumber, SemesterModuleData.DisciplineThematicTheme theme)> themes = new List<(int semesterNumber, SemesterModuleData.DisciplineThematicTheme theme)>();

            {
                var items = generalModel.DisciplineThematicPlan.Select(x => new { semesterNumber = x.Key.semesterNumber, x.Value.DisciplineThematicPlan });

                foreach (var item in items)
                {
                    themes.AddRange(item.DisciplineThematicPlan.Select(x => (item.semesterNumber, x)));
                }
            }

            app.Selection.Find.Execute("<DISCIPLINE_THEMATIC_PLAN_TABLE>");

            Word.Range tableRange = app.Selection.Range;
            var wordTable = wordDocument.Tables.Add(tableRange,
                2 + themes.Count + 1, 7);

            // Entering and formating columns rows
            wordTable.Rows[1].Range.Bold = 1;
            wordTable.Rows[2].Range.Bold = 1;
            wordTable.Rows[2 + themes.Count + 1].Range.Bold = 1;

            wordTable.Cell(1, 1).Merge(wordTable.Cell(2, 1));
            wordTable.Cell(1, 2).Merge(wordTable.Cell(2, 2));
            wordTable.Cell(1, 3).Merge(wordTable.Cell(3, 3));

            wordTable.Cell(1, 4).Merge(wordTable.Cell(1, 5));
            wordTable.Cell(1, 4).Merge(wordTable.Cell(1, 6));

            wordTable.Cell(1, 7).Merge(wordTable.Cell(2, 7));

            // Text orienting
            wordTable.Cell(1, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable.Cell(1, 2).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable.Cell(1, 3).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable.Cell(1, 4).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable.Cell(2, 4).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable.Cell(2, 5).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable.Cell(2, 6).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable.Cell(1, 3).Range.Orientation = WdTextOrientation.wdTextOrientationUpward;
            wordTable.Cell(2, 4).Range.Orientation = WdTextOrientation.wdTextOrientationUpward;
            wordTable.Cell(2, 5).Range.Orientation = WdTextOrientation.wdTextOrientationUpward;
            wordTable.Cell(2, 6).Range.Orientation = WdTextOrientation.wdTextOrientationUpward;
            wordTable.Cell(1, 7).Range.Orientation = WdTextOrientation.wdTextOrientationUpward;


            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Темы дисциплины";
            wordTable.Cell(1, 3).Range.Text = "семестр";
            wordTable.Cell(1, 4).Range.Text = "Виды и часы "
                + "контактной \nработы, \nих трудоемкость \n(в часах)";
            wordTable.Cell(2, 4).Range.Text = "Лекции";
            wordTable.Cell(2, 5).Range.Text = "Практические \nзанятия";
            wordTable.Cell(2, 6).Range.Text = "Лабораторные \nзанятия";
            wordTable.Cell(1, 7).Range.Text = "СРС";

            // Columns widthing
            wordTable.Columns[1].Width = 1.17f;
            wordTable.Columns[2].Width = 8.07f;
            wordTable.Columns[3].Width = 1.86f;
            wordTable.Columns[4].Width = 1.56f;
            wordTable.Columns[5].Width = 1.72f;
            wordTable.Columns[6].Width = 1.71f;
            wordTable.Columns[7].Width = 1.22f;


            int countItems = themes.Count;

            for (int i = 0; i < countItems; i++)
            {
                int currentTableRow = 3 + i;
                int semesterNumber = themes[i].semesterNumber;
                var theme = themes[i].theme;

                wordTable.Cell(currentTableRow, 1).Range.Text = (i + 1).ToString();
                wordTable.Cell(currentTableRow, 2).Range.Text = theme.ThemeName;
                wordTable.Cell(currentTableRow, 3).Range.Text = semesterNumber.ToString();
                wordTable.Cell(currentTableRow, 4).Range.Text = theme.LectureHours != 0 ? theme.LectureHours.ToString() : "-";
                wordTable.Cell(currentTableRow, 5).Range.Text = theme.PracticeHours != 0 ? theme.PracticeHours.ToString() : "-";
                wordTable.Cell(currentTableRow, 6).Range.Text = theme.LaboratoryWorkHours != 0 ? theme.LaboratoryWorkHours.ToString() : "-";
                wordTable.Cell(currentTableRow, 7).Range.Text = theme.IndependentHours != 0 ? theme.IndependentHours.ToString() : "-";

                //выравнивание=слева
                wordTable.Cell(3 + i, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            }

            // Last row
            wordTable.Cell(3 + countItems, 1).Range.Text = "";
            wordTable.Cell(3 + countItems, 2).Range.Text = "Итого по дисциплине";
            wordTable.Cell(3 + countItems, 3).Range.Text = "";
            wordTable.Cell(3 + countItems, 4).Range.Text = generalModel.NeedTotalLectureHours.ToString();
            wordTable.Cell(3 + countItems, 5).Range.Text = generalModel.NeedTotalPracticeHours.ToString();
            wordTable.Cell(3 + countItems, 6).Range.Text = generalModel.NeedTotalLaboratoryWorkHours.ToString();
            wordTable.Cell(3 + countItems, 7).Range.Text = generalModel.NeedTotalIndependentHours.ToString();

            //форматирование таблицы
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Borders.Enable = 1;
        }


        private void CreateAcquiredCompetenciesAsDisciplineMasteringResultTable()
        {
            //данные - код, имя, знать, уметь, владеть, индикаторы
            List<CompetencyPlanningResult> competences = generalModel.competencyPlanningResults;

            app.Selection.Find.Execute("<ACQUITED_COMPETENCIES_TABLE>");
            Word.Range wordRange = app.Selection.Range;

            var wordTable = wordDocument.Tables.Add(wordRange,
                competences.Count + 1, 4); // Rows: Competencies count + row for attribute names

            // Entering data from model
            wordTable.Cell(1, 1).Range.Text = "Оцениваемые компетенции (код, наименование)";
            wordTable.Cell(1, 2).Range.Text = "Код и наименование индикатора (индикаторов) достижения компетенции";
            wordTable.Cell(1, 3).Range.Text = "Результаты освоения компетенции";
            wordTable.Cell(1, 4).Range.Text = "Оценочные средства текущего контроля и промежуточной аттестации";

            for (int i = 0; i < competences.Count; i++)
            {
                int currentTableRow = 2 + i;
                var row = competences[i];
                Dictionary<int, string> childs = row.CompetencyAchivmentIndicators;

                // Column 1
                Word.Range range = wordTable.Cell(currentTableRow, 1).Range;
                range.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range.InsertAfter(row.Code);
                range.Font.Bold = Convert.ToInt32(true);
                range.InsertParagraphAfter();
                range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                range.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range.InsertAfter(generalModel.competencyCode_Names.First(x => x.Code == row.Code).CodeName);
                range.Font.Bold = Convert.ToInt32(false);
                range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                // Column 2
                Word.Range range2 = wordTable.Cell(currentTableRow, 2).Range;
                int childIndex = 0;

                foreach (var keyValuePair in childs)
                {
                    string childCode = row.Code + "." + keyValuePair.Key + ".";
                    string childName = keyValuePair.Value;

                    range2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range2.InsertAfter(childCode + ".");
                    range2.Font.Bold = Convert.ToInt32(true);
                    range2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                    range2.Collapse(Word.WdCollapseDirection.wdCollapseStart);

                    if (childIndex == childs.Count - 1)
                    {
                        range2.InsertAfter(" " + childName + ".");
                    }
                    else
                    {
                        range2.InsertAfter(" " + childName + ";");
                        range2.InsertParagraphAfter();
                    }

                    range2.Font.Bold = Convert.ToInt32(false);
                    range2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                    childIndex++;
                }

                // Column 3
                Word.Range range3 = wordTable.Cell(currentTableRow, 3).Range;

                // To know
                range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range3.InsertAfter("Знать:");
                range3.Font.Bold = Convert.ToInt32(true);
                range3.InsertParagraphAfter();
                range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                for (int toKnowIndex = 0; toKnowIndex < row.ToKnowResult.Count; toKnowIndex++)
                {
                    string know = row.ToKnowResult[toKnowIndex];

                    range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range3.InsertAfter(know.ToLower() + (toKnowIndex < row.ToKnowResult.Count - 1 ? ";" : "."));
                    range3.Font.Bold = Convert.ToInt32(false);
                    range3.InsertParagraphAfter();
                    range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                }

                // To able
                range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range3.InsertAfter("Уметь:");
                range3.Font.Bold = Convert.ToInt32(true);
                range3.InsertParagraphAfter();
                range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                for (int toAbleIndex = 0; toAbleIndex < row.ToAbilityResult.Count; toAbleIndex++)
                {
                    string able = row.ToAbilityResult[toAbleIndex];

                    range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range3.InsertAfter(able.ToLower() + (toAbleIndex < row.ToAbilityResult.Count - 1 ? ";" : "."));
                    range3.Font.Bold = Convert.ToInt32(false);
                    range3.InsertParagraphAfter();
                    range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                }

                // To own
                range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range3.InsertAfter("Владеть:");
                range3.Font.Bold = Convert.ToInt32(true);
                range3.InsertParagraphAfter();
                range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                for (int toOwnIndex = 0; toOwnIndex < row.ToOwnResult.Count; toOwnIndex++)
                {
                    string own = row.ToOwnResult[toOwnIndex];

                    range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range3.InsertAfter(own.ToLower() + (toOwnIndex < row.ToOwnResult.Count - 1 ? ";" : "."));
                    range3.Font.Bold = Convert.ToInt32(false);
                    range3.InsertParagraphAfter();
                    range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                }

                //!!!!!!!!!!!!!!!!ДОРАБОТАТЬ - данные брать из excel или программы
                // Работает по странному...
                //Столбец4
                if (currentTableRow != 2)
                    wordTable.Cell(currentTableRow, 4).Merge(wordTable.Cell(2, 4));
            }

            Word.Range range4 = wordTable.Cell(2, 4).Range;
            // Текущий контроль
            range4.Collapse(Word.WdCollapseDirection.wdCollapseStart);
            range4.InsertAfter("Текущий контроль:");
            range4.Font.Bold = Convert.ToInt32(true);
            range4.InsertParagraphAfter();
            range4.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

            range4.Collapse(Word.WdCollapseDirection.wdCollapseStart);

            // TODO: If exists
            range4.InsertAfter("Компьютерное тестирование по теме 1-5\n");
            range4.InsertAfter("Практические задачи по темам 1-5\n");
            range4.InsertAfter("Лабораторные работыпо темам 1-3");

            range4.Font.Bold = Convert.ToInt32(false);
            range4.InsertParagraphAfter();
            range4.InsertParagraphAfter();
            range4.InsertParagraphAfter();
            range4.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

            // Промежуточная аттестация
            range4.Collapse(Word.WdCollapseDirection.wdCollapseStart);
            range4.InsertAfter("Промежуточная аттестация:");
            range4.Font.Bold = Convert.ToInt32(true);
            range4.InsertParagraphAfter();
            range4.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

            range4.Collapse(Word.WdCollapseDirection.wdCollapseStart);
            range4.InsertAfter("Экзамен"); //!!!!!
            range4.Font.Bold = Convert.ToInt32(false);
            range4.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

            // Formating table columns and rows
            wordTable.Borders.Enable = 1;
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            wordTable.Cell(1, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Cell(1, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Cell(1, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Cell(1, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            wordTable.Cell(1, 1).Range.Bold = Convert.ToInt32(true);
            wordTable.Cell(1, 2).Range.Bold = Convert.ToInt32(true);
            wordTable.Cell(1, 3).Range.Bold = Convert.ToInt32(true);
            wordTable.Cell(1, 4).Range.Bold = Convert.ToInt32(true);

            wordTable.Columns[1].Width = 4.17f;
            wordTable.Columns[2].Width = 3.97f;
            wordTable.Columns[3].Width = 3.98f;
            wordTable.Columns[4].Width = 5.36f;
        }

        // 4.2.
        private void CreateDisciplineContentTable()
        {
            var disciplineThematicPlan = generalModel.DisciplineThematicPlan;

            string main_key = "<DISCIPLINE_CONTENT_TABLE>";

            app.Selection.Find.Execute(main_key);
            Word.Range wordRange = app.Selection.Range;

            int rowsCount = 1  // Columns row
                    + disciplineThematicPlan.Count // Discipline module rows
                    + disciplineThematicPlan.Sum(x => x.Value.DisciplineThematicPlan.Count // Themes count
                    + disciplineThematicPlan.Sum(x => x.Value.DisciplineThematicPlan.Sum(x => x.ThemeContents.Count))); // Lectures + practices + laboratories count

            var wordTable = wordDocument.Tables.Add(app.Selection.Range,
                    rowsCount, 4);

            wordTable.Cell(1, 1).Range.Text = "Тема";
            wordTable.Cell(1, 2).Range.Text = "Кол-во часов";
            wordTable.Cell(1, 3).Range.Text = "Используемый метод";
            wordTable.Cell(1, 4).Range.Text = "Формируемые компетенции";

            wordTable.Columns[1].Width = 9.54f;
            wordTable.Columns[2].Width = 1.69f;
            wordTable.Columns[3].Width = 3.24f;
            wordTable.Columns[4].Width = 3.01f;

            wordTable.Rows[1].Range.Bold = 1;

            int lectureNumber = 1,
                laboratoryNumber = 1,
                practiceNumber = 1,
                themeNumber = 1;
            int rowNumber = 2;

            foreach (var disciplineModule in disciplineThematicPlan)
            {
                var themes = disciplineModule.Value.DisciplineThematicPlan;

                wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 2));
                wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 3));
                wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 4));

                wordTable.Rows[rowNumber].Range.Bold = 1;

                wordTable.Cell(rowNumber, 1).Range.Text = "Дисциплинарный модуль " + disciplineModule.Key.semesterNumber + "." + disciplineModule.Key.semesterModuleNumber;

                if (themes.Count > 0)
                {
                    rowNumber++;
                }

                for (int themeIndex = 0; themeIndex < themes.Count; themeIndex++)
                {
                    var theme = themes[themeIndex];

                    var lectures = theme.ThemeContents.Where(x => x.ThemeType == SemesterModuleData.DisciplineThematicTheme.ThemeContent.ThemeTypeEnum.Lecture).ToList();
                    var laboratories = theme.ThemeContents.Where(x => x.ThemeType == SemesterModuleData.DisciplineThematicTheme.ThemeContent.ThemeTypeEnum.LaboratoryWork).ToList();
                    var practices = theme.ThemeContents.Where(x => x.ThemeType == SemesterModuleData.DisciplineThematicTheme.ThemeContent.ThemeTypeEnum.PracticeWork).ToList();

                    wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 2));
                    wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 3));
                    wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 4));

                    wordTable.Rows[rowNumber].Range.Bold = 1;

                    string themeTag = "<_THEME_" + themeNumber + ">";
                    string themeText = "Тема " + themeNumber.ToString() + ". " + themeTag + " ("
                        + theme.ThemeContents.Sum(x => x.Hour)
                        + " ч.)";

                    wordTable.Cell(rowNumber, 1).Range.Text = themeText;

                    ReplaceTextToTag(themeTag, theme.ThemeName);

                    int coupleIndex = 0;
                    string tagForReplace = "<_PFR>"; // Place for replace

                    // Lectures writing
                    if (lectures.Count() > 0)
                        rowNumber++;

                    for (; coupleIndex < lectures.Count(); coupleIndex++)
                    {
                        var lecture = lectures[coupleIndex];

                        Word.Range rangeColumn1 = wordTable.Cell(rowNumber, 1).Range;
                        rangeColumn1.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                        rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseStart);

                        rangeColumn1.InsertAfter("Лекция " + lectureNumber++ + ". ");
                        rangeColumn1.Font.Italic = 1;

                        rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                        rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                        rangeColumn1.InsertAfter(tagForReplace);
                        rangeColumn1.Font.Italic = Convert.ToInt32(false);
                        rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                        ReplaceTextToTag(tagForReplace, lecture.ThemeText);

                        wordTable.Cell(rowNumber, 2).Range.Text = lecture.Hour.ToString();

                        wordTable.Cell(rowNumber, 3).Range.Text = lecture.UsingMethod;
                        wordTable.Cell(rowNumber, 3).Range.Italic = 1;

                        wordTable.Cell(rowNumber, 4).Range.Text = string.Join(", ", lecture.FormingCompetency);

                        if (coupleIndex < lectures.Count() - 1)
                        {
                            rowNumber++;
                        }
                    }

                    // Laboratory writing
                    if (laboratories.Count() > 0)
                        rowNumber++;

                    for (coupleIndex = 0; coupleIndex < laboratories.Count(); coupleIndex++)
                    {
                        var laboratory = laboratories[coupleIndex];

                        Word.Range rangeColumn1 = wordTable.Cell(rowNumber, 1).Range;
                        rangeColumn1.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                        rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseStart);

                        rangeColumn1.InsertAfter("Лабораторная работа " + laboratoryNumber++ + ". ");
                        rangeColumn1.Font.Italic = 1;

                        rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                        rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                        rangeColumn1.InsertAfter(tagForReplace);
                        rangeColumn1.Font.Italic = Convert.ToInt32(false);
                        rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                        ReplaceTextToTag(tagForReplace, laboratory.ThemeText);

                        wordTable.Cell(rowNumber, 2).Range.Text = laboratory.Hour.ToString();

                        wordTable.Cell(rowNumber, 3).Range.Text = laboratory.UsingMethod;
                        wordTable.Cell(rowNumber, 3).Range.Italic = 1;

                        wordTable.Cell(rowNumber, 4).Range.Text = string.Join(", ", laboratory.FormingCompetency);

                        if (coupleIndex < laboratories.Count() - 1)
                        {
                            rowNumber++;
                        }
                    }

                    // Practices writing
                    if (practices.Count() > 0)
                        rowNumber++;

                    for (coupleIndex = 0; coupleIndex < practices.Count(); coupleIndex++)
                    {
                        var practice = practices[coupleIndex];

                        Word.Range rangeColumn1 = wordTable.Cell(rowNumber, 1).Range;
                        rangeColumn1.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                        rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseStart);

                        rangeColumn1.InsertAfter("Практическое занятие " + practiceNumber++ + ". ");
                        rangeColumn1.Font.Italic = 1;

                        rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                        rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                        rangeColumn1.InsertAfter(tagForReplace);
                        rangeColumn1.Font.Italic = Convert.ToInt32(false);
                        rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                        ReplaceTextToTag(tagForReplace, practice.ThemeText);

                        wordTable.Cell(rowNumber, 2).Range.Text = practice.Hour.ToString();

                        wordTable.Cell(rowNumber, 3).Range.Text = practice.UsingMethod;
                        wordTable.Cell(rowNumber, 3).Range.Italic = 1;

                        wordTable.Cell(rowNumber, 4).Range.Text = string.Join(", ", practice.FormingCompetency);

                        if (coupleIndex < practices.Count() - 1)
                        {
                            rowNumber++;
                        }
                    }

                    if (themeIndex < themes.Count - 1) // For last theme in current discipline module if next discipline module is exist
                    {
                        rowNumber++;
                    }
                }

                rowNumber++;
            }
        }

        // 6.2
        private void CreateAcquiredCompetenciesWithEvaluationCriteriesTable()
        {
            //данные - код, имя, знать, уметь, владеть, индикаторы
            List<CompetencyPlanningResult> competences = generalModel.competencyPlanningResults;

            app.Selection.Find.Execute("<>");
            Word.Range wordRange = app.Selection.Range;

            var wordTable = wordDocument.Tables.Add(wordRange,
                competences.Count * 3 + 1 + 3, 7); // Rows: Competencies count + row for attribute names

            // Entering data from model
            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Оцениваемые компетенции (код, наименование)";
            wordTable.Cell(1, 3).Range.Text = "Код и наименование индикатора (индикаторов) достижения компетенции";
            wordTable.Cell(1, 4).Range.Text = "планируемые\nрезультаты\nобучения";
            wordTable.Cell(1, 5).Range.Text = "Уроверь компетенций";

            wordTable.Cell(2, 5).Range.Text = "Продвинутый уровень";
            wordTable.Cell(2, 6).Range.Text = "Средний уровень";
            wordTable.Cell(2, 7).Range.Text = "Базовый уровень";
            wordTable.Cell(2, 8).Range.Text = "Компетенциии не освоены";

            wordTable.Cell(3, 5).Range.Text = "Критерии оценивания результатов обучения";

            wordTable.Cell(4, 5).Range.Text = "«отлично»" + "";
            wordTable.Cell(4, 6).Range.Text = "«хорошо»" + "";
            wordTable.Cell(4, 7).Range.Text = "«удовлетворительно»" + "";
            wordTable.Cell(4, 8).Range.Text = "«неудовлетв.»" + "";


            // Formating table columns and rows
            wordTable.Borders.Enable = 1;
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            wordTable.Rows[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Rows[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Rows[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Rows[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            wordTable.Rows[1].Range.Bold = 1;
            wordTable.Rows[2].Range.Bold = 1;
            wordTable.Rows[3].Range.Bold = 1;
            wordTable.Rows[4].Range.Bold = 1;

            wordTable.Columns[1].Width = 0.98f;
            wordTable.Columns[2].Width = 3.27f;
            wordTable.Columns[3].Width = 3.7f;
            wordTable.Columns[4].Width = 3.7f;
            wordTable.Columns[5].Width = 3.82f;
            wordTable.Columns[6].Width = 3.62f;
            wordTable.Columns[7].Width = 3.51f;
            wordTable.Columns[8].Width = 3.58f;


            // Entering data
            for (int i = 0; i < competences.Count; i++)
            {
                int rowNumber = 5 + i;
                var row = competences[i];
                Dictionary<int, string> childs = row.CompetencyAchivmentIndicators;

                // Column 1
                Word.Range range1 = wordTable.Cell(rowNumber, 1).Range;
                range1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range1.InsertAfter((i + 1).ToString());
                range1.Font.Bold = 1;
                range1.InsertParagraphAfter();
                range1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber + 3, 1));

                // Column 2
                Word.Range range2 = wordTable.Cell(rowNumber, 2).Range;
                range2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range2.InsertAfter(row.Code);
                range2.Font.Bold = 1;
                range2.InsertParagraphAfter();
                range2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                range2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range2.InsertAfter(generalModel.competencyCode_Names.First(x => x.Code == row.Code).CodeName);
                range2.Font.Bold = 0;
                range2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                wordTable.Cell(rowNumber, 2).Merge(wordTable.Cell(rowNumber + 3, 2));

                // Column 3
                Word.Range range3 = wordTable.Cell(rowNumber, 3).Range;
                int childIndex = 0;

                foreach (var keyValuePair in childs)
                {
                    string childCode = row.Code + "." + keyValuePair.Key + ".";
                    string childName = keyValuePair.Value;

                    range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range3.InsertAfter(childCode + ".");
                    range3.Font.Bold = 1;
                    range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                    range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);

                    if (childIndex == childs.Count - 1)
                    {
                        range3.InsertAfter(" " + childName + ".");
                    }
                    else
                    {
                        range3.InsertAfter(" " + childName + ";");
                        range3.InsertParagraphAfter();
                    }

                    range3.Font.Bold = Convert.ToInt32(false);
                    range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                    childIndex++;
                }

                wordTable.Cell(rowNumber, 3).Merge(wordTable.Cell(rowNumber + 3, 3));

                // Column 4
                Word.Range range4_1 = wordTable.Cell(rowNumber, 4).Range;

                // To know
                range4_1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4_1.InsertAfter("Знать:");
                range4_1.Font.Bold = 1;
                range4_1.InsertParagraphAfter();
                range4_1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                for (int toKnowIndex = 0; toKnowIndex < row.ToKnowResult.Count; toKnowIndex++)
                {
                    string know = row.ToKnowResult[toKnowIndex];

                    range4_1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range4_1.InsertAfter(know.ToLower() + (toKnowIndex < row.ToKnowResult.Count - 1 ? ";" : "."));
                    range4_1.Font.Bold = 0;
                    range4_1.InsertParagraphAfter();
                    range4_1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                }

                // To able
                Word.Range range4_2 = wordTable.Cell(rowNumber+1, 4).Range;
                
                range4_2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4_2.InsertAfter("Уметь:");
                range4_2.Font.Bold = 1;
                range4_2.InsertParagraphAfter();
                range4_2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                for (int toAbleIndex = 0; toAbleIndex < row.ToAbilityResult.Count; toAbleIndex++)
                {
                    string able = row.ToAbilityResult[toAbleIndex];

                    range4_2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range4_2.InsertAfter(able.ToLower() + (toAbleIndex < row.ToAbilityResult.Count - 1 ? ";" : "."));
                    range4_2.Font.Bold = 0;
                    range4_2.InsertParagraphAfter();
                    range4_2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                }

                // To own
                Word.Range range4_3 = wordTable.Cell(rowNumber + 2, 4).Range;

                range4_3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4_3.InsertAfter("Владеть:");
                range4_3.Font.Bold = 1;
                range4_3.InsertParagraphAfter();
                range4_3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                for (int toOwnIndex = 0; toOwnIndex < row.ToOwnResult.Count; toOwnIndex++)
                {
                    string own = row.ToOwnResult[toOwnIndex];

                    range4_3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range4_3.InsertAfter(own.ToLower() + (toOwnIndex < row.ToOwnResult.Count - 1 ? ";" : "."));
                    range4_3.Font.Bold = 0;
                    range4_3.InsertParagraphAfter();
                    range4_3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                }

                // Column 5
                //Word.Range range5 

                // Column 6
                
                
                // Column 7
                
                
                // Column 8

            }
        }

        // 6.4
        private void CreateRatingPointsDisctributionByDisciplineTables()
        {
            var disciplineThematicPlan = generalModel.DisciplineThematicPlan;
            var semestersNumber = generalModel.Semesters.Select(x => x.SemesterNumber).ToList();

            app.Selection.Find.Execute("<ACQUIRED_COMPETENCIES_WITH_EVALUATION_CRITERIES_TABLE>");
            Word.Range wordRange = app.Selection.Range;

            Dictionary<int, string> semesterNumberTableTags = new Dictionary<int, string>();
            Dictionary<(int semesterNumber, int semesterModuleNumber), string> semesterModuleNumberTableTags = new Dictionary<(int semesterNumber, int semesterModuleNumber), string>();

            // For semester tables
            foreach (var semesterNumber in semestersNumber)
            {
                string tagForNewTable = "<_SEMESTER_RATING_TABLE_" + semesterNumber + ">\n";

                semesterNumberTableTags.Add(semesterNumber, tagForNewTable);

                wordRange.Collapse(WdCollapseDirection.wdCollapseStart);
                wordRange.InsertAfter(tagForNewTable);
                wordRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordRange.Font.Size = 12;
                wordRange.InsertParagraphAfter();
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);
            }

            // For semester modules tables
            foreach (var semesterModuleInfo in disciplineThematicPlan)
            {
                int semesterNumber = semesterModuleInfo.Key.semesterNumber;
                int semesterModuleNumber = semesterModuleInfo.Key.semesterModuleNumber;
                string tagForNewTable = "<_SEMESTER_MODULE_RATING_TABLE_" + semesterNumber + "_" + semesterModuleNumber + ">\n";

                semesterModuleNumberTableTags.Add((semesterNumber, semesterModuleNumber), tagForNewTable);

                // Text that before table of discipline module
                wordRange.Collapse(WdCollapseDirection.wdCollapseStart);
                wordRange.InsertAfter("Дисциплинарный модуль " + semesterNumber + "." + semesterModuleNumber);
                wordRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordRange.Font.Size = 14;
                wordRange.Bold = 1;
                wordRange.InsertParagraphAfter();
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                wordRange.Collapse(WdCollapseDirection.wdCollapseStart);
                wordRange.InsertAfter(tagForNewTable);
                wordRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordRange.Font.Size = 12;
                wordRange.InsertParagraphAfter();
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);
            }

            //
            int current_lecture = 1;
            int current_laboratories = 1;
            int current_practical = 1;

            //замена тэгов на таблицы семестров
            foreach (var semesterNumber in semestersNumber)
            {
                SemesterModuleData[] semesterModules = new SemesterModuleData[] {
                    disciplineThematicPlan[(semesterNumber, 1)],
                    disciplineThematicPlan[(semesterNumber, 2)]
                };

                string semesterDisciplineTableTag = semesterNumberTableTags[semesterNumber];
                string[] semesterModulesTableTag = new string[] {
                    semesterModuleNumberTableTags[(semesterNumber, 1)],
                    semesterModuleNumberTableTags[(semesterNumber, 2)]
                };

                //ГЛАВНАЯ ТАБЛИЦА
                app.Selection.Find.Wrap = WdFindWrap.wdFindContinue;
                app.Selection.Find.Execute(semesterDisciplineTableTag);

                Word.Range wordRangeTable = app.Selection.Range;

                var wordTable = wordDocument.Tables.Add(wordRangeTable, 5, 3);

                wordTable.Cell(1, 1).Range.Text = "Дисциплинарный модуль";
                
                wordTable.Cell(2, 1).Range.Text = "Текущий контроль (лабораторные работы, практические задачи)";
                wordTable.Cell(3, 1).Range.Text = "Текущий контроль (тестирование)";
                wordTable.Cell(4, 1).Range.Text = "Общее количество баллов";
                wordTable.Cell(5, 1).Range.Text = "Итоговый балл:";
                wordTable.Cell(1, 2).Range.Text = $"ДМ {semesterNumber}.1";
                wordTable.Cell(1, 3).Range.Text = $"ДМ {semesterNumber}.2";
                
                //баллы за практические, лабы, устные
                wordTable.Cell(2, 2).Range.Text = semesterModules[0].CurrentControl_Laboratory_Practice.minPoints 
                    + "-" + semesterModules[0].CurrentControl_Laboratory_Practice.maxPoints;
                wordTable.Cell(2, 3).Range.Text = semesterModules[1].CurrentControl_Laboratory_Practice.minPoints
                    + "-" + semesterModules[1].CurrentControl_Laboratory_Practice.maxPoints;

                //баллы за тестирование
                wordTable.Cell(3, 2).Range.Text = semesterModules[0].CurrentControl_Testing.minPoints
                    + "-" + semesterModules[0].CurrentControl_Testing.maxPoints;
                wordTable.Cell(3, 3).Range.Text = semesterModules[1].CurrentControl_Testing.minPoints
                    + "-" + semesterModules[1].CurrentControl_Testing.maxPoints;

                //общее кол-во баллов
                wordTable.Cell(3, 2).Range.Text = semesterModules[0].TotalPointsCount.minPoints
                    + "-" + semesterModules[0].TotalPointsCount.maxPoints;
                wordTable.Cell(3, 3).Range.Text = semesterModules[1].TotalPointsCount.minPoints
                    + "-" + semesterModules[1].TotalPointsCount.maxPoints;

                //итоговый балл
                wordTable.Cell(5, 2).Range.Text = "35-60";

                //форматирование
                wordTable.Borders.Enable = 1;
                wordTable.Range.ParagraphFormat.SpaceBefore = 0;
                wordTable.Range.ParagraphFormat.SpaceAfter = 0;

                wordTable.Rows[1].Range.Bold = 1;
                wordTable.Rows[4].Range.Bold = 1;
                wordTable.Rows[5].Range.Bold = 1;
                
                for (int i = 1; i <= 5; i++)
                    wordTable.Cell(i, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                for (int i = 1; i <= 5; i++)
                    for (int j = 2; j <= 3; j++)
                        wordTable.Cell(i, j).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                wordTable.Cell(5, 2).Merge(wordTable.Cell(5, 3));
                wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);

                // ТАБЛИЦЫ - дисциплинарный модуль1,2
                for (int module = 1; module <= 2; module++)
                {
                    SemesterModuleData semesterModuleData = semesterModules[module - 1];
                    string semesterModuleTableTag = semesterModulesTableTag[module - 1];

                    List<SemesterModuleData.DisciplineThematicTheme> semesterModuleThemePlan = semesterModuleData.DisciplineThematicPlan;

                    List<SemesterModuleData.DisciplineThematicTheme.ThemeContent> lecture_list = semesterModuleThemePlan.SelectMany(x => x.ThemeContents.Where(theme => theme.ThemeType == SemesterModuleData.DisciplineThematicTheme.ThemeContent.ThemeTypeEnum.Lecture)).ToList();
                    List<SemesterModuleData.DisciplineThematicTheme.ThemeContent> laboratory_list = semesterModuleThemePlan.SelectMany(x => x.ThemeContents.Where(theme => theme.ThemeType == SemesterModuleData.DisciplineThematicTheme.ThemeContent.ThemeTypeEnum.LaboratoryWork)).ToList();
                    List<SemesterModuleData.DisciplineThematicTheme.ThemeContent> practical_list = semesterModuleThemePlan.SelectMany(x => x.ThemeContents.Where(theme => theme.ThemeType == SemesterModuleData.DisciplineThematicTheme.ThemeContent.ThemeTypeEnum.PracticeWork)).ToList();

                    int points = 0;
                    int row = 2 + lecture_list.Count + laboratory_list.Count + practical_list.Count + 4;
                    int column = 3;

                    app.Selection.Find.Wrap = WdFindWrap.wdFindContinue;
                    app.Selection.Find.Execute(semesterModuleTableTag);
                    wordRangeTable = app.Selection.Range;
                    wordTable = wordDocument.Tables.Add(wordRangeTable, row, column);

                    wordTable.Cell(1, 1).Range.Text = "№п/п";
                    wordTable.Cell(1, 2).Range.Text = "Виды работ";
                    wordTable.Cell(1, 3).Range.Text = "Максимальный балл";
                    wordTable.Cell(2, 1).Range.Text = "Текущий контроль";

                    // Форматирование
                    wordTable.Borders.Enable = 1;
                    wordTable.Range.ParagraphFormat.SpaceBefore = 0;
                    wordTable.Range.ParagraphFormat.SpaceAfter = 0;
                    wordTable.Range.Font.Size = 12;
                    wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                    wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    wordTable.Cell(1, 1).Range.Bold = 1;
                    wordTable.Cell(1, 2).Range.Bold = 1;
                    wordTable.Cell(1, 3).Range.Bold = 1;
                    wordTable.Cell(2, 1).Range.Bold = 1;
                    wordTable.Cell(2, 1).Merge(wordTable.Cell(2, 3));

                    int current_row = 3;
                    if (lecture_list is not null)
                        for (int index = 0; index < lecture_list.Count; index++)
                        {
                            wordTable.Cell(current_row, 1).Range.Text = (index + 1).ToString();
                            wordTable.Cell(current_row, 2).Range.Text = $"Лекция-{current_lecture} {lecture_list[index].ThemeText}";
                            wordTable.Cell(current_row, 3).Range.Text = lecture_list[index].MaxPoints.ToString();

                            points += lecture_list[index].MaxPoints;

                            //форматирование
                            wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                            current_lecture++;
                            current_row++;
                        }
                    if (laboratory_list is not null)
                        for (int index = 0; index < laboratory_list.Count; index++)
                        {
                            wordTable.Cell(current_row, 1).Range.Text = (index + 1).ToString();
                            wordTable.Cell(current_row, 2).Range.Text = $"Л.Р.-{current_laboratories} {laboratory_list[index].ThemeText}";
                            wordTable.Cell(current_row, 3).Range.Text = laboratory_list[index].MaxPoints.ToString();

                            points += laboratory_list[index].MaxPoints;

                            //форматирование
                            wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                            current_laboratories++;
                            current_row++;
                        }
                    if (practical_list is not null)
                        for (int index = 0; index < practical_list.Count; index++)
                        {
                            wordTable.Cell(current_row, 1).Range.Text = (index + 1).ToString();
                            wordTable.Cell(current_row, 2).Range.Text = $"П.З.-{current_practical} {practical_list[index].ThemeText}";
                            wordTable.Cell(current_row, 3).Range.Text = practical_list[index].MaxPoints.ToString();

                            points += practical_list[index].MaxPoints;

                            //форматирование
                            wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                            current_practical++;
                            current_row++;
                        }

                    wordTable.Cell(current_row, 1).Range.Text = "Итого:";
                    wordTable.Cell(current_row, 3).Range.Text = points.ToString();
                    
                    //форматирование
                    wordTable.Cell(current_row, 1).Range.Bold = 1;
                    wordTable.Cell(current_row, 3).Range.Bold = 1;
                    wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, 2));
                    wordTable.Cell(current_row, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    current_row++;
                    wordTable.Cell(current_row, 1).Range.Text = "Текущий контроль";
                    
                    //форматирование
                    wordTable.Cell(current_row, 1).Range.Bold = 1;
                    wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, 3));
                    current_row++;
                    wordTable.Cell(current_row, 1).Range.Text = "1";
                    wordTable.Cell(current_row, 2).Range.Text = "Тестирование";
                    wordTable.Cell(current_row, 3).Range.Text = "15";
                    
                    //форматирование
                    wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    wordTable.Cell(current_row, 3).Range.Bold = 1;
                    current_row++;
                    wordTable.Cell(current_row, 1).Range.Text = $"Итого по ДМ {semesterNumber}.{module}";
                    wordTable.Cell(current_row, 3).Range.Text = (points + 15).ToString();
                    
                    //форматирование
                    wordTable.Cell(current_row, 1).Range.Bold = 1;
                    wordTable.Cell(current_row, 3).Range.Bold = 1;
                    wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, 2));
                    wordTable.Cell(current_row, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    //финальное форматирование
                    wordTable.Columns[1].Width = 1.15f;
                    wordTable.Columns[2].Width = 12.62f;
                    wordTable.Columns[3].Width = 3.71f;
                }
            }
        }

        //6.3.1.2 Содержание оценочного средства
        private void CreateCompetenciesFormingLevelEvaluationTestTasksTable()
        {
            var testsByDisciplineModuleAndCompetencies = generalModel.TestTasksByDiscipModule;

            int rowCount = 2
                + testsByDisciplineModuleAndCompetencies.Count
                + testsByDisciplineModuleAndCompetencies.Sum(x => x.Value.competencyFormingTestTasks.Sum(x => x.testTaskLines.Count));

            app.Selection.Find.Execute("<TEST_TASKS_TABLE>");
            Word.Range wordRange = app.Selection.Range;
            Word.Table wordTable = wordDocument.Tables.Add(wordRange, rowCount, 6);

            // Шапка
            wordTable.Cell(1, 1).Range.Text = "Код компетенции";
            wordTable.Cell(1, 2).Range.Text = "Тестовые вопросы";
            wordTable.Cell(1, 3).Range.Text = "Варианты ответов";
            wordTable.Cell(2, 3).Range.Text = "1";
            wordTable.Cell(2, 4).Range.Text = "2";
            wordTable.Cell(2, 5).Range.Text = "3";
            wordTable.Cell(2, 6).Range.Text = "4";

            // Formating
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Borders.Enable = 1;
            wordTable.Cell(1, 1).Merge(wordTable.Cell(2, 1));
            wordTable.Cell(1, 2).Merge(wordTable.Cell(2, 2));
            wordTable.Cell(1, 3).Merge(wordTable.Cell(1, 6));

            wordTable.Columns[1].Width = 1.97f;
            wordTable.Columns[2].Width = 2.9f;
            wordTable.Columns[3].Width = 3.23f;
            wordTable.Columns[4].Width = 3.1f;
            wordTable.Columns[5].Width = 3.05f;
            wordTable.Columns[6].Width = 3.24f;

            int rowNumber = 3;

            foreach (var testsByDisciplineModule in testsByDisciplineModuleAndCompetencies)
            {
                var testsByCompetencies = testsByDisciplineModule.Value.competencyFormingTestTasks;

                wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 6));
                wordTable.Cell(rowNumber, 1).Range.Bold = 1;

                wordTable.Cell(rowNumber, 1).Range.Text = "Дисциплинарный модуль "
                    + testsByDisciplineModule.Key.semesterNumber + "."
                    + testsByDisciplineModule.Key.semesterModuleNumber;

                if (testsByCompetencies.Count > 0)
                    rowNumber++;

                foreach (var keyValuePair in testsByCompetencies)
                {
                    string competencies = string.Join(", ", keyValuePair.competencies);
                    var tests = keyValuePair.testTaskLines;

                    wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber + tests.Count - 1, 1));

                    wordTable.Cell(rowNumber, 1).Range.Text = competencies;

                    for (int testIndex = 0; testIndex < tests.Count; testIndex++)
                    {
                        var test = tests[testIndex];

                        wordTable.Cell(rowNumber, 2).Range.Text = test.Question;
                        wordTable.Cell(rowNumber, 3).Range.Text = test.Answers[0];
                        wordTable.Cell(rowNumber, 4).Range.Text = test.Answers[1];
                        wordTable.Cell(rowNumber, 5).Range.Text = test.Answers[2];
                        wordTable.Cell(rowNumber, 6).Range.Text = test.Answers[3];

                        wordTable.Cell(rowNumber, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                        if (testIndex < tests.Count - 1)
                            rowNumber++;
                    }
                }

                rowNumber++;
            }

            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitFixed);
        }

        //Экзамен 6.3.4.3. Содержание оценочного средства
        private void CreateAssesmentToolsContentTable()
        {
            app.Selection.Find.Execute("<ASSESMENT_TOOLS_CONTENT_TABLE>");
            var wordRange = app.Selection.Range;

            int colomn_count = 2 + generalModel.DisciplineCompetencies.Count();
            int row_count = 1 + generalModel.QuestionCodes.Count;

            Word.Table wordTable = wordDocument.Tables.Add(wordRange, row_count, colomn_count);
            wordTable.Borders.Enable = Convert.ToInt32(true);

            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            wordTable.Columns[1].Width = 1.15f;

            //заполнение
            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Примерные вопросы к экзамену";
            for (int i = 0; i < generalModel.DisciplineCompetencies.Count(); i++)
                wordTable.Cell(1, 3 + i).Range.Text = generalModel.DisciplineCompetencies[i];

            int current_row = 2;
            for (int i = 0; i < generalModel.QuestionCodes.Count; i++)
            {
                var question = generalModel.QuestionCodes[i];
                wordTable.Cell(current_row, 1).Range.Text = (i + 1).ToString();
                wordTable.Cell(current_row, 2).Range.Text = question.Question;
                for (int j = 0; j < generalModel.DisciplineCompetencies.Count(); j++)
                {
                    var current_competence = generalModel.DisciplineCompetencies[j];

                    for (int k = 0; k < question.Competencies.Count; k++)
                    {
                        if (question.Competencies.Contains(current_competence))
                            wordTable.Cell(current_row, 3 + j).Range.Text = "+";
                    }
                }

                wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                current_row++;
            }

            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
        }

        //образей вариантов тестовых заданий на экзамен
        private void CreateExamTestTasksVariantTemplateTable()
        {
            var test = generalModel.examTestTasksVariantTemplate;

            int rowsCount = 2
                + test.Values.Sum(x => x.Count);

            app.Selection.Find.Execute("<EXAM_TEST_TASKS_VARIANT_TEMPLATE>");
            Word.Range wordRange = app.Selection.Range;
            Word.Table wordTable = wordDocument.Tables.Add(wordRange, rowsCount, 6);

            //шапка
            wordTable.Cell(1, 1).Range.Text = "Код компетенции";
            wordTable.Cell(1, 2).Range.Text = "Тестовые вопросы";
            wordTable.Cell(1, 3).Range.Text = "Варианты ответов";
            wordTable.Cell(2, 3).Range.Text = "1";
            wordTable.Cell(2, 4).Range.Text = "2";
            wordTable.Cell(2, 5).Range.Text = "3";
            wordTable.Cell(2, 6).Range.Text = "4";


            //форматирование
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Borders.Enable = Convert.ToInt32(true);
            wordTable.Cell(1, 1).Merge(wordTable.Cell(2, 1));
            wordTable.Cell(1, 2).Merge(wordTable.Cell(2, 2));
            wordTable.Cell(1, 3).Merge(wordTable.Cell(1, 6));

            int rowNumber = 3;

            //данные
            foreach (var competencyTasks in test)
            {
                wordTable.Cell(rowNumber, 1).Range.Text = string.Join(", ", competencyTasks.Key);

                wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber + competencyTasks.Value.Count - 1, 1));

                foreach (var lineTask in competencyTasks.Value)
                {
                    wordTable.Cell(rowNumber, 2).Range.Text = lineTask.Question;
                    wordTable.Cell(rowNumber, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    for (int i = 0; i < lineTask.Answers.Count; i++)
                    {
                        wordTable.Cell(rowNumber, 3 + i).Range.Text = lineTask.Answers[i];
                        wordTable.Cell(rowNumber, 3 + i).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    }

                    rowNumber++;
                }
            }

            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitFixed);
        }

        
        // 6.
        private void CreateEducationLiteratureTable()
        {
            EducationLiteratureModelComplex literatureBooks = generalModel.EducationLiteraturesComplex;

            List<EducationLiteratureModelComplex.EducationLiteratureModel> main = literatureBooks.MainLiteratures;
            List<EducationLiteratureModelComplex.EducationLiteratureModel> additional = literatureBooks.AdditionalLiteratures;
            List<EducationLiteratureModelComplex.EducationLiteratureModel> methodical = literatureBooks.EducationMethodicalLiteratures;

            int row = 2 + main.Count + 1 + additional.Count + 1 + methodical.Count;
            int column = 4;
            app.Selection.Find.Execute("<TABLE11>");
            Word.Range wordTableRange = app.Selection.Range;
            Word.Table wordTable = app.ActiveDocument.Tables.Add(wordTableRange, row, column);
            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Библиографическое описание";
            wordTable.Cell(1, 3).Range.Text = "Количество печатных экземпляров или адрес электронного ресурса";
            wordTable.Cell(1, 4).Range.Text = "Коэффициент обеспеченности";

            // Форматирование
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Cell(1, 4).Range.Orientation = WdTextOrientation.wdTextOrientationUpward;
            wordTable.Cell(1, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable.Cell(1, 2).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable.Cell(1, 3).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable.Cell(1, 4).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable.Rows[1].Height = app.CentimetersToPoints(4f);
            wordTable.Range.Columns[1].Width = app.CentimetersToPoints(0.99f);
            wordTable.Range.Columns[2].Width = app.CentimetersToPoints(7.84f);
            wordTable.Range.Columns[3].Width = app.CentimetersToPoints(7.30f);
            wordTable.Range.Columns[4].Width = app.CentimetersToPoints(1.22f);

            int rowNumber = 2;

            //основная литература
            wordTable.Cell(rowNumber, 1).Range.Text = "Основная литература";
            wordTable.Cell(rowNumber, 1).Range.Bold = Convert.ToInt32(true);
            wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 4));
            rowNumber++;
            for (int i = 0; i < main.Count; i++)
            {
                wordTable.Cell(rowNumber, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(rowNumber, 2).Range.Text = $"{main[i].Name}";
                wordTable.Cell(rowNumber, 3).Range.Text = main[i].Link is not null ? $"Режим доступа:\n{main[i].Link}" : $"{main[i].Count} экз.";
                wordTable.Cell(rowNumber, 4).Range.Text = main[i].Coefficient is 0 ? "" : $"{main[i].Coefficient}";
                //форматирование
                wordTable.Cell(rowNumber, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(rowNumber, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                rowNumber++;
            }

            //дополнительная литература
            wordTable.Cell(rowNumber, 1).Range.Text = "Дополнительная литература";
            wordTable.Cell(rowNumber, 1).Range.Bold = Convert.ToInt32(true);
            wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 4));
            rowNumber++;
            for (int i = 0; i < additional.Count; i++)
            {
                wordTable.Cell(rowNumber, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(rowNumber, 2).Range.Text = $"{additional[i].Name}";
                wordTable.Cell(rowNumber, 3).Range.Text = additional[i].Link is not null ? $"Режим доступа:\n{additional[i].Link}" : $"{additional[i].Count} экз.";
                wordTable.Cell(rowNumber, 4).Range.Text = additional[i].Coefficient is 0 ? "" : $"{additional[i].Coefficient}";
                //форматирование
                wordTable.Cell(rowNumber, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(rowNumber, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                rowNumber++;
            }

            //Учебно-методические издания
            wordTable.Cell(rowNumber, 1).Range.Text = "Учебно-методические издания";
            wordTable.Cell(rowNumber, 1).Range.Bold = Convert.ToInt32(true);
            wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 4));
            rowNumber++;
            for (int i = 0; i < methodical.Count; i++)
            {
                wordTable.Cell(rowNumber, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(rowNumber, 2).Range.Text = $"{methodical[i].Name}";
                wordTable.Cell(rowNumber, 3).Range.Text = methodical[i].Link is not null ? $"Режим доступа:\n{methodical[i].Link}" : $"{methodical[i].Count} экз.";
                wordTable.Cell(rowNumber, 4).Range.Text = methodical[i].Coefficient is 0 ? "" : $"{methodical[i].Coefficient}";
                //форматирование
                wordTable.Cell(rowNumber, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(rowNumber, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                rowNumber++;
            }

            wordTable.Borders.Enable = Convert.ToInt32(true);
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.Font.Size = 12;
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
        }

        // 8.

        private void CreateProffectionalBasesTable()
        {
            List<LiteratureModel> site_list = generalModel.SiteList;

            int row = 1 + site_list.Count;
            int column = 3;
            app.Selection.Find.Execute("<TABLE12>");
            Word.Range wordTableRange = app.Selection.Range;
            Word.Table wordTable = wordDocument.Tables.Add(wordTableRange, row, column);

            // Форматирование
            wordTable.Borders.Enable = Convert.ToInt32(true);
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            wordTable.Rows[1].Range.Bold = Convert.ToInt32(true);
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Наименование";
            wordTable.Cell(1, 3).Range.Text = "Алрес в Интернете";

            wordTable.Columns[1].Width = 1.19f;
            wordTable.Columns[2].Width = 9.68f;
            wordTable.Columns[3].Width = 6.61f;

            int rowNumber = 2;
            for (int i = 0; i < site_list.Count; i++)
            {
                wordTable.Cell(rowNumber, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(rowNumber, 2).Range.Text = $"{site_list[i].Name}";
                wordTable.Cell(rowNumber, 3).Range.Text = $"{site_list[i].Link}";
                //форматирование
                wordTable.Cell(rowNumber, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(rowNumber, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                rowNumber++;
            }
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
        }

        private void CreateSoftwareInfoTable() //10
        {
            List<GeneralModel.SoftwareInfo> software_list = generalModel.SoftwareInfos;

            int row = 1 + software_list.Count;
            int column = 4;

            app.Selection.Find.Execute("<TABLE13>");
            Word.Range wordTableRange = app.Selection.Range;
            Word.Table wordTable = wordDocument.Tables.Add(wordTableRange, row, column);

            // форматирование
            wordTable.Borders.Enable = Convert.ToInt32(true);
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Rows[1].Range.Bold = Convert.ToInt32(true);
            wordTable.Range.Font.Size = 12;
            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Наименование программного обеспечения";
            wordTable.Cell(1, 3).Range.Text = "Лицензия";
            wordTable.Cell(1, 4).Range.Text = "Договор";

            wordTable.Columns[1].Width = 1.19f;
            wordTable.Columns[2].Width = 6.84f;
            wordTable.Columns[3].Width = 5.37f;
            wordTable.Columns[4].Width = 4.09f;

            int current_row = 2;
            for (int i = 0; i < software_list.Count; i++)
            {
                wordTable.Cell(current_row, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(current_row, 2).Range.Text = $"{software_list[i].Name}";
                wordTable.Cell(current_row, 3).Range.Text = $"{software_list[i].License}";
                wordTable.Cell(current_row, 4).Range.Text = $"{software_list[i].Agreement}";

                // форматирование
                wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(current_row, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(current_row, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                current_row++;
            }
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
        }


        //11 материально техническая база
        private void CreateMaterialTechnicalBaseTable()
        {
            int row = 1 + generalModel.PlaceTheirEquipments.Count;
            int column = 3;
            app.Selection.Find.Execute("<TABLE14>");
            Word.Range wordTableRange = app.Selection.Range;
            Word.Table wordTable = wordDocument.Tables.Add(wordTableRange, row, column);
            //форматирование
            wordTable.Borders.Enable = Convert.ToInt32(true);
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            wordTable.Range.Font.Size = 12;
            wordTable.Rows[1].Range.Bold = Convert.ToInt32(true);
            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Наименование специальных* помещений и помещений для самостоятельной работы";
            wordTable.Cell(1, 3).Range.Text = "Оснащенность специальных помещений и помещений для самостоятельной работы";

            wordTable.Columns[1].Width = 1.19f;
            wordTable.Columns[2].Width = 7.15f;
            wordTable.Columns[3].Width = 9.15f;

            int rowNumber = 2;
            for (int i = 0; i < generalModel.PlaceTheirEquipments.Count; i++)
            {
                wordTable.Cell(rowNumber, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(rowNumber, 2).Range.Text = $"{generalModel.PlaceTheirEquipments[i].PlaceName}";
                string equipment = "";
                for (int j = 0; j < generalModel.PlaceTheirEquipments[i].EquipmentsName.Count; j++)
                {
                    if (generalModel.PlaceTheirEquipments[i].EquipmentsName.Count == 1)
                        equipment = generalModel.PlaceTheirEquipments[i].EquipmentsName[j];
                    else
                        equipment = equipment + $"{j + 1}. {generalModel.PlaceTheirEquipments[i].EquipmentsName[j]}\n";
                }
                wordTable.Cell(rowNumber, 3).Range.Text = equipment;
                //форматирование
                wordTable.Cell(rowNumber, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(rowNumber, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                rowNumber++;
            }
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
        }
    }
}
