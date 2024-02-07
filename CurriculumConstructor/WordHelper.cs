using CurriculumConstructor;
using CurriculumConstructor.SettingMenu.Model;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static CurriculumConstructor.SettingMenu.Model.GeneralModel;
using Word = Microsoft.Office.Interop.Word;

namespace TestWord
{
    class WordHelper
    {
        private FileInfo _fileInfo;
        private Word.Application app;
        private Word._Document wordDocument;
        private bool IsClosedWordDocument = true;

        // Data
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
                if(!IsClosedWordDocument)
                    app.ActiveDocument.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка! " + ex.Message + "\n" +
                        ex.StackTrace + "\n" +
                        ex.TargetSite + "\n" +
                        ex.HelpLink);
                Console.WriteLine(ex.Message);
            }

            try
            {
                if (app is not null)
                {
                    app.Quit();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка! " + ex.Message + "\n" +
                        ex.StackTrace + "\n" +
                        ex.TargetSite + "\n" +
                        ex.HelpLink);
                Console.WriteLine(ex.Message);
            }
        }

        internal bool Process(bool forPreview, string filePathForSave = "")
        {
            try
            {
                Object file = _fileInfo.FullName;

                app = new Word.Application();
                wordDocument = app.Documents.Open(file, ReadOnly: false);

                IsClosedWordDocument = false;

                replaceText(new Dictionary<string, string>());

                CreateAcquiredCompetenciesAsDisciplineMasteringResultTable(); // 1.; Annotation
                CreateDisciplineThematicPlanTable(); // 4.1.
                CreateDisciplineContentTable(); // 4.2.

                createTable5(); // 6.1.

                CreateAcquiredCompetenciesWithEvaluationCriteriesTable(); // 6.2.

                CreateCompetenciesFormingLevelEvaluationTestTasksTable(); // 6.3.1.2.

                PutTextToPracticeAndLabEvaluationCriteries(); // 6.3.2.3, 6.3.3.3

                CreateTextForExamTypes(); // 6.3.4; 6.3.5; 6.3.6.


                CreateRatingPointsDisctributionByDisciplineTables(); // 6.4.

                CreateEducationLiteratureTable(); // 7.
                CreateProffectionalBasesTable(); // 8.
                CreateSoftwareInfoTable(); // 10.
                CreateMaterialTechnicalBaseTable(); // 11.

                CreateAcquiredCompetenciesAsDisciplineMasteringResultTable(); // Annotation again

                FillingAnnotationTable(); /// Annotation again (exists table, about discipline)


                if (forPreview)
                    PreviewView(filePathForSave);
                else
                    saveWord();

                app.ActiveDocument.Close(Word.WdSaveOptions.wdDoNotSaveChanges);

                IsClosedWordDocument = true;

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка! " + ex.Message + "\n" +
                    ex.StackTrace + "\n" +
                    ex.TargetSite + "\n" +
                    ex.HelpLink);
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        internal void PreviewView(string filePathForTempSave)
        {
            app.ActiveDocument.SaveAs2(filePathForTempSave);
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
            if (items.Count <= 0)
            {
                List<Semester> semesters = generalModel.Semesters;
                List<string> attestationList = new List<string>();

                if (generalModel.IsOffset)
                    attestationList.Add("зачёта");
                if (generalModel.IsOffsetWithMark)
                    attestationList.Add("зачёта с оценкой");
                if (generalModel.IsExam)
                    attestationList.Add("экзамена");
                
                items = new Dictionary<string, string>
                {
                    //EXCEL ИЛИ ПРОГРАММНО РАССЧИТАТЬ
                    //0-1
                    {"<YEAR>", DateTime.Now.Year.ToString() },
                    {"<INDEX>", generalModel.Index },
                    {"<DISCIPLINE>", generalModel.DisciplineName }, //6, 6.3.1.1 (ЛАБЫ), 6.4
                    {"<DISCIPLINE_UP_CASE>", generalModel.DisciplineName.ToUpper()}, // 11, аннотация
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
                    {"<LECTURE_HOURS>", semesters.Sum(semester => Convert.ToInt32(semester.Lectures)).ToString()},
                    {"<PRACTICE_HOURS>" , semesters.Sum(semester => Convert.ToInt32(semester.PracticeWorks)).ToString()},
                    {"<LABORATORY_HOURS>" , semesters.Sum(semester => Convert.ToInt32(semester.LaboratoryWorks)).ToString()},
                    {"<INDEPENDENT_HOURS>" , semesters.Sum(semester => Convert.ToInt32(semester.IndependentWork)).ToString()},
                    {"<CONTROL_HOURS>" ,semesters.Sum(semester => Convert.ToInt32(semester.Control)).ToString()},
                    {"<ATTESTATION_LIST>", string.Join(", ", semesters.Select(x => x.SemesterNumber).Select(x =>
                        (generalModel.OffsetSemesterNumbers.Contains(x) ? "зачёт в "
                        : generalModel.OffsetWithMarkSemesterNumbers.Contains(x) ? "зачёт с оценкой в "
                        : generalModel.ExamSemesterNumbers.Contains(x) ? "экзамен в "
                        : "неизвестное в ")
                        + x.ToString() + " семестре"
                        ))
                    },
                    {"<ATTESTATION_LIST_WITH_COURSEWORK>", string.Join(", ", attestationList)
                        + (generalModel.CourseworkSemesters.Length > 0 ? ((generalModel.IsOffset || generalModel.IsOffsetWithMark || generalModel.IsExam ? " и " : "") + "курсовой работы") : "")},

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

        private Word.Range FindByTag(string tag, bool forward = true)
        {
            Object missing = Type.Missing;

            app.Selection.Find.Execute(tag,
                    MatchCase: false,
                    MatchWholeWord: false,
                    MatchWildcards: false,
                    MatchSoundsLike: missing,
                    MatchAllWordForms: false,
                    Forward: forward);

            return app.Selection.Range;
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
                    Forward: false,
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
            var semesterNumbers = generalModel.Semesters.Select(x => x.SemesterNumber).OrderBy(x => x).ToArray();
            bool semesterMoreThan1 = semesterNumbers.Length > 1;

            {
                var items = generalModel.DisciplineThematicPlan.Select(x => new { semesterNumber = x.Key.SemesterNumber, x.Value.DisciplineThematicPlan });

                foreach (var item in items)
                {
                    themes.AddRange(item.DisciplineThematicPlan.Select(x => (item.semesterNumber, x)));
                }
            }

            Word.Range tableRange = FindByTag("<DISCIPLINE_THEMATIC_PLAN_TABLE>");

            var wordTable = wordDocument.Tables.Add(tableRange,
                2 + themes.Count +
                (semesterMoreThan1 ? generalModel.Semesters.Count : 0) + 1 // totals of each semester if they more than 1 and total of all
                , 7
            );

            wordTable.Range.Font.Size = 12;
            wordTable.Borders.Enable = 1;

            // Entering and formating columns rows
            wordTable.Rows[1].Range.Bold = 1;
            wordTable.Rows[2].Range.Bold = 1;


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


            wordTable.Cell(1, 1).Range.Text = "№";
            wordTable.Cell(1, 2).Range.Text = "Темы дисциплины";
            wordTable.Cell(1, 3).Range.Text = "семестр";
            wordTable.Cell(1, 4).Range.Text = "Виды и часы "
                + "контактной \nработы, \nих трудоемкость \n(в часах)";
            wordTable.Cell(2, 4).Range.Text = "Лекции";
            wordTable.Cell(2, 5).Range.Text = "Практические \nзанятия";
            wordTable.Cell(2, 6).Range.Text = "Лабораторные \nзанятия";
            wordTable.Cell(1, 7).Range.Text = "СРС";


            //форматирование таблицы
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;


            // Columns widthing
            wordTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
            wordTable.Columns[1].PreferredWidth = app.MillimetersToPoints(8.1f);
            wordTable.Columns[2].PreferredWidth = app.MillimetersToPoints(103.8f);
            wordTable.Columns[3].PreferredWidth = app.MillimetersToPoints(8.8f);
            wordTable.Columns[4].PreferredWidth = app.MillimetersToPoints(12.8f);
            wordTable.Columns[5].PreferredWidth = app.MillimetersToPoints(12.3f);
            wordTable.Columns[6].PreferredWidth = app.MillimetersToPoints(11.1f);
            wordTable.Columns[7].PreferredWidth = app.MillimetersToPoints(10.7f);

            int currentTableRow = 3;

            foreach(var semesterNumber in semesterNumbers)
            {
                var semesterThemes = themes.Where(x => x.semesterNumber == semesterNumber).Select(x => x.theme).ToList();
                int semesterTotalLectureHous = 0, semesterTotalPracticeHous = 0, semesterTotalLabWordHous = 0, semesterTotalIndependentHous = 0;

                if (semesterThemes.Count > 0)
                {
                    int themeNumber = 1;

                    foreach(var semesterTheme in semesterThemes)
                    {
                        wordTable.Cell(currentTableRow, 1).Range.Text = (themeNumber + 1).ToString();
                        wordTable.Cell(currentTableRow, 2).Range.Text = semesterTheme.ThemeName;
                        wordTable.Cell(currentTableRow, 3).Range.Text = semesterNumber.ToString();
                        wordTable.Cell(currentTableRow, 4).Range.Text = semesterTheme.LectureHours != 0 ? semesterTheme.LectureHours.ToString() : "-";
                        wordTable.Cell(currentTableRow, 5).Range.Text = semesterTheme.PracticeHours != 0 ? semesterTheme.PracticeHours.ToString() : "-";
                        wordTable.Cell(currentTableRow, 6).Range.Text = semesterTheme.LaboratoryWorkHours != 0 ? semesterTheme.LaboratoryWorkHours.ToString() : "-";
                        wordTable.Cell(currentTableRow, 7).Range.Text = semesterTheme.IndependentHours != 0 ? semesterTheme.IndependentHours.ToString() : "-";

                        //выравнивание=слева
                        wordTable.Cell(3 + currentTableRow, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                        themeNumber++;
                        currentTableRow++;
                    }
                }

                if (semesterMoreThan1)
                {
                    wordTable.Cell(currentTableRow, 2).Range.Text = "Итого в " + semesterNumber.ToString() + " семестре";
                    wordTable.Cell(currentTableRow, 4).Range.Text = semesterTotalLectureHous.ToString();
                    wordTable.Cell(currentTableRow, 5).Range.Text = semesterTotalPracticeHous.ToString();
                    wordTable.Cell(currentTableRow, 6).Range.Text = semesterTotalLabWordHous.ToString();
                    wordTable.Cell(currentTableRow, 7).Range.Text = semesterTotalIndependentHous.ToString();

                    wordTable.Rows[currentTableRow].Range.Bold = 1;

                    currentTableRow++;
                }
            }

            // Last row
            wordTable.Cell(currentTableRow, 1).Range.Text = "";
            wordTable.Cell(currentTableRow, 2).Range.Text = "Итого по дисциплине";
            wordTable.Cell(currentTableRow, 3).Range.Text = "";
            wordTable.Cell(currentTableRow, 4).Range.Text = generalModel.NeedTotalLectureHours.ToString();
            wordTable.Cell(currentTableRow, 5).Range.Text = generalModel.NeedTotalPracticeHours.ToString();
            wordTable.Cell(currentTableRow, 6).Range.Text = generalModel.NeedTotalLaboratoryWorkHours.ToString();
            wordTable.Cell(currentTableRow, 7).Range.Text = generalModel.NeedTotalIndependentHours.ToString();

            wordTable.Rows[currentTableRow].Range.Bold = 1;


            // Merging
            wordTable.Cell(1, 7).Merge(wordTable.Cell(2, 7));

            wordTable.Cell(1, 4).Merge(wordTable.Cell(1, 6));

            wordTable.Cell(1, 3).Merge(wordTable.Cell(2, 3));
            wordTable.Cell(1, 2).Merge(wordTable.Cell(2, 2));
            wordTable.Cell(1, 1).Merge(wordTable.Cell(2, 1));
        }


        private void CreateAcquiredCompetenciesAsDisciplineMasteringResultTable()
        {
            //данные - код, имя, знать, уметь, владеть, индикаторы
            List<CompetencyPlanningResult> competences = generalModel.competencyPlanningResults;

            Word.Range wordRange = FindByTag("<ACQUIRED_COMPETENCIES_AS_DISCIPLINE_MASTERING_RESULT_TABLE>");

            var wordTable = wordDocument.Tables.Add(wordRange,
                competences.Count + 1, 4); // Rows: Competencies count + row for attribute names

            wordTable.Range.Font.Size = 12;
            wordTable.Borders.Enable = 1;

            // Entering data from model
            wordTable.Cell(1, 1).Range.Text = "Оцениваемые компетенции (код, наименование)";
            wordTable.Cell(1, 2).Range.Text = "Код и наименование индикатора (индикаторов) достижения компетенции";
            wordTable.Cell(1, 3).Range.Text = "Результаты освоения компетенции";
            wordTable.Cell(1, 4).Range.Text = "Оценочные средства текущего контроля и промежуточной аттестации";

            wordTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
            wordTable.Columns[1].PreferredWidth = app.MillimetersToPoints(42.4f);
            wordTable.Columns[2].PreferredWidth = app.MillimetersToPoints(61.8f);
            wordTable.Columns[3].PreferredWidth = app.MillimetersToPoints(34.6f);
            wordTable.Columns[4].PreferredWidth = app.MillimetersToPoints(35.0f);

            var semesterNumbers_attestType = generalModel.OffsetSemesterNumbers.Select(x => (x, 0)).Union(
                    generalModel.OffsetWithMarkSemesterNumbers.Select(x => (x, 1))).Union(
                    generalModel.ExamSemesterNumbers.Select(x => (x, 2))).OrderBy(x => x.x).ToArray();
            

            for (int i = 0; i < competences.Count; i++)
            {
                int currentTableRow = 2 + i;
                var row = competences[i];
                Dictionary<int, string> childs = row.CompetencyAchivmentIndicators;

                // Column 1
                Word.Range range = wordTable.Cell(currentTableRow, 1).Range;
                range.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range.InsertAfter(row.Code);
                range.Font.Bold = 1;
                range.InsertParagraphAfter();
                range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                range.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range.InsertAfter(generalModel.competencyCode_Names.First(x => x.Code == row.Code).CodeName);
                range.Font.Bold = 0;
                range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                // Column 2
                Word.Range range2 = wordTable.Cell(currentTableRow, 2).Range;
                int childIndex = 0;

                foreach (var keyValuePair in childs)
                {
                    string childCode = row.Code + "." + keyValuePair.Key + ".";
                    string childName = keyValuePair.Value;

                    range2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range2.InsertAfter(childCode);
                    range2.Font.Bold = 1;
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

                    range2.Font.Bold = 0;
                    range2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                    childIndex++;
                }

                // Column 3
                Word.Range range3 = wordTable.Cell(currentTableRow, 3).Range;

                // To know
                range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range3.InsertAfter("Знать:");
                range3.Font.Bold = 1;
                range3.InsertParagraphAfter();
                range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                for (int toKnowIndex = 0; toKnowIndex < row.ToKnowResult.Count; toKnowIndex++)
                {
                    string know = row.ToKnowResult[toKnowIndex];

                    range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range3.InsertAfter("- " + know.ToLower() + (toKnowIndex < row.ToKnowResult.Count - 1 ? ";" : "."));
                    range3.Font.Bold = 0;
                    range3.InsertParagraphAfter();
                    range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                }

                // To able
                range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range3.InsertAfter("Уметь:");
                range3.Font.Bold = 1;
                range3.InsertParagraphAfter();
                range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                for (int toAbleIndex = 0; toAbleIndex < row.ToAbilityResult.Count; toAbleIndex++)
                {
                    string able = row.ToAbilityResult[toAbleIndex];

                    range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range3.InsertAfter("- " + able.ToLower() + (toAbleIndex < row.ToAbilityResult.Count - 1 ? ";" : "."));
                    range3.Font.Bold = 0;
                    range3.InsertParagraphAfter();
                    range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                }

                // To own
                range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range3.InsertAfter("Владеть:");
                range3.Font.Bold = 1;
                range3.InsertParagraphAfter();
                range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                for (int toOwnIndex = 0; toOwnIndex < row.ToOwnResult.Count; toOwnIndex++)
                {
                    string own = row.ToOwnResult[toOwnIndex];

                    range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range3.InsertAfter("- " + own.ToLower() + (toOwnIndex < row.ToOwnResult.Count - 1 ? ";" : "."));
                    range3.Font.Bold = 0;
                    range3.InsertParagraphAfter();
                    range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                }


                // Column 4
                Word.Range range4 = wordTable.Cell(currentTableRow, 4).Range;

                //текущий контроль
                range4.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4.InsertAfter("Текущий контроль:");
                range4.Font.Bold = 1;
                range4.InsertParagraphAfter();
                range4.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                range4.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4.InsertAfter("Компьютерное тестирование по теме 1-5\nПрактические задачи по темам 1-5\nЛабораторные работыпо темам 1-3"); //!!!!!
                range4.Font.Bold = 0;
                range4.InsertParagraphAfter();
                range4.InsertParagraphAfter();
                range4.InsertParagraphAfter();
                range4.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                //промежуточная аттестация
                range4.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4.InsertAfter("Промежуточная аттестация:");
                range4.Font.Bold = 1;
                range4.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                foreach (var semesterNumber_attestType in semesterNumbers_attestType)
                {
                    Word.Range rangeForSemAtt = range4;

                    rangeForSemAtt.InsertAfter("\n");
                    rangeForSemAtt.Collapse(WdCollapseDirection.wdCollapseEnd);
                    
                    rangeForSemAtt.InsertAfter(semesterNumber_attestType.x.ToString() + " семестр – ");

                    rangeForSemAtt.Font.Bold = 1;
                    rangeForSemAtt.Collapse(WdCollapseDirection.wdCollapseEnd);

                    rangeForSemAtt.InsertAfter((semesterNumber_attestType.Item2 == 0 ? "зачет"
                           : semesterNumber_attestType.Item2 == 1 ? "зачет с оценкой"
                           : "экзамен"));

                    rangeForSemAtt.Font.Bold = 0;
                }
            }

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

            wordTable.Cell(1, 1).Range.Bold = 1;
            wordTable.Cell(1, 2).Range.Bold = 1;
            wordTable.Cell(1, 3).Range.Bold = 1;
            wordTable.Cell(1, 4).Range.Bold = 1;
        }

        // 4.2.
        private void CreateDisciplineContentTable()
        {
            var disciplineThematicPlan = generalModel.DisciplineThematicPlan;

            string main_key = "<DISCIPLINE_CONTENT_TABLE>";

            Word.Range wordRange = FindByTag(main_key);

            int rowsCount = 1  // Columns row
                    + disciplineThematicPlan.Count // Discipline module rows
                    + disciplineThematicPlan.Sum(x => x.Value.DisciplineThematicPlan.Count // Themes count
                    + disciplineThematicPlan.Sum(x => x.Value.DisciplineThematicPlan.Sum(x => x.ThemeContents.Count))); // Lectures + practices + laboratories count

            var wordTable = wordDocument.Tables.Add(app.Selection.Range,
                    rowsCount, 4);

            wordTable.Range.Font.Size = 12;
            wordTable.Borders.Enable = 1;

            wordTable.Cell(1, 1).Range.Text = "Тема";
            wordTable.Cell(1, 2).Range.Text = "Кол-во часов";
            wordTable.Cell(1, 3).Range.Text = "Используемый метод";
            wordTable.Cell(1, 4).Range.Text = "Формируемые компетенции";

            wordTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
            wordTable.Columns[1].PreferredWidth = app.MillimetersToPoints(101.2f);
            wordTable.Columns[2].PreferredWidth = app.MillimetersToPoints(9.8f);
            wordTable.Columns[3].PreferredWidth = app.MillimetersToPoints(29.3f);
            wordTable.Columns[4].PreferredWidth = app.MillimetersToPoints(28.7f);

            wordTable.Rows[1].Range.Bold = 1;

            int lectureNumber = 1,
                laboratoryNumber = 1,
                practiceNumber = 1,
                themeNumber = 1;
            int rowNumber = 2;

            foreach (var disciplineModule in disciplineThematicPlan)
            {
                var themes = disciplineModule.Value.DisciplineThematicPlan;

                wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 4));

                wordTable.Rows[rowNumber].Range.Bold = 1;

                wordTable.Cell(rowNumber, 1).Range.Text = "Дисциплинарный модуль " + disciplineModule.Key.SemesterNumber + "." + disciplineModule.Key.SemesterModuleNumber;

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
                        rangeColumn1.Font.Italic = 0;
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
                        rangeColumn1.Font.Italic = 0;
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
                        rangeColumn1.Font.Italic = 0;
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

        // 6.1
        private void createTable5()
        {
            Word.Range wordRange = FindByTag("<TABLE5>");
            var wordTable = wordDocument.Tables.Add(wordRange,
                1 + 2 + 3 + ((generalModel.IsExam ? 1 : 0) + (generalModel.IsOffsetWithMark ? 1 : 0) + (generalModel.IsOffset ? 1 : 0)), 4);

            wordTable.Cell(1, 1).Range.Text = "Этапы формирования компетенции";
            wordTable.Cell(1, 2).Range.Text = "Вид оценочного средства";
            wordTable.Cell(1, 3).Range.Text = "Краткая характеристика оценочного средства";
            wordTable.Cell(1, 4).Range.Text = "Представление оценочного средства в фонде";

            wordTable.Cell(2, 1).Range.Text = "Текущий контроль";


            // 
            wordTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
            wordTable.Columns[1].PreferredWidth = app.MillimetersToPoints(26.9f);
            wordTable.Columns[2].PreferredWidth = app.MillimetersToPoints(30.0f);
            wordTable.Columns[3].PreferredWidth = app.MillimetersToPoints(85.0f);
            wordTable.Columns[4].PreferredWidth = app.MillimetersToPoints(31.9f);


            // Форматирование
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Cell(2, 1).Range.Bold = 1;
            wordTable.Cell(2, 1).Merge(wordTable.Cell(2, 4));

            Dictionary<string, string> keyValuesForReplace = new Dictionary<string, string>();


            List<((string tag, string name) name_tag, (string tag, string description) description_tag, (string tag, string path) path_tag)> currentControlStrings = new List<((string tag, string name), (string tag, string description), (string tag, string path))>()
            {
                (("<_LAB_NAME>", "Лабораторная работа"), ("<_LAB_DESCR>", "Может выполняться в индивидуальном порядке или группой обучающихся. Задания в лабораторных работах должны включать элемент командной работы. Позволяет оценить умения, обучающихся самостоятельно конструировать свои знания в процессе решения практических задач и оценить уровень сформированности аналитических, исследовательских навыков, а также навыков практического мышления. Позволяет оценить способность к профессиональным трудовым действиям."), ("<_LAB_PATH>", "Темы, задания для выполнения лабораторных работ; вопросы к их защите")),
                (("<_PRACT_NAME>", "Практическая задача"), ("<_PRACT_DESCR>", "Средство оценки умения применять полученные теоретические знания в практической ситуации. Задача должна быть направлена на оценивание тех компетенций, которые подлежат освоению в данной дисциплине, должна содержать четкую инструкцию по выполнению или алгоритм действий."), ("<_PRACT_PATH>", "Комплект задач")),
                (("<_TEST_NAME>", "Тестирование компьютерное"), ("<_TEST_DESCR>", "Система стандартизированных заданий, позволяющая автоматизировать процедуру измерения уровня знаний и умений, обучающегося по соответствующим компетенциям. Обработка результатов тестирования на компьютере обеспечивается специальными программами. Позволяет проводить самоконтроль (репетиционное тестирование), может выступать в роли тренажера при подготовке к зачету или экзамену."), ("<_TEST_PATH>", "Фонд тестовых заданий"))
            };

            int currentRow = 3;
            int stage = 1;
            foreach (var control in currentControlStrings)
            {
                wordTable.Cell(currentRow, 1).Range.Text = stage.ToString();
                wordTable.Cell(currentRow, 2).Range.Text = control.name_tag.name;
                wordTable.Cell(currentRow, 3).Range.Text = control.description_tag.description;
                wordTable.Cell(currentRow, 4).Range.Text = control.path_tag.path;

                keyValuesForReplace.Add(control.name_tag.tag, control.name_tag.name);
                keyValuesForReplace.Add(control.description_tag.tag, control.description_tag.description);
                keyValuesForReplace.Add(control.path_tag.tag, control.path_tag.path);

                // Форматирование
                wordTable.Cell(currentRow, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(currentRow, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                wordTable.Cell(currentRow, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                
                // Переход на новую строку
                stage++;
                currentRow++;
            }

            wordTable.Cell(currentRow, 1).Range.Text = "Промежуточная аттестация";
            wordTable.Cell(currentRow, 1).Range.Bold = 1;
            wordTable.Cell(currentRow, 1).Merge(wordTable.Cell(currentRow, 4));

            currentRow++;

            List<((string tag, string name) name_tag, (string tag, string description) description_tag, (string tag, string path) path_tag, bool descriptionToPath)> attestationsStrings = 
                new List<((string tag, string name) name_tag, (string tag, string description) description_tag, (string tag, string path) path_tag, bool descriptionToPath)>();

            if (generalModel.IsOffset)
                attestationsStrings.Add((("<_OFFSET_NAME>", "Зачет"), ("<_OFFSET_DESCR>", ""), ("<_OFFSET_PATH>", ""), false));
            if (generalModel.IsOffsetWithMark)
                attestationsStrings.Add((("<_OFF_MARK_NAME>", "Зачёт с оценкой"), ("<_OFF_MARK_DESCR>", "Итоговая форма определения степени достижения запланированных результатов обучения (оценивания уровня освоения компетенций). Зачет с оценкой  выставляется по результатам текущей работы в семестре без дополнительного опроса."), ("<_OFF_MARK_PATH>", ""), true)); 
            if (generalModel.IsExam)
                attestationsStrings.Add((("<_EXAM_NAME>", "Экзамен"), ("<_EXAM_DESCR>", "Итоговая форма определения степени достижения запланированных результатов обучения (оценивания уровня освоения компетенций). Экзамен нацелен на комплексную проверку освоения дисциплины. Экзамен проводится в форме тестирования по всем темам дисциплины."), ("<_EXAM_PATH>", "Перечень вопросов, фонд тестового задания"), false));

            foreach (var attestation in attestationsStrings)
            {
                wordTable.Cell(currentRow, 1).Range.Text = stage.ToString();
                wordTable.Cell(currentRow, 2).Range.Text = attestation.name_tag.name;
                wordTable.Cell(currentRow, 3).Range.Text = attestation.description_tag.description;

                keyValuesForReplace.Add(attestation.name_tag.tag, attestation.name_tag.name);
                keyValuesForReplace.Add(attestation.description_tag.tag, attestation.description_tag.description);

                if (!attestation.descriptionToPath)
                {
                    wordTable.Cell(currentRow, 4).Range.Text = attestation.path_tag.path;
                    wordTable.Cell(currentRow, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                
                    keyValuesForReplace.Add(attestation.path_tag.tag, attestation.path_tag.path);
                }
                else
                    wordTable.Cell(currentRow, 3).Merge(wordTable.Cell(currentRow, 4));

                // Форматирование
                wordTable.Cell(currentRow, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(currentRow, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                
                // Переход на новую строку
                stage++;
                currentRow++;
            }

            // Форматирование
            wordTable.Borders.Enable = 1;
            wordTable.Range.Font.Size = 12;
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);

            // Replace tags to text
            foreach(var keyValuePair in keyValuesForReplace)
            {
                ReplaceTextToTag(keyValuePair.Key, keyValuePair.Value);
            }
        }


        // 6.2
        private void CreateAcquiredCompetenciesWithEvaluationCriteriesTable()
        {
            //данные - код, имя, знать, уметь, владеть, индикаторы
            List<CompetencyPlanningResult> competences = generalModel.competencyPlanningResults;

            Word.Range wordRange = FindByTag("<ACQUIRED_COMPETENCIES_WITH_EVALUATION_CRITERIES_TABLE>");

            wordRange.Font.Size = 11;

            var wordTable = wordDocument.Tables.Add(wordRange,
                competences.Count * 3 + 1 + 3, 8); // Rows: Competencies count + row for attribute names

            wordTable.Range.Font.Size = 12;

            // Entering data from model
            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Оцениваемые компетенции (код, наименование)";
            wordTable.Cell(1, 3).Range.Text = "Код и наименование индикатора (индикаторов) достижения компетенции";
            wordTable.Cell(1, 4).Range.Text = "Планируемые\nрезультаты\nобучения";
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

            // Entering data
            for (int i = 0; i < competences.Count; i++)
            {
                int rowNumber = 5 + i*3;
                var row = competences[i];
                Dictionary<int, string> childs = row.CompetencyAchivmentIndicators;

                // Column 1
                Word.Range range1 = wordTable.Cell(rowNumber, 1).Range;
                range1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range1.InsertAfter((i + 1).ToString());
                range1.Font.Bold = 0;
                range1.InsertParagraphAfter();
                range1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber + 2, 1));

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

                wordTable.Cell(rowNumber, 2).Merge(wordTable.Cell(rowNumber + 2, 2));

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
                        range3.InsertAfter(" " + childName);
                    }
                    else
                    {
                        range3.InsertAfter(" " + childName + ";");
                        range3.InsertParagraphAfter();
                    }

                    range3.Font.Bold = 0;
                    range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                    childIndex++;
                }

                wordTable.Cell(rowNumber, 3).Merge(wordTable.Cell(rowNumber + 2, 3));

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
                    range4_1.InsertAfter("- " + know.ToLower() + (toKnowIndex < row.ToKnowResult.Count - 1 ? ";" : "."));
                    range4_1.Font.Bold = 0;
                    range4_1.InsertParagraphAfter();
                    range4_1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                }

                // To able
                Word.Range range4_2 = wordTable.Cell(rowNumber + 1, 4).Range;

                range4_2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4_2.InsertAfter("Уметь:");
                range4_2.Font.Bold = 1;
                range4_2.InsertParagraphAfter();
                range4_2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                for (int toAbleIndex = 0; toAbleIndex < row.ToAbilityResult.Count; toAbleIndex++)
                {
                    string able = row.ToAbilityResult[toAbleIndex];

                    range4_2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range4_2.InsertAfter("- " + able.ToLower() + (toAbleIndex < row.ToAbilityResult.Count - 1 ? ";" : "."));
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
                    range4_3.InsertAfter("- " + own.ToLower() + (toOwnIndex < row.ToOwnResult.Count - 1 ? ";" : "."));
                    range4_3.Font.Bold = 0;
                    range4_3.InsertParagraphAfter();
                    range4_3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                }

                // Column 5
                //Word.Range range5 
                Word.Range range5_1 = wordTable.Cell(rowNumber, 5).Range;

                range5_1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range5_1.InsertAfter(row.CompAchivMarkCriteriesToKnow.Excelent);
                range5_1.Font.Bold = 0;
                range5_1.InsertParagraphAfter();
                range5_1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                Word.Range range5_2 = wordTable.Cell(rowNumber + 1, 5).Range;

                range5_2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range5_2.InsertAfter(row.CompAchivMarkCriteriesToAble.Excelent);
                range5_2.Font.Bold = 0;
                range5_2.InsertParagraphAfter();
                range5_2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                Word.Range range5_3 = wordTable.Cell(rowNumber + 2, 5).Range;

                range5_3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range5_3.InsertAfter(row.CompAchivMarkCriteriesToOwn.Excelent);
                range5_3.Font.Bold = 0;
                range5_3.InsertParagraphAfter();
                range5_3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);


                // Column 6
                Word.Range range6_1 = wordTable.Cell(rowNumber, 6).Range;

                range6_1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range6_1.InsertAfter(row.CompAchivMarkCriteriesToKnow.Good);
                range6_1.Font.Bold = 0;
                range6_1.InsertParagraphAfter();
                range6_1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                Word.Range range6_2 = wordTable.Cell(rowNumber + 1, 6).Range;

                range6_2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range6_2.InsertAfter(row.CompAchivMarkCriteriesToAble.Good);
                range6_2.Font.Bold = 0;
                range6_2.InsertParagraphAfter();
                range6_2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                Word.Range range6_3 = wordTable.Cell(rowNumber + 2, 6).Range;

                range6_3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range6_3.InsertAfter(row.CompAchivMarkCriteriesToOwn.Good);
                range6_3.Font.Bold = 0;
                range6_3.InsertParagraphAfter();
                range6_3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);


                // Column 7
                Word.Range range7_1 = wordTable.Cell(rowNumber, 7).Range;

                range7_1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range7_1.InsertAfter(row.CompAchivMarkCriteriesToKnow.Satisfactory);
                range7_1.Font.Bold = 0;
                range7_1.InsertParagraphAfter();
                range7_1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                Word.Range range7_2 = wordTable.Cell(rowNumber + 1, 7).Range;

                range7_2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range7_2.InsertAfter(row.CompAchivMarkCriteriesToAble.Satisfactory);
                range7_2.Font.Bold = 0;
                range7_2.InsertParagraphAfter();
                range7_2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                Word.Range range7_3 = wordTable.Cell(rowNumber + 2, 7).Range;

                range7_3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range7_3.InsertAfter(row.CompAchivMarkCriteriesToOwn.Satisfactory);
                range7_3.Font.Bold = 0;
                range7_3.InsertParagraphAfter();
                range7_3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);


                // Column 8
                Word.Range range8_1 = wordTable.Cell(rowNumber, 8).Range;

                range8_1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range8_1.InsertAfter(row.CompAchivMarkCriteriesToKnow.Unsatisfactory);
                range8_1.Font.Bold = 0;
                range8_1.InsertParagraphAfter();
                range8_1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                Word.Range range8_2 = wordTable.Cell(rowNumber + 1, 8).Range;

                range8_2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range8_2.InsertAfter(row.CompAchivMarkCriteriesToAble.Unsatisfactory);
                range8_2.Font.Bold = 0;
                range8_2.InsertParagraphAfter();
                range8_2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                Word.Range range8_3 = wordTable.Cell(rowNumber + 2, 8).Range;

                range8_3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range8_3.InsertAfter(row.CompAchivMarkCriteriesToOwn.Unsatisfactory);
                range8_3.Font.Bold = 0;
                range8_3.InsertParagraphAfter();
                range8_3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            }


            wordTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
            wordTable.Columns[1].PreferredWidth = app.CentimetersToPoints(0.98f);
            wordTable.Columns[2].PreferredWidth = app.CentimetersToPoints(3.27f);
            wordTable.Columns[3].PreferredWidth = app.CentimetersToPoints(3.7f);
            wordTable.Columns[4].PreferredWidth = app.CentimetersToPoints(3.7f);
            wordTable.Columns[5].PreferredWidth = app.CentimetersToPoints(3.82f);
            wordTable.Columns[6].PreferredWidth = app.CentimetersToPoints(3.62f);
            wordTable.Columns[7].PreferredWidth = app.CentimetersToPoints(3.51f);
            wordTable.Columns[8].PreferredWidth = app.CentimetersToPoints(3.58f);

            wordTable.Cell(1, 1).Merge(wordTable.Cell(4, 1));
            wordTable.Cell(1, 2).Merge(wordTable.Cell(4, 2));
            wordTable.Cell(1, 3).Merge(wordTable.Cell(4, 3));
            wordTable.Cell(1, 4).Merge(wordTable.Cell(4, 4));

            wordTable.Cell(1, 5).Merge(wordTable.Cell(1, 8));

            wordTable.Cell(3, 5).Merge(wordTable.Cell(3, 8));
        }

        //6.3.1.2 Содержание оценочного средства
        private void CreateCompetenciesFormingLevelEvaluationTestTasksTable()
        {
            var testsByDisciplineModuleAndCompetencies = generalModel.TestTasksByDiscipModule;

            int rowCount = 2
                + testsByDisciplineModuleAndCompetencies.Count
                + testsByDisciplineModuleAndCompetencies.Sum(x => x.Value.competencyFormingTestTasks.Sum(x => x.Value.Count));

            Word.Range wordRange = FindByTag("<TEST_TASKS_TABLE>");
            Word.Table wordTable = wordDocument.Tables.Add(wordRange, rowCount, 6);

            wordTable.Range.Font.Size = 12;

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

            wordTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
            wordTable.Columns[1].PreferredWidth = app.CentimetersToPoints(1.97f);
            wordTable.Columns[2].PreferredWidth = app.CentimetersToPoints(2.9f);
            wordTable.Columns[3].PreferredWidth = app.CentimetersToPoints(3.23f);
            wordTable.Columns[4].PreferredWidth = app.CentimetersToPoints(3.1f);
            wordTable.Columns[5].PreferredWidth = app.CentimetersToPoints(3.05f);
            wordTable.Columns[6].PreferredWidth = app.CentimetersToPoints(3.24f);

            int rowNumber = 3;

            foreach (var testsByDisciplineModule in testsByDisciplineModuleAndCompetencies)
            {
                var testsByCompetencies = testsByDisciplineModule.Value.competencyFormingTestTasks;

                wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 6));
                wordTable.Cell(rowNumber, 1).Range.Bold = 1;

                wordTable.Cell(rowNumber, 1).Range.Text = "Дисциплинарный модуль "
                    + testsByDisciplineModule.Key.SemesterNumber + "."
                    + testsByDisciplineModule.Key.SemesterModuleNumber;

                if (testsByCompetencies.Count > 0)
                    rowNumber++;

                foreach (var keyValuePair in testsByCompetencies)
                {
                    string competencies = string.Join(", ", keyValuePair.Key);
                    var tests = keyValuePair.Value;

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

            // Merging
            wordTable.Cell(1, 1).Merge(wordTable.Cell(2, 1));
            wordTable.Cell(1, 2).Merge(wordTable.Cell(2, 2));
            wordTable.Cell(1, 3).Merge(wordTable.Cell(1, 6));
        }

        // 6.3.2.3, 6.3.3.3
        private void PutTextToPracticeAndLabEvaluationCriteries()
        {
            ReplaceTextToTag("<TAB_TASK_WITH_NUMBER>", generalModel.EvaluationCriteries.laboratory.LaboratoryTaskWithNumber);
            ReplaceTextToTag("<LAB_EXAMPLE_TASK>", generalModel.EvaluationCriteries.laboratory.TaskTextExampleForDefenceLabWork);

            {
                Word.Range rangeForQuestCompList = FindByTag("<QUESTION_COMPETENCY_CODE_LIST>");

                rangeForQuestCompList.Text = "";

                var questCodeList = generalModel.EvaluationCriteries.laboratory.QuestionsCodeExampleForDefenceLabWork;

                int questionNumber = 1;

                foreach (var questCode in questCodeList)
                {
                    rangeForQuestCompList.InsertAfter(questionNumber.ToString() + ". " + questCode.Question + "(" + questCode.CompetencyCode + ")");

                    if(questCode != questCodeList.Last())
                    {
                        rangeForQuestCompList.InsertAfter(Environment.NewLine);
                    }

                    questionNumber++;
                }
            }
            
            ReplaceTextToTag("<PRACTICE_TASK_COMPETENCY_CODE>", generalModel.EvaluationCriteries.practice.CompetencyCode);
            ReplaceTextToTag("<PRACTICE_TASK>", generalModel.EvaluationCriteries.practice.PracticeTask);

            {
                Word.Range rangeForPractTaskDescr = FindByTag("<PRACTICE_TASK_DESCRIPTION>");

                rangeForPractTaskDescr.Text = "";

                rangeForPractTaskDescr.Collapse(WdCollapseDirection.wdCollapseEnd);

                if (generalModel.EvaluationCriteries.practice.PracticeTaskDescription != "")
                {
                    rangeForPractTaskDescr.InsertAfter(Environment.NewLine);
                    rangeForPractTaskDescr.InsertAfter(generalModel.EvaluationCriteries.practice.PracticeTaskDescription);
                }
            }
        }

        // 6.3.4 - 6.3.6
        public void CreateTextForExamTypes()
        {
            ///
            var wordRange = FindByTag("<ATTESTATION_SECTIONS>");

            wordRange.Text = "";
            wordRange.Font.Size = 14;

            Dictionary<string, string> keyValuesForReplace = new Dictionary<string, string>();

            int subSubSectionNumber = 4; // Default for first
            object bigTextRangeStart = wordRange.Start, bigTextRangeEnd = 0;


            if (generalModel.IsOffset)
            {
                wordRange.InsertAfter("6.3." + subSubSectionNumber.ToString() + ". Зачет" + Environment.NewLine);
                wordRange.Font.Bold = 1;

                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                subSubSectionNumber++;
            }

            if (generalModel.IsOffsetWithMark)
            {
                wordRange.InsertAfter("6.3." + subSubSectionNumber.ToString() + ". Зачет с оценкой" + Environment.NewLine);
                wordRange.Font.Bold = 1;
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                wordRange.InsertAfter("6.3." + subSubSectionNumber.ToString() + ".1. Порядок проведения" + Environment.NewLine);
                wordRange.Font.Bold = 0;
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                string tag = "<_6.3." + subSubSectionNumber + ".1_TEXT>";
                wordRange.InsertAfter(tag + Environment.NewLine);
                keyValuesForReplace.Add(tag, "Зачет формируется по результатам текущего контроля, без дополнительного опроса, так как в течение семестра проводится необходимое количество контрольных мероприятий, которые в своей совокупности проверяют уровень сформированности соответствующих компетенций.");
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                wordRange.InsertAfter("6.3." + subSubSectionNumber.ToString() + ".2. Критерии оценивания" + Environment.NewLine);
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                tag = "<_6.3." + subSubSectionNumber + ".2_TEXT>";
                wordRange.InsertAfter(tag + Environment.NewLine);
                keyValuesForReplace.Add(tag, "Для получения зачета общая сумма баллов за контрольные мероприятия текущего контроля (с учетом поощрения обучающегося за участие в научной деятельности или особые успехи в изучении дисциплины) должна составлять от 55 до 100 баллов.");
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                subSubSectionNumber++;
            }


            if (generalModel.IsExam)
            {
                wordRange.InsertAfter("6.3." + subSubSectionNumber.ToString() + ". Экзамен" + Environment.NewLine);
                wordRange.Font.Bold = 1;
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                wordRange.InsertAfter("6.3." + subSubSectionNumber.ToString() + ".1. Порядок проведения" + Environment.NewLine);
                wordRange.Font.Bold = 0;
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                string tag = "<_6.3." + subSubSectionNumber.ToString() + ".1_SECOND_TEXT>";
                wordRange.InsertAfter("Экзамен проводится в форме компьютерного тестирования." + Environment.NewLine + tag + Environment.NewLine);
                keyValuesForReplace.Add(tag, "На экзамене, который проводится в форме компьютерного тестирования, студенту предоставляется блок тестовых заданий в количестве 30 шт., которые генерируются автоматической тестирующей системой персонально в случайном порядке и содержат вопросы по всему перечню тем дисциплины. Каждое правильно выполненное тестовое задание оценивается в 1 балл. Максимальное количество баллов, которое студент имеет возможность набрать – 40.");
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                wordRange.InsertAfter("6.3." + subSubSectionNumber.ToString() + ".2 Критерии оценивания" + Environment.NewLine);
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                /*
                tag = "<_6.3." + subSubSectionNumber.ToString() + ".2_TEXT>";
                bigTextRangeStart = wordRange.Start;
                wordRange.InsertAfter(tag + Environment.NewLine);
                keyValuesForReplace.Add(tag, "Баллы в интервале 86-100% от максимальных ставятся, если обучающийся:\r- демонстрирует продвинутый уровень владения знаниями, умениями и навыками соответствующих компетенций, что позволяет ему решать широкий круг типовых и нетиповых задач;\r\n- проявил высокую эрудицию и свободное владение материалом дисциплины;\r\n- дал ответы на вопросы четкие, обоснованные и полные, проявил готовность к дискуссии.\r\nБаллы в интервале 71-85% от максимальных ставятся, если обучающийся:\r\n- демонстрирует знания, умения, навыки, сформированные на среднем уровне соответствующих компетенций;\r\n- способен самостоятельно воспроизводить и применять соответствующие знания, умения и навыки для решения типовых задач дисциплины;\r\n- может выполнять поиск и использовать полученную информацию для выполнения новых профессиональных действий;\r\n- дал ответы на вопросы преимущественно правильные, но недостаточно четкие.\r\nБаллы в интервале 55-70% от максимальных ставятся, если обучающийся:\r\n- демонстрирует знания, умения, навыки, сформированные на базовом уровне соответствующих компетенций;\r\n- частично, с помощью извне (например, с использованием наводящих вопросов) может воспроизводить и применять соответствующие знания, умения, навыки;\r\n- дал ответы на вопросы не полные.\r\nБаллы в интервале 0-54% от максимальных ставятся, если обучающийся:\r\n- не ответил на большую часть вопросов;\r\n- демонстрирует полную некомпетентность в материале дисциплины, не способность самостоятельно, без помощи извне, воспроизводить и применять соответствующие знания, умения, навыки.\r\n");
                */

                wordRange.InsertAfter("Баллы в интервале 86-100% от максимальных ставятся, если обучающийся:\n");
                wordRange.InsertAfter("- демонстрирует продвинутый уровень владения знаниями, умениями и навыками соответствующих компетенций, что позволяет ему решать широкий круг типовых и нетиповых задач;\n");
                wordRange.InsertAfter("- проявил высокую эрудицию и свободное владение материалом дисциплины;\n");
                wordRange.InsertAfter("- дал ответы на вопросы четкие, обоснованные и полные, проявил готовность к дискуссии.\n");
                wordRange.InsertAfter("Баллы в интервале 71-85% от максимальных ставятся, если обучающийся: \n");
                wordRange.InsertAfter("- демонстрирует знания, умения, навыки, сформированные на среднем уровне соответствующих компетенций;\n");
                wordRange.InsertAfter("- способен самостоятельно воспроизводить и применять соответствующие знания, умения и навыки для решения типовых задач дисциплины;\n");
                wordRange.InsertAfter("- может выполнять поиск и использовать полученную информацию для выполнения новых профессиональных действий;\n");
                wordRange.InsertAfter("- дал ответы на вопросы преимущественно правильные, но недостаточно четкие.\n");
                wordRange.InsertAfter("Баллы в интервале 55-70% от максимальных ставятся, если обучающийся:\n");
                wordRange.InsertAfter("- демонстрирует знания, умения, навыки, сформированные на базовом уровне соответствующих компетенций;\n");
                wordRange.InsertAfter("- частично, с помощью извне (например, с использованием наводящих вопросов) может воспроизводить и применять соответствующие знания, умения, навыки;\n");
                wordRange.InsertAfter("- дал ответы на вопросы не полные.\n");
                wordRange.InsertAfter("Баллы в интервале 0-54% от максимальных ставятся, если обучающийся:\n");
                wordRange.InsertAfter("- не ответил на большую часть вопросов;\n");
                wordRange.InsertAfter("- демонстрирует полную некомпетентность в материале дисциплины, не способность самостоятельно, без помощи извне, воспроизводить и применять соответствующие знания, умения, навыки.\n");

                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                wordRange.InsertAfter("6.3." + subSubSectionNumber.ToString() + ".3 Содержание оценочного средства");
                wordRange.Font.Italic = 1;
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                wordRange.InsertAfter("<ASSESMENT_TOOLS_CONTENT_TABLE>" + Environment.NewLine + Environment.NewLine);
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                wordRange.InsertAfter("Примерные тестовые задания к экзамену" + Environment.NewLine);
                wordRange.Font.Bold = 1;
                wordRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                wordRange.InsertAfter("<EXAM_TEST_TASKS_VARIANT_TEMPLATE>");
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                wordRange.InsertAfter("Полный перечень оценочных средств текущего контроля и промежуточной аттестации по дисциплине представлен в Фонде оценочных средств (приложении 3 к данной рабочей программе)." + Environment.NewLine);
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);


                CreateAssesmentToolsContentTable(); // For exam (max 6.3.6)

                CreateExamTestTasksVariantTemplateTable();
            }

            foreach (var keyValuePair in keyValuesForReplace)
            {
                ReplaceTextToTag(keyValuePair.Key, keyValuePair.Value);
            }
        }

        //Экзамен 6.3.4.3. Содержание оценочного средства
        private void CreateAssesmentToolsContentTable()
        {
            var wordRange = FindByTag("<ASSESMENT_TOOLS_CONTENT_TABLE>");
            bool semesterCountMoreThan1 = generalModel.Semesters.Count > 1;

            int colomn_count = 2 + generalModel.DisciplineCompetencies.Count();
            int row_count = 1 + generalModel.SemesterQuestionCodes.Count + (semesterCountMoreThan1 ? generalModel.Semesters.Count : 0);

            Word.Table wordTable = wordDocument.Tables.Add(wordRange, row_count, colomn_count);

            wordTable.Range.Font.Size = 12;
            wordTable.Borders.Enable = 1;

            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            wordTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
            wordTable.Columns[1].PreferredWidth = app.MillimetersToPoints(11.5f);
            wordTable.Columns[2].PreferredWidth = app.MillimetersToPoints(130.2f);
            wordTable.Columns[3].PreferredWidth = app.MillimetersToPoints(22.5f);

            //заполнение
            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Примерные вопросы к экзамену";
            for (int i = 0; i < generalModel.DisciplineCompetencies.Count(); i++)
                wordTable.Cell(1, 3 + i).Range.Text = generalModel.DisciplineCompetencies[i];

            int currentRow = 2, questionNumber = 1;
            foreach(var semesterQuestions in generalModel.SemesterQuestionCodes)
            {
                if (semesterCountMoreThan1)
                {
                    wordTable.Cell(currentRow, 1).Merge(wordTable.Cell(currentRow, colomn_count));

                    wordTable.Cell(currentRow, 1).Range.Text = semesterQuestions.Key.ToString() + " семестр";
                    wordTable.Cell(currentRow, 1).Range.Bold = 1;
                    wordTable.Cell(currentRow, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    currentRow++;
                }

                var questions = semesterQuestions.Value;

                if(questions.Count > 0)
                {
                    foreach(var  question in questions)
                    {
                        wordTable.Cell(currentRow, 1).Range.Text = questionNumber.ToString();
                        wordTable.Cell(currentRow, 2).Range.Text = question.Question;

                        for (int j = 0; j < generalModel.DisciplineCompetencies.Count(); j++)
                        {
                            var current_competence = generalModel.DisciplineCompetencies[j];

                            for (int k = 0; k < question.Competencies.Count; k++)
                            {
                                if (question.Competencies.Contains(current_competence))
                                    wordTable.Cell(currentRow, 3 + j).Range.Text = "+";
                            }
                        }

                        wordTable.Cell(currentRow, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                        currentRow++;
                    }
                }
            }

            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
        }

        //образей вариантов тестовых заданий на экзамен
        private void CreateExamTestTasksVariantTemplateTable()
        {
            var test = generalModel.ExamTestTasksVariantTemplate;
            bool semestersCountMoreThan1 = generalModel.Semesters.Count > 1;

            int rowsCount = 2
                + test.Sum(x => x.Value.Sum(x => x.Value.Count)) + (semestersCountMoreThan1 ? generalModel.Semesters.Count : 0);

            Word.Range wordRange = FindByTag("<EXAM_TEST_TASKS_VARIANT_TEMPLATE>");
            Word.Table wordTable = wordDocument.Tables.Add(wordRange, rowsCount, 6);

            wordTable.Range.Font.Size = 12;
            wordTable.Borders.Enable = 1;

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
            wordTable.Borders.Enable = 1;
            wordTable.Cell(1, 1).Merge(wordTable.Cell(2, 1));
            wordTable.Cell(1, 2).Merge(wordTable.Cell(2, 2));
            wordTable.Cell(1, 3).Merge(wordTable.Cell(1, 6));

            int rowNumber = 3;

            // Данные
            foreach(var semesterCompetencyTasks in test)
            {
                if (semestersCountMoreThan1)
                {
                    wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 6));

                    wordTable.Cell(rowNumber, 1).Range.Text = semesterCompetencyTasks.Key.ToString() + " СЕМЕСТР";

                    wordTable.Cell(rowNumber, 1).Range.Bold = 1;
                    wordTable.Cell(rowNumber, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    rowNumber++;
                }

                var competenciesWithOwnTasks = semesterCompetencyTasks.Value;

                if(competenciesWithOwnTasks.Count > 0)
                {
                    foreach (var competencyTasks in competenciesWithOwnTasks)
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
                }
            }

            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitFixed);
        }


        // 6.4
        private void CreateRatingPointsDisctributionByDisciplineTables()
        {
            var disciplineThematicPlan = generalModel.DisciplineThematicPlan;
            var semesterNumbers = generalModel.Semesters.Select(x => x.SemesterNumber).ToList();
            bool semesterMoreThan1 = semesterNumbers.Count > 1;

            Word.Range wordRange = FindByTag("<RATING_POINTS_DISCTRIBUTION_BY_DISCIPLINE_TABLES>");

            Dictionary<int, string> semesterNumberTableTags = new Dictionary<int, string>();
            Dictionary<(int semesterNumber, int semesterModuleNumber), string> semesterModuleNumberTableTags = new Dictionary<(int semesterNumber, int semesterModuleNumber), string>();

            int current_lecture = 1;
            int current_laboratories = 1;
            int current_practical = 1;

            foreach (var semesterNumber in semesterNumbers)
            {
                wordRange.Text = "";

                // Putting markers for tables
                if (semesterMoreThan1)
                {
                    wordRange.InsertAfter(semesterNumber.ToString() + " семестр\n");
                    wordRange.Underline = WdUnderline.wdUnderlineSingle;
                    wordRange.Font.Bold = 1;
                    wordRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                string tagForSemesterTable = "<_SEMESTER_RATING_TABLE>";
                wordRange.InsertAfter(tagForSemesterTable + Environment.NewLine);
                wordRange.Font.Bold = 0;
                wordRange.Underline = WdUnderline.wdUnderlineNone;
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                wordRange.InsertAfter("Дисциплинарный модуль " + semesterNumber.ToString() + ".1\n");
                wordRange.Font.Size = 14;
                wordRange.Bold = 1;
                wordRange.Underline = WdUnderline.wdUnderlineWords;
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                string tagForSemesterModuleTable_1 = "<_SEMESTER_MODULE_RATING_TABLE_F>";
                wordRange.InsertAfter(tagForSemesterModuleTable_1 + Environment.NewLine);
                wordRange.Font.Bold = 0;
                wordRange.Underline = WdUnderline.wdUnderlineNone;
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                wordRange.InsertAfter("Дисциплинарный модуль " + semesterNumber.ToString() + ".2\n");
                wordRange.Font.Size = 14;
                wordRange.Bold = 1;
                wordRange.Underline = WdUnderline.wdUnderlineWords;
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                string tagForSemesterModuleTable_2 = "<_SEMESTER_MODULE_RATING_TABLE_S>";
                wordRange.InsertAfter(tagForSemesterModuleTable_2 + Environment.NewLine);
                wordRange.Font.Bold = 0;
                wordRange.Underline = WdUnderline.wdUnderlineNone;
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                // Putting special marker for next semester tables if it need
                string tagForNextSemestersRatingTables = "<_NEXT_SEMESTER_RATING_TABLES>";

                if (semesterNumber != semesterNumbers.Last())
                {
                    wordRange.InsertAfter(tagForNextSemestersRatingTables + Environment.NewLine);
                    wordRange.Font.Bold = 0;
                    wordRange.Underline = WdUnderline.wdUnderlineNone;
                    wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                
                // Making tables
                {
                    SemesterModuleData[] semesterModules = new SemesterModuleData[] {
                        disciplineThematicPlan[new SemesterModuleNumbers(semesterNumber, 1)],
                        disciplineThematicPlan[new SemesterModuleNumbers(semesterNumber, 2)]
                    };

                    string[] semesterModulesTableTag = new string[] {
                        tagForSemesterModuleTable_1,
                        tagForSemesterModuleTable_2
                    };

                    Word.Range wordRangeTable = FindByTag(tagForSemesterTable);

                    wordRangeTable.Text = "";

                    var wordTable = wordDocument.Tables.Add(wordRangeTable, 5, 3);


                    wordTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
                    wordTable.Columns[1].PreferredWidth = app.MillimetersToPoints(99.6f);
                    wordTable.Columns[2].PreferredWidth = app.MillimetersToPoints(38.4f);
                    wordTable.Columns[3].PreferredWidth = app.MillimetersToPoints(35.8f);

                    wordTable.Range.Font.Size = 12;

                    wordTable.Cell(1, 1).Range.Text = "Дисциплинарный модуль";

                    wordTable.Cell(2, 1).Range.Text = "Текущий контроль (лабораторные работы, практические задачи)";
                    wordTable.Cell(3, 1).Range.Text = "Текущий контроль (тестирование)";
                    wordTable.Cell(4, 1).Range.Text = "Общее количество баллов";
                    wordTable.Cell(5, 1).Range.Text = "Итоговый балл:";
                    wordTable.Cell(1, 2).Range.Text = $"ДМ {semesterNumber}.1";
                    wordTable.Cell(1, 3).Range.Text = $"ДМ {semesterNumber}.2";

                    //баллы за практические, лабы, устные
                    wordTable.Cell(2, 2).Range.Text = semesterModules[0].CurrentControl_Laboratory_Practice.Item1
                        + "-" + semesterModules[0].CurrentControl_Laboratory_Practice.Item2;
                    wordTable.Cell(2, 3).Range.Text = semesterModules[1].CurrentControl_Laboratory_Practice.Item1
                        + "-" + semesterModules[1].CurrentControl_Laboratory_Practice.Item2;

                    //баллы за тестирование
                    wordTable.Cell(3, 2).Range.Text = semesterModules[0].CurrentControl_Testing.Item1
                        + "-" + semesterModules[0].CurrentControl_Testing.Item2;
                    wordTable.Cell(3, 3).Range.Text = semesterModules[1].CurrentControl_Testing.Item1
                        + "-" + semesterModules[1].CurrentControl_Testing.Item2;

                    //общее кол-во баллов
                    wordTable.Cell(3, 2).Range.Text = semesterModules[0].TotalPointsCount.Item1
                        + "-" + semesterModules[0].TotalPointsCount.Item2;
                    wordTable.Cell(3, 3).Range.Text = semesterModules[1].TotalPointsCount.Item1
                        + "-" + semesterModules[1].TotalPointsCount.Item2;

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

                    
                    // ТАБЛИЦЫ - дисциплинарный модуль 1,2
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

                        wordRangeTable = FindByTag(semesterModuleTableTag);
                        wordRangeTable.Text = "";

                        wordTable = wordDocument.Tables.Add(wordRangeTable, row, column);

                        wordTable.Range.Font.Size = 12;

                        wordTable.Cell(1, 1).Range.Text = "№п/п";
                        wordTable.Cell(1, 2).Range.Text = "Виды работ";
                        wordTable.Cell(1, 3).Range.Text = "Максимальный балл";
                        wordTable.Cell(2, 1).Range.Text = "Текущий контроль";

                        // Форматирование
                        wordTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
                        wordTable.Columns[1].PreferredWidth = app.MillimetersToPoints(9.9f);
                        wordTable.Columns[2].PreferredWidth = app.MillimetersToPoints(128.8f);
                        wordTable.Columns[3].PreferredWidth = app.MillimetersToPoints(35.1f);

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

                        // Final merging
                        wordTable.Cell(2, 1).Merge(wordTable.Cell(2, 3));
                    }
                }


                // Go to special marker for next semester tables if it need
                if (semesterNumber != semesterNumbers.Last())
                {
                    wordRange = FindByTag(tagForNextSemestersRatingTables);
                }
            }
        }


        // 7.
        private void CreateEducationLiteratureTable()
        {
            EducationLiteratureModelComplex literatureBooks = generalModel.EducationLiteraturesComplex;

            List<EducationLiteratureModelComplex.EducationLiteratureModel> main = literatureBooks.MainLiteratures;
            List<EducationLiteratureModelComplex.EducationLiteratureModel> additional = literatureBooks.AdditionalLiteratures;
            List<EducationLiteratureModelComplex.EducationLiteratureModel> methodical = literatureBooks.EducationMethodicalLiteratures;

            int row = 2 + main.Count + 1 + additional.Count + 1 + methodical.Count;
            int column = 4;

            Word.Range wordTableRange = FindByTag("<TABLE11>");
            Word.Table wordTable = app.ActiveDocument.Tables.Add(wordTableRange, row, column);

            wordTable.Range.Font.Size = 12;
            wordTable.Borders.Enable = 1;

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

            wordTable.Rows[1].Height = app.MillimetersToPoints(40.0f);

            wordTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
            wordTable.Columns[1].PreferredWidth = app.MillimetersToPoints(9.9f);
            wordTable.Columns[2].PreferredWidth = app.MillimetersToPoints(78.4f);
            wordTable.Columns[3].PreferredWidth = app.MillimetersToPoints(73.0f);
            wordTable.Columns[4].PreferredWidth = app.MillimetersToPoints(12.2f);

            int rowNumber = 2;

            // Основная литература
            wordTable.Cell(rowNumber, 1).Range.Text = "Основная литература";
            wordTable.Cell(rowNumber, 1).Range.Bold = 1;
            wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 4));
            rowNumber++;
            for (int i = 0; i < main.Count; i++)
            {
                wordTable.Cell(rowNumber, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(rowNumber, 2).Range.Text = $"{main[i].Name}";
                wordTable.Cell(rowNumber, 3).Range.Text = main[i].Link is not null ? $"Режим доступа:\n{main[i].Link}" : $"{main[i].Count} экз.";
                wordTable.Cell(rowNumber, 4).Range.Text = main[i].Coefficient is 0 ? "" : $"{main[i].Coefficient}";
                // Форматирование
                wordTable.Cell(rowNumber, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(rowNumber, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                rowNumber++;
            }

            // Дополнительная литература
            wordTable.Cell(rowNumber, 1).Range.Text = "Дополнительная литература";
            wordTable.Cell(rowNumber, 1).Range.Bold = 1;
            wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 4));
            rowNumber++;
            for (int i = 0; i < additional.Count; i++)
            {
                wordTable.Cell(rowNumber, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(rowNumber, 2).Range.Text = $"{additional[i].Name}";
                wordTable.Cell(rowNumber, 3).Range.Text = additional[i].Link is not null ? $"Режим доступа:\n{additional[i].Link}" : $"{additional[i].Count} экз.";
                wordTable.Cell(rowNumber, 4).Range.Text = additional[i].Coefficient is 0 ? "" : $"{additional[i].Coefficient}";
                // Форматирование
                wordTable.Cell(rowNumber, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(rowNumber, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                rowNumber++;
            }

            // Учебно-методические издания
            wordTable.Cell(rowNumber, 1).Range.Text = "Учебно-методические издания";
            wordTable.Cell(rowNumber, 1).Range.Bold = 1;
            wordTable.Cell(rowNumber, 1).Merge(wordTable.Cell(rowNumber, 4));
            rowNumber++;
            for (int i = 0; i < methodical.Count; i++)
            {
                wordTable.Cell(rowNumber, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(rowNumber, 2).Range.Text = $"{methodical[i].Name}";
                wordTable.Cell(rowNumber, 3).Range.Text = methodical[i].Link is not null ? $"Режим доступа:\n{methodical[i].Link}" : $"{methodical[i].Count} экз.";
                wordTable.Cell(rowNumber, 4).Range.Text = methodical[i].Coefficient is 0 ? "" : $"{methodical[i].Coefficient}";
                // Форматирование
                wordTable.Cell(rowNumber, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(rowNumber, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                rowNumber++;
            }

            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
        }

        // 8.
        private void CreateProffectionalBasesTable()
        {
            List<LiteratureModel> site_list = generalModel.SiteList;

            int row = 1 + site_list.Count;
            int column = 3;

            Word.Range wordTableRange = FindByTag("<TABLE12>");
            Word.Table wordTable = wordDocument.Tables.Add(wordTableRange, row, column);

            wordTable.Range.Font.Size = 12;
            wordTable.Borders.Enable = 1;

            // Форматирование
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            wordTable.Rows[1].Range.Bold = 1;
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Наименование";
            wordTable.Cell(1, 3).Range.Text = "Адрес в Интернете";

            wordTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
            wordTable.Columns[1].PreferredWidth = app.MillimetersToPoints(11.9f);
            wordTable.Columns[2].PreferredWidth = app.MillimetersToPoints(96.8f);
            wordTable.Columns[3].PreferredWidth = app.MillimetersToPoints(66.1f);

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
        }

        private void CreateSoftwareInfoTable() //10
        {
            List<GeneralModel.SoftwareInfo> software_list = generalModel.SoftwareInfos;

            int row = 1 + software_list.Count;
            int column = 4;

            Word.Range wordTableRange = FindByTag("<TABLE13>");

            wordTableRange.Font.Name = "Times New Roman";

            Word.Table wordTable = wordDocument.Tables.Add(wordTableRange, row, column);

            wordTable.Range.Font.Size = 12;
            wordTable.Borders.Enable = 1;

            // форматирование
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Rows[1].Range.Bold = 1;
            wordTable.Range.Font.Size = 12;
            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Наименование программного обеспечения";
            wordTable.Cell(1, 3).Range.Text = "Лицензия";
            wordTable.Cell(1, 4).Range.Text = "Договор";

            wordTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
            wordTable.Columns[1].PreferredWidth = app.MillimetersToPoints(11.9f);
            wordTable.Columns[2].PreferredWidth = app.MillimetersToPoints(68.4f);
            wordTable.Columns[3].PreferredWidth = app.MillimetersToPoints(53.7f);
            wordTable.Columns[4].PreferredWidth = app.MillimetersToPoints(40.9f);

            int current_row = 2;
            for (int i = 0; i < software_list.Count; i++)
            {
                wordTable.Cell(current_row, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(current_row, 2).Range.Text = $"{software_list[i].Name}";
                wordTable.Cell(current_row, 3).Range.Text = $"{software_list[i].License}";

                if (software_list[i].AgreementIsExist)
                {
                    wordTable.Cell(current_row, 4).Range.Text = $"{software_list[i].Agreement}";
                }
                else
                {
                    wordTable.Cell(current_row, 3).Merge(wordTable.Cell(current_row, 4));
                }

                // форматирование
                wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(current_row, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                if (software_list[i].AgreementIsExist)
                {
                    wordTable.Cell(current_row, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                }

                current_row++;
            }
        }


        //11 материально техническая база
        private void CreateMaterialTechnicalBaseTable()
        {
            int row = 1 + generalModel.PlaceTheirEquipments.Count;
            int column = 3;

            Word.Range wordTableRange = FindByTag("<TABLE14>");
            Word.Table wordTable = wordDocument.Tables.Add(wordTableRange, row, column);

            wordTable.Range.Font.Size = 12;
            wordTable.Borders.Enable = 1;

            //форматирование
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            wordTable.Range.Font.Size = 12;
            wordTable.Rows[1].Range.Bold = 1;
            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Наименование специальных* помещений и помещений для самостоятельной работы";
            wordTable.Cell(1, 3).Range.Text = "Оснащенность специальных помещений и помещений для самостоятельной работы";

            wordTable.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
            wordTable.Columns[1].PreferredWidth = app.MillimetersToPoints(11.9f);
            wordTable.Columns[2].PreferredWidth = app.MillimetersToPoints(71.5f);
            wordTable.Columns[3].PreferredWidth = app.MillimetersToPoints(91.5f);

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
        }

        private void FillingAnnotationTable()
        {
            var rangeForLearningThemes = FindByTag("<LEARNING_THEMES>");

            rangeForLearningThemes.Text = "";

            List<(int semesterNumber, SemesterModuleData.DisciplineThematicTheme theme)> themes = new List<(int semesterNumber, SemesterModuleData.DisciplineThematicTheme theme)>();
            var semesterNumbers = generalModel.Semesters.Select(x => x.SemesterNumber).OrderBy(x => x).ToArray();
            bool semesterMoreThan1 = semesterNumbers.Length > 1;

            foreach (var semesterNumber in semesterNumbers)
            {
                var semesterThemes = themes.Where(x => x.semesterNumber == semesterNumber).Select(x => x.theme).ToList();
                
                // For learning themes cell
                if (semesterMoreThan1)
                {
                    rangeForLearningThemes.Collapse(WdCollapseDirection.wdCollapseEnd);

                    rangeForLearningThemes.InsertAfter("Семестр " + semesterNumber.ToString() + Environment.NewLine);

                    rangeForLearningThemes.Underline = WdUnderline.wdUnderlineSingle;
                    rangeForLearningThemes.Font.Bold = 1;

                    rangeForLearningThemes.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                foreach(var semesterTheme in semesterThemes)
                {
                    rangeForLearningThemes.Collapse(WdCollapseDirection.wdCollapseEnd);

                    rangeForLearningThemes.InsertAfter(semesterTheme.ThemeName);

                    rangeForLearningThemes.Underline = WdUnderline.wdUnderlineNone;
                    rangeForLearningThemes.Font.Bold = 0;

                    rangeForLearningThemes.Collapse(WdCollapseDirection.wdCollapseEnd);

                    if(semesterTheme != semesterThemes.Last())
                    {
                        rangeForLearningThemes.InsertAfter(Environment.NewLine);
                    }
                }
            }

            // For attestation cell
            var rangeForAttestationsDate = FindByTag("<ATTESTATIONS_DATE>", true);

            rangeForAttestationsDate.Text = "";

            var semesterNumbers_attestType = generalModel.OffsetSemesterNumbers.Select(x => (x, 0)).Union(
                    generalModel.OffsetWithMarkSemesterNumbers.Select(x => (x, 1))).Union(
                    generalModel.ExamSemesterNumbers.Select(x => (x, 2))).OrderBy(x => x.x).ToArray();

            foreach (var semesterNumber in semesterNumbers)
            {
                var semesterNumber_attestType = semesterNumbers_attestType.First(x => x.x == semesterNumber);

                rangeForAttestationsDate.Collapse(WdCollapseDirection.wdCollapseEnd);

                rangeForAttestationsDate.InsertAfter((semesterNumber_attestType.Item2 == 0 ? "Зачет "
                           : semesterNumber_attestType.Item2 == 1 ? "Зачет с оценкой "
                           : "Экзамен "));

                rangeForAttestationsDate.Font.Bold = 1;
                rangeForAttestationsDate.Collapse(WdCollapseDirection.wdCollapseEnd);

                rangeForAttestationsDate.InsertAfter("в " + semesterNumber.ToString() + " семестре");
                rangeForAttestationsDate.Font.Bold = 0;

                if(semesterNumber != semesterNumbers.Last())
                {
                    rangeForAttestationsDate.InsertAfter(Environment.NewLine);
                }
            }
        }
    }
}
