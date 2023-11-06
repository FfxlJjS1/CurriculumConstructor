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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using DataTable = System.Data.DataTable;
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

        public WordHelper(string fileName, GeneralModel generalModel)
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

        internal bool Process(bool forPreview, string nameForSave = "shablon_1.docx")
        {
            try
            {
                Object file = _fileInfo.FullName;

                app = new Word.Application();
                wordDocument = app.Documents.Open(file, ReadOnly: false);

                replaceText(new Dictionary<string, string>());

                createTable1();
                createTable2();
                createTable3_15("<TABLE3>");
                createTable4();
                createTable5();
                createTable6();
                createTable7();
                createTable8();

                createTable9();
                createTable10(); ////
                createTable11();
                createTable12();
                createTable13();
                createTable14();
                createTable3_15("<TABLE15>");
                createTable16();

                if (forPreview)
                    PreviewView(nameForSave);
                else
                    saveWord();

                app.ActiveDocument.Close();

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
            Object missing = Type.Missing;

            List<Semester> semesters = generalModel.Semesters;

            if (items.Count <= 0)
            {
                items = new Dictionary<string, string>
                {
                    //EXCEL ИЛИ ПРОГРАММНО РАССЧИТАТЬ
                    //0-1
                    {"<YEAR>", DateTime.Now.Year.ToString() },
                    {"<INDEX>", generalModel.Index },
                    {"<DISCIPLINE>", generalModel.DisciplineName }, //6, 6.3.1.1 (ЛАБЫ), 6.4, 11, аннотация
                    {"<DIRECTION>", generalModel.ProfileNumber + " " +  generalModel.ProfileName }, //2, 6.4, 12, аннотация
                    {"<PROFILE>", generalModel.ProfileName }, //2, 12, аннотация
                    {"<QUALIFICATION>", generalModel.Qualification },
                    {"<FORM_STUDY>", generalModel.EducationForm },
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
                    {"<WORK>",
                        "Контактная работа обучающихся с преподавателем - " + generalModel.ContansHours.ToString() + " часов;"
                        + "\r\n- лекции " + semesters.Sum(semester => Convert.ToInt32(semester.Lectures)).ToString() + " ч.;"
                        + "\r\n- практические занятия " + semesters.Sum(semester => Convert.ToInt32(semester.PracticeWorks)).ToString() + " ч.;"
                        + "\r\n- лабораторные работы " + semesters.Sum(semester => Convert.ToInt32(semester.LaboratoryWorks)).ToString() + " ч."
                        + "\r\nСамостоятельная работа " + semesters.Sum(semester => Convert.ToInt32(semester.IndependentWork)).ToString() + " ч."
                        + "\r\nКонтроль (экзамен) " + semesters.Sum(semester => Convert.ToInt32(semester.Control)).ToString() + " ч."},
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
                Word.Find find = app.Selection.Find;
                find.Text = item.Key;
                find.Replacement.Text = item.Value;

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
            }
        }

        private void createTable1()
        {
            //СОЗДАНИЕ И ЗАПОЛНЕНИЕ ТАБЛИЦЫ
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("Picture ID", typeof(int)));
            dt.Columns.Add(new DataColumn("Title", typeof(string)));
            dt.Columns.Add(new DataColumn("Date Added", typeof(DateTime)));

            DataRow dr = dt.NewRow();
            dr["Picture ID"] = 1;
            dr["Title"] = "Earth";
            dr["Date Added"] = new DateTime();
            dt.Rows.Add(dr);

            //вставка таблицы
            app.Selection.Find.Execute("<TABLE1>");
            Word.Range wordRange = app.Selection.Range;

            var wordTable = wordDocument.Tables.Add(wordRange,
                dt.Rows.Count, dt.Columns.Count);
            wordTable.Borders.Enable = 1;
            wordTable.Columns.Width = 100;
            //wordTable.Columns[1].Cells[1].Column.Cells.Borders.OutsideColor = WdColor.wdColorDarkRed;
            //wordTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            wordTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            wordTable.Borders.OutsideLineWidth = WdLineWidth.wdLineWidth225pt;
            //..Borders.InsideColor = Word.WdColor.wdColorAqua;

            for (var j = 0; j < dt.Rows.Count; j++)
            {
                for (var k = 0; k < dt.Columns.Count; k++)
                {
                    wordTable.Cell(j + 1, k + 1).Range.Text = dt.Rows[j][k].ToString();
                }
            }
        }

        private void createTable2()
        {
            var dt2 = new DataTable();
            dt2.Columns.Add(new DataColumn("Номер", typeof(string)));
            dt2.Columns.Add(new DataColumn("Тема", typeof(string)));
            dt2.Columns.Add(new DataColumn("Семестр", typeof(string)));
            dt2.Columns.Add(new DataColumn("Лекции", typeof(string)));
            dt2.Columns.Add(new DataColumn("Практические", typeof(string)));
            dt2.Columns.Add(new DataColumn("Лабораторные", typeof(string)));
            dt2.Columns.Add(new DataColumn("СРС", typeof(string)));

            for (int i = 0; i < generalModel.DisciplineThematicPlan.Count + 3; i++)
                dt2.Rows.Add();

            app.Selection.Find.Execute("<TABLE2>");
            Word.Range wordRange2 = app.Selection.Range;
            var wordTable2 = wordDocument.Tables.Add(wordRange2,
                dt2.Rows.Count, dt2.Columns.Count);

            //форматирование
            for (int i = 1; i <= 2; i++)
                for (int j = 1; j <= dt2.Columns.Count; j++)
                    wordTable2.Cell(i, j).Range.Bold = Convert.ToInt32(true);
            wordTable2.Cell(1, 4).Merge(wordTable2.Cell(1, 5));
            wordTable2.Cell(1, 4).Merge(wordTable2.Cell(1, 5));

            //заполнение шаблона
            wordTable2.Cell(1, 1).Range.Text = "№ п/п";
            wordTable2.Cell(1, 2).Range.Text = "Темы дисциплины";
            wordTable2.Cell(1, 3).Range.Text = "семестр";
            wordTable2.Cell(1, 4).Range.Text = "Виды и часы " +
                "контактной \nработы, \nих трудоемкость \n(в часах)";
            wordTable2.Cell(1, 5).Range.Text = "СРС";

            //направление текста
            wordTable2.Cell(1, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable2.Cell(1, 2).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable2.Cell(1, 3).Range.Orientation = WdTextOrientation.wdTextOrientationUpward;
            wordTable2.Cell(1, 3).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable2.Cell(1, 5).Range.Orientation = WdTextOrientation.wdTextOrientationUpward;
            wordTable2.Cell(1, 5).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            //объединение ячеек
            wordTable2.Cell(1, 1).Merge(wordTable2.Cell(2, 1));
            wordTable2.Cell(1, 2).Merge(wordTable2.Cell(2, 2));
            wordTable2.Cell(1, 3).Merge(wordTable2.Cell(2, 3));
            wordTable2.Cell(1, 5).Merge(wordTable2.Cell(2, 7));

            float width_column1, width_column2, width_column3,
                width_column4, width_column5,
                width_column6, width_column7, point;

            point = 28.35f;
            width_column1 = 1.13f * point;
            width_column2 = 7.83f * point;
            width_column3 = 1.8f * point;
            width_column4 = 1.51f * point;
            width_column5 = 1.67f * point;
            width_column6 = 1.66f * point;
            width_column7 = 1.18f * point;

            //ширина, высоты столбцов
            wordTable2.Cell(1, 1).Width = width_column1;
            wordTable2.Cell(1, 2).Width = width_column2;
            wordTable2.Cell(1, 3).Width = width_column3;
            wordTable2.Cell(1, 4).Width = 4.84f * 28.35f;
            wordTable2.Cell(1, 5).Width = width_column7;
            wordTable2.Cell(1, 4).Height = 1.21f * 28.35f;

            //заполнение шаблона
            wordTable2.Cell(2, 4).Range.Text = "Лекции";
            wordTable2.Cell(2, 5).Range.Text = "Практические занятия";
            wordTable2.Cell(2, 6).Range.Text = "Лабораторные занятия";

            //направление текста
            wordTable2.Cell(2, 4).Range.Orientation = WdTextOrientation.wdTextOrientationUpward;
            wordTable2.Cell(2, 4).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable2.Cell(2, 5).Range.Orientation = WdTextOrientation.wdTextOrientationUpward;
            wordTable2.Cell(2, 5).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            wordTable2.Cell(2, 6).Range.Orientation = WdTextOrientation.wdTextOrientationUpward;
            wordTable2.Cell(2, 6).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            //ширина, высоты столбцов
            wordTable2.Cell(2, 4).Width = width_column4;
            wordTable2.Cell(2, 5).Width = width_column5;
            wordTable2.Cell(2, 6).Width = width_column6;
            wordTable2.Cell(2, 7).Width = width_column7;
            wordTable2.Cell(2, 5).Height = 3.31f * 28.35f;

            var themes = generalModel.DisciplineThematicPlan;
            int countItems = generalModel.DisciplineThematicPlan.Count;

            for (int i = 0; i < countItems; i++)
            {
                wordTable2.Cell(3 + i, 1).Range.Text = (i + 1).ToString();
                wordTable2.Cell(3 + i, 2).Range.Text = themes[i].ThemeName;
                wordTable2.Cell(3 + i, 3).Range.Text = themes[i].Semester.ToString();
                wordTable2.Cell(3 + i, 4).Range.Text = themes[i].LectureHours != 0 ? themes[i].LectureHours.ToString() : "-";
                wordTable2.Cell(3 + i, 5).Range.Text = themes[i].PracticeHours != 0 ? themes[i].PracticeHours.ToString() : "-";
                wordTable2.Cell(3 + i, 6).Range.Text = themes[i].LaboratoryWorkHours != 0 ? themes[i].LaboratoryWorkHours.ToString() : "-";
                wordTable2.Cell(3 + i, 7).Range.Text = themes[i].IndependentHours != 0 ? themes[i].IndependentHours.ToString() : "-";

                wordTable2.Cell(3 + i, 1).Width = width_column1;
                wordTable2.Cell(3 + i, 2).Width = width_column2;
                wordTable2.Cell(3 + i, 3).Width = width_column3;
                wordTable2.Cell(3 + i, 4).Width = width_column4;
                wordTable2.Cell(3 + i, 5).Width = width_column5;
                wordTable2.Cell(3 + i, 6).Width = width_column6;
                wordTable2.Cell(3 + i, 7).Width = width_column7;

                //выравнивание=слева
                wordTable2.Cell(3 + i, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            }

            //последняя строка
            wordTable2.Cell(3 + countItems, 1).Range.Text = "";
            wordTable2.Cell(3 + countItems, 2).Range.Text = "Итого по дисциплине";
            wordTable2.Cell(3 + countItems, 3).Range.Text = "";
            wordTable2.Cell(3 + countItems, 4).Range.Text = generalModel.NeedTotalLectureHours.ToString();
            wordTable2.Cell(3 + countItems, 5).Range.Text = generalModel.NeedTotalPracticeHours.ToString();
            wordTable2.Cell(3 + countItems, 6).Range.Text = generalModel.NeedTotalLaboratoryWorkHours.ToString();
            wordTable2.Cell(3 + countItems, 7).Range.Text = generalModel.NeedTotalIndependentHours.ToString();

            wordTable2.Cell(3 + countItems, 1).Width = width_column1;
            wordTable2.Cell(3 + countItems, 2).Width = width_column2;
            wordTable2.Cell(3 + countItems, 3).Width = width_column3;
            wordTable2.Cell(3 + countItems, 4).Width = width_column4;
            wordTable2.Cell(3 + countItems, 5).Width = width_column5;
            wordTable2.Cell(3 + countItems, 6).Width = width_column6;
            wordTable2.Cell(3 + countItems, 7).Width = width_column7;

            for (int i = 1; i <= 7; i++)
                wordTable2.Cell(3 + countItems, i).Range.Bold = Convert.ToInt32(true);

            //форматирование таблицы
            wordTable2.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            wordTable2.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable2.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable2.Borders.Enable = 1;
            //Столбец СРС
            wordTable2.Cell(1, 5).Merge(wordTable2.Cell(2, 7));
        }


        private void createTable3_15(string tag)
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("Оцениваемые компетенции", typeof(string)));
            dt.Columns.Add(new DataColumn("Код и наименование индикатора", typeof(string)));
            dt.Columns.Add(new DataColumn("Результаты освоения", typeof(string)));
            dt.Columns.Add(new DataColumn("Оценочные средства", typeof(string)));

            //данные - код, имя, знать, уметь, владеть, индикаторы
            List<GeneralModel.CompetencyPlanningResult> competences = generalModel.competencyPlanningResults;

            for (int i = 0; i < competences.Count + 1; i++)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
            }

            app.Selection.Find.Execute(tag);
            Word.Range wordRange = app.Selection.Range;

            var wordTable = wordDocument.Tables.Add(wordRange,
                dt.Rows.Count, dt.Columns.Count);

            wordTable.Cell(1, 1).Range.Text = "Оцениваемые компетенции (код, наименование)";
            wordTable.Cell(1, 2).Range.Text = "Код и наименование индикатора (индикаторов) достижения компетенции";
            wordTable.Cell(1, 3).Range.Text = "Результаты освоения компетенции";
            wordTable.Cell(1, 4).Range.Text = "Оценочные средства текущего контроля и промежуточной аттестации";

            //заполнение данными
            for (int i = 0; i < competences.Count; i++)
            {
                var item = competences[i];
                string kod = item.Code;
                string name = item.CodeName;
                string know = item.ToKnowResult;
                string able = item.ToAbilityResult;
                string own = item.ToOwnResult;
                List<GeneralModel.CompetencyPlanningResult.CompetencyAchivmentIndicator> childs = item.CompetencyAchivmentIndicators;

                //Столбец1
                Word.Range range = wordTable.Cell(2 + i, 1).Range;
                range.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range.InsertAfter(kod);
                range.Font.Bold = Convert.ToInt32(true);
                range.InsertParagraphAfter();
                range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                range.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range.InsertAfter(name);
                range.Font.Bold = Convert.ToInt32(false);
                range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                //Столбец2
                Word.Range range2 = wordTable.Cell(2 + i, 2).Range;
                for (int j = 0; j < childs.Count; j++)
                {
                    var child = childs[j];
                    string kod_child = child.Code;
                    string name_child = child.Text;

                    range2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range2.InsertAfter(kod_child + ".");
                    range2.Font.Bold = Convert.ToInt32(true);
                    range2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                    range2.Collapse(Word.WdCollapseDirection.wdCollapseStart);

                    if (j == childs.Count - 1)
                    {
                        range2.InsertAfter(" " + name_child + ".");
                    }
                    else
                    {
                        range2.InsertAfter(" " + name_child + ";");
                        range2.InsertParagraphAfter();
                    }
                    range2.Font.Bold = Convert.ToInt32(false);
                    range2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                }

                //Столбец3
                Word.Range range3 = wordTable.Cell(2 + i, 3).Range;
                //знать
                range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range3.InsertAfter("Знать:");
                range3.Font.Bold = Convert.ToInt32(true);
                range3.InsertParagraphAfter();
                range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range3.InsertAfter(know.ToLower() + ";");
                range3.Font.Bold = Convert.ToInt32(false);
                range3.InsertParagraphAfter();
                range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                //уметь
                range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range3.InsertAfter("Уметь:");
                range3.Font.Bold = Convert.ToInt32(true);
                range3.InsertParagraphAfter();
                range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range3.InsertAfter(able.ToLower() + ";");
                range3.Font.Bold = Convert.ToInt32(false);
                range3.InsertParagraphAfter();
                range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                //владеть
                range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range3.InsertAfter("Владеть:");
                range3.Font.Bold = Convert.ToInt32(true);
                range3.InsertParagraphAfter();
                range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range3.InsertAfter(own.ToLower() + ".");
                range3.Font.Bold = Convert.ToInt32(false);
                range3.InsertParagraphAfter();
                range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                //!!!!!!!!!!!!!!!!ДОРАБОТАТЬ - данные брать из excel или программы
                // Работает по странному...
                //Столбец4

                Word.Range range4 = wordTable.Cell(2 + i, 4).Range;
                //текущий контроль
                range4.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4.InsertAfter("Текущий контроль:");
                range4.Font.Bold = Convert.ToInt32(true);
                range4.InsertParagraphAfter();
                range4.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                range4.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4.InsertAfter(
                    "Компьютерное тестирование по теме 1-5\n"
                    + "Практические задачи по темам 1-5\n"
                    + "Лабораторные работыпо темам 1-3"); //!!!!!
                range4.Font.Bold = Convert.ToInt32(false);
                range4.InsertParagraphAfter();
                range4.InsertParagraphAfter();
                range4.InsertParagraphAfter();
                range4.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                //промежуточная аттестация
                range4.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4.InsertAfter("Промежуточная аттестация:");
                range4.Font.Bold = Convert.ToInt32(true);
                range4.InsertParagraphAfter();
                range4.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                range4.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4.InsertAfter("Экзамен"); //!!!!!
                range4.Font.Bold = Convert.ToInt32(false);
                range4.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            }

            float width_column1, width_column2, width_column3,
                width_column4, point;


            //форматирование таблицы
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
        }

        private void createTable4()
        {
            string main_key = "<TABLE4>";
            app.Selection.Find.Execute(main_key);
            Word.Range wordRange = app.Selection.Range;
            Dictionary<string, string> text_keys = new Dictionary<string, string>();
            Dictionary<string, int> table_keys = new Dictionary<string, int>();

            //вставка тэгов в ворд
            foreach (var semester in generalModel.Semesters)
            {
                string semester_text = "<SEMESTER" + semester.SemesterNumber + ">";
                string semester_table = "<SEMESTER_TABLE" + semester.SemesterNumber + ">";
                text_keys.Add(semester_text, $"Семестр {semester.SemesterNumber}");
                table_keys.Add(semester_table, semester.SemesterNumber);

                wordRange.Collapse(WdCollapseDirection.wdCollapseStart);
                wordRange.InsertAfter(semester_text);
                wordRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordRange.Bold = Convert.ToInt32(true);
                wordRange.Font.Size = 14;
                wordRange.InsertParagraphAfter();
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                wordRange.Collapse(WdCollapseDirection.wdCollapseStart);
                wordRange.InsertAfter(semester_table);
                wordRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                wordRange.Bold = Convert.ToInt32(false);
                wordRange.Font.Size = 12;
                wordRange.InsertParagraphAfter();
                wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);
            }

            //удаление main_key из word
            app.Selection.Find.Execute(main_key);
            Word.Range wordRangeDelete = app.Selection.Range;
            wordRangeDelete.Delete();

            string[] columns = {
                "Тема",
                "Кол-во часов",
                "Используемый метод",
                "Формируемые компетенции" };

            //замена тэгов на таблицы семестров
            foreach (var semester in table_keys)
            {
                var dt = new DataTable();
                foreach (string column in columns)
                    dt.Columns.Add(column);

                int count_theme = discipline.themes.Where(a => a.semester.Equals(semester.Value)).Count();
                int count_theme_lecture = discipline.themes.Where(a => a.semester.Equals(semester.Value) && a.lectures is not null).SelectMany(a => a.lectures).ToList().Count;
                int count_theme_laboratory = discipline.themes.Where(a => a.semester.Equals(semester.Value) && a.laboratories is not null).SelectMany(a => a.laboratories).ToList().Count;
                int count_theme_practical = discipline.themes.Where(a => a.semester.Equals(semester.Value) && a.practicals is not null).SelectMany(a => a.practicals).ToList().Count;
                int total_row = count_theme + count_theme_lecture + count_theme_laboratory + count_theme_practical;

                for (int i = 0; i < total_row + 3; i++)
                    dt.Rows.Add();

                app.Selection.Find.Wrap = WdFindWrap.wdFindContinue;
                bool g = app.Selection.Find.Execute(semester.Key);
                Word.Range wordRangeTable = app.Selection.Range;
                var wordTable = wordDocument.Tables.Add(wordRangeTable,
                    dt.Rows.Count, dt.Columns.Count);

                wordTable.Cell(1, 1).Range.Text = columns[0];
                wordTable.Cell(1, 2).Range.Text = columns[1];
                wordTable.Cell(1, 3).Range.Text = columns[2];
                wordTable.Cell(1, 4).Range.Text = columns[3];

                bool first_write1 = true;
                bool first_write2 = true;
                int current_row = 2;
                int current_lecture = 1;
                int current_laboratories = 1;
                int current_practical = 1;

                for (int i = 0; i < discipline.themes.Count; i++)
                {
                    if (semester.Value.Equals(discipline.themes[i].semester))
                    {
                        if (discipline.themes[i].module == 1)
                        {
                            if (first_write1)
                            {
                                wordTable.Cell(current_row, 1).Range.Text = "Дисциплинарный модуль " + semester.Value + ".1";
                                wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, 4));
                                //форматирование
                                wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);

                                current_row++;
                                first_write1 = false;
                            }

                            //загрузка данных в таблицу
                            int[] currents = loadDataTable4(wordTable, current_lecture, current_laboratories, current_practical, current_row, i);
                            current_lecture = currents[0];
                            current_laboratories = currents[1];
                            current_practical = currents[2];
                            current_row = currents[3];

                        }
                        else if (discipline.themes[i].module == 2)
                        {
                            if (first_write2)
                            {
                                wordTable.Cell(current_row, 1).Range.Text = "Дисциплинарный модуль " + semester.Value + ".2";
                                wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, 4));
                                //форматирование
                                wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
                                current_row++;
                                first_write2 = false;
                            }

                            //загрузка данных в таблицу
                            int[] currents = loadDataTable4(wordTable, current_lecture, current_laboratories, current_practical, current_row, i);
                            current_lecture = currents[0];
                            current_laboratories = currents[1];
                            current_practical = currents[2];
                            current_row = currents[3];
                        }
                    }
                }

                //форматирование таблицы
                wordTable.Borders.Enable = Convert.ToInt32(true);
                wordTable.Range.ParagraphFormat.SpaceBefore = 0;
                wordTable.Range.ParagraphFormat.SpaceAfter = 0;
                wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);

                wordTable.Cell(1, 1).Range.Bold = Convert.ToInt32(true);
                wordTable.Cell(1, 2).Range.Bold = Convert.ToInt32(true);
                wordTable.Cell(1, 3).Range.Bold = Convert.ToInt32(true);
                wordTable.Cell(1, 4).Range.Bold = Convert.ToInt32(true);
            }
            //замена тэгов на слова семестров
            replaceText(text_keys);
        }

        //загрузка данных в таблицу4
        //чтобы убрать повтор кода
        private int[] loadDataTable4(Word.Table wordTable,
            int current_lecture, int current_laboratories,
            int current_practical, int current_row, int i)
        {
            List<ChildThemeModel>? lectures = discipline.themes[i].lectures;
            List<ChildThemeModel>? laboratories = discipline.themes[i].laboratories;
            List<ChildThemeModel>? practicals = discipline.themes[i].practicals;

            int hour_lecture = lectures is not null ? lectures.Select(a => a.hour).Sum() : 0;
            int hour_lab = laboratories is not null ? laboratories.Select(a => a.hour).Sum() : 0;
            int hour_practical = practicals is not null ? practicals.Select(a => a.hour).Sum() : 0;
            int total_hour = hour_lecture + hour_lab + hour_practical;

            wordTable.Cell(current_row, 1).Range.Text = $"Тема {i + 1}. {discipline.themes[i].theme} ({total_hour} ч.)";
            wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, 4));
            //форматирование
            wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
            current_row++;

            if (lectures is not null)
                foreach (var lecture in lectures)
                {
                    //Столбец 1 - Тема
                    Word.Range rangeColumn1 = wordTable.Cell(current_row, 1).Range;
                    rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    rangeColumn1.InsertAfter("Лекция " + current_lecture + ".");
                    rangeColumn1.Font.Italic = Convert.ToInt32(true);
                    rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                    rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    rangeColumn1.InsertAfter(" " + lecture.name);
                    rangeColumn1.Font.Italic = Convert.ToInt32(false);
                    rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                    //Столбец 2 - Кол-во часов
                    wordTable.Cell(current_row, 2).Range.Text = lecture.hour.ToString();

                    //Столбец 3 - Используемый метод
                    wordTable.Cell(current_row, 3).Range.Text = lecture.method;

                    //Столбец 4 - Формируемые компетенции
                    wordTable.Cell(current_row, 4).Range.Text = string.Join(", ", generalModel.Competencies);

                    //форматирование
                    wordTable.Cell(current_row, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    wordTable.Cell(current_row, 3).Range.Italic = Convert.ToInt32(true);

                    //переход на новую строку
                    current_row++;
                    current_lecture++;
                }

            if (laboratories is not null)
                foreach (var lab in laboratories)
                {
                    Word.Range rangeColumn1 = wordTable.Cell(current_row, 1).Range;
                    rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    rangeColumn1.InsertAfter("Лабораторная работа  " + current_laboratories + ".");
                    rangeColumn1.Font.Italic = Convert.ToInt32(true);
                    rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                    rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    rangeColumn1.InsertAfter(" " + lab.name);
                    rangeColumn1.Font.Italic = Convert.ToInt32(false);
                    rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                    wordTable.Cell(current_row, 2).Range.Text = lab.hour.ToString();
                    wordTable.Cell(current_row, 3).Range.Text = lab.method;
                    wordTable.Cell(current_row, 4).Range.Text = string.Join(", ", generalModel.Competencies);

                    //форматирование
                    wordTable.Cell(current_row, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    wordTable.Cell(current_row, 3).Range.Italic = Convert.ToInt32(true);

                    current_row++;
                    current_laboratories++;
                }

            if (practicals is not null)
                foreach (var practical in practicals)
                {
                    Word.Range rangeColumn1 = wordTable.Cell(current_row, 1).Range;
                    rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    rangeColumn1.InsertAfter("Практическое занятие " + current_practical + ".");
                    rangeColumn1.Font.Italic = Convert.ToInt32(true);
                    rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                    rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    rangeColumn1.InsertAfter(" " + practical.name);
                    rangeColumn1.Font.Italic = Convert.ToInt32(false);
                    rangeColumn1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                    wordTable.Cell(current_row, 2).Range.Text = practical.hour.ToString();
                    wordTable.Cell(current_row, 3).Range.Text = practical.method;
                    wordTable.Cell(current_row, 4).Range.Text = string.Join(", ", generalModel.Competencies);

                    //форматирование
                    wordTable.Cell(current_row, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    wordTable.Cell(current_row, 3).Range.Italic = Convert.ToInt32(true);

                    current_row++;
                    current_practical++;
                }
            int[] currents = { current_lecture, current_laboratories, current_practical, current_row };
            return currents;
        }

        private void createTable5()
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("Этап", typeof(string)));
            dt.Columns.Add(new DataColumn("Название", typeof(string)));
            dt.Columns.Add(new DataColumn("Описание", typeof(string)));
            dt.Columns.Add(new DataColumn("Представление", typeof(string)));

            //данные - код, имя, знать, уметь, владеть, индикаторы
            List<EvaluationToolModel> controls = discipline.controls;
            List<EvaluationToolModel> attestations = discipline.attestations;

            int total_row = controls.Count + attestations.Count;
            for (int i = 0; i < total_row + 3; i++)
                dt.Rows.Add();

            app.Selection.Find.Execute("<TABLE5>");
            Word.Range wordRange = app.Selection.Range;
            var wordTable = wordDocument.Tables.Add(wordRange,
                dt.Rows.Count, dt.Columns.Count);

            wordTable.Cell(1, 1).Range.Text = "Этапы формирования компетенции";
            wordTable.Cell(1, 2).Range.Text = "Вид оценочного средства";
            wordTable.Cell(1, 3).Range.Text = "Краткая характеристика оценочного средства";
            wordTable.Cell(1, 4).Range.Text = "Представление оценочного средства в фонде";
            wordTable.Cell(2, 1).Range.Text = "Текущий контроль";

            //форматирование
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Cell(2, 1).Range.Bold = Convert.ToInt32(true);
            wordTable.Cell(2, 1).Merge(wordTable.Cell(2, 4));

            int current_row = 3;
            int stage = 1;
            foreach (var control in controls)
            {
                wordTable.Cell(current_row, 1).Range.Text = stage.ToString();
                wordTable.Cell(current_row, 2).Range.Text = control.name;
                wordTable.Cell(current_row, 3).Range.Text = control.description;
                wordTable.Cell(current_row, 4).Range.Text = control.path;
                //форматирование
                wordTable.Cell(current_row, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(current_row, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                wordTable.Cell(current_row, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                //переход на новую строку
                stage++;
                current_row++;
            }

            wordTable.Cell(current_row, 1).Range.Text = "Промежуточная аттестация";
            wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
            wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, 4));
            current_row++;

            foreach (var attestation in attestations)
            {
                wordTable.Cell(current_row, 1).Range.Text = stage.ToString();
                wordTable.Cell(current_row, 2).Range.Text = attestation.name;
                wordTable.Cell(current_row, 3).Range.Text = attestation.description;
                wordTable.Cell(current_row, 4).Range.Text = attestation.path;
                //форматирование
                wordTable.Cell(current_row, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(current_row, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                wordTable.Cell(current_row, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                //переход на новую строку
                stage++;
                current_row++;
            }

            //форматирование
            wordTable.Borders.Enable = Convert.ToInt32(true);
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
        }

        //6.2 Уровень освоения компетенции и критерии оценивания результатов обучения
        private void createTable6()
        {
            List<GeneralModel.CompetencyPlanningResult> competences = generalModel.competencyPlanningResults;


            var dt = new DataTable();
            dt.Columns.Add("номер");
            dt.Columns.Add("код");
            dt.Columns.Add("индикатор");
            dt.Columns.Add("результаты");
            dt.Columns.Add("продвинутый");
            dt.Columns.Add("средний");
            dt.Columns.Add("базовый");
            dt.Columns.Add("компетенции не освоены");

            for (int i = 1; i <= (competences.Count * 3) + 4; i++)
                dt.Rows.Add();

            app.Selection.Find.Execute("<TABLE6>");
            Word.Range wordRange = app.Selection.Range;
            var wordTable = wordDocument.Tables.Add(wordRange, dt.Rows.Count, dt.Columns.Count);
            //форматирование
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            for (int i = 1; i <= 4; i++)
                for (int j = 1; j <= wordTable.Columns.Count; j++)
                {
                    wordTable.Cell(i, j).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    wordTable.Cell(i, j).Range.Bold = Convert.ToInt32(true);
                }

            //шапка таблицы
            //строка1
            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Оцениваемые компетенции (код, наименование)";
            wordTable.Cell(1, 3).Range.Text = "Код и наименование индикатора (индикаторов) достижения компетенции";
            wordTable.Cell(1, 4).Range.Text = "Планируемые результаты обучения";
            wordTable.Cell(1, 5).Range.Text = "Уровень освоения компетенций";
            //строка2
            wordTable.Cell(2, 5).Range.Text = "Продвинутый уровень";
            wordTable.Cell(2, 6).Range.Text = "Средний уровень";
            wordTable.Cell(2, 7).Range.Text = "Базовый уровень";
            wordTable.Cell(2, 8).Range.Text = "Компетенции не освоены";
            //строка3
            wordTable.Cell(3, 5).Range.Text = "Критерии оценивания результатов обучения";
            //строка4
            wordTable.Cell(4, 5).Range.Text = "«отлично»\n(от 86 до 100 баллов)";
            wordTable.Cell(4, 6).Range.Text = "«хорошо»\n(от 71 до 85 баллов)";
            wordTable.Cell(4, 7).Range.Text = "«удовлетворительно»\n(от 55 до 70 баллов)\n";
            wordTable.Cell(4, 8).Range.Text = "«неудовлетв.»\n(менее 55 баллов)";
            //объединение строк, столбцов
            wordTable.Cell(1, 1).Merge(wordTable.Cell(4, 1));
            wordTable.Cell(1, 2).Merge(wordTable.Cell(4, 2));
            wordTable.Cell(1, 3).Merge(wordTable.Cell(4, 3));
            wordTable.Cell(1, 4).Merge(wordTable.Cell(4, 4));
            wordTable.Cell(1, 5).Merge(wordTable.Cell(1, 8));
            wordTable.Cell(3, 5).Merge(wordTable.Cell(3, 8));

            int current_row = 5;
            for (int i = 0; i < competences.Count; i++)
            {
                var current_competence = competences[i];
                string kod = current_competence.Code;
                string name = current_competence.CodeName;
                string know = current_competence.ToKnowResult;
                string able = current_competence.ToAbilityResult;
                string own = current_competence.ToOwnResult;
                var childs = current_competence.CompetencyAchivmentIndicators;

                //Столбец1
                wordTable.Cell(current_row, 1).Range.Text = (i + 1).ToString();

                //Столбец2
                Word.Range range2 = wordTable.Cell(current_row, 2).Range;
                range2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range2.InsertAfter(kod);
                range2.Font.Bold = Convert.ToInt32(true);
                range2.InsertParagraphAfter();
                range2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                range2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range2.InsertAfter(name);
                range2.Font.Bold = Convert.ToInt32(false);
                range2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                //Столбец3
                Word.Range range3 = wordTable.Cell(current_row, 3).Range;
                for (int j = 0; j < childs.Count; j++)
                {
                    var child = childs[j];
                    string kod_child = child.Code;
                    string name_child = child.Text;

                    range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                    range3.InsertAfter(kod_child + ".");
                    range3.Font.Bold = Convert.ToInt32(true);
                    range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                    range3.Collapse(Word.WdCollapseDirection.wdCollapseStart);

                    if (j == childs.Count - 1)
                    {
                        range3.InsertAfter(" " + name_child + ".");
                    }
                    else
                    {
                        range3.InsertAfter(" " + name_child + ";");
                        range3.InsertParagraphAfter();
                    }
                    range3.Font.Bold = Convert.ToInt32(false);
                    range3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                }

                //Столбец4
                //знать
                Word.Range range4_1 = wordTable.Cell(current_row, 4).Range;
                range4_1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4_1.InsertAfter("Знать:");
                range4_1.Font.Bold = Convert.ToInt32(true);
                range4_1.InsertParagraphAfter();
                range4_1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                range4_1.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4_1.InsertAfter(know.ToLower() + ";");
                range4_1.Font.Bold = Convert.ToInt32(false);
                range4_1.InsertParagraphAfter();
                range4_1.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                //уметь
                Word.Range range4_2 = wordTable.Cell(current_row + 1, 4).Range;
                range4_2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4_2.InsertAfter("Уметь:");
                range4_2.Font.Bold = Convert.ToInt32(true);
                range4_2.InsertParagraphAfter();
                range4_2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                range4_2.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4_2.InsertAfter(able.ToLower() + ";");
                range4_2.Font.Bold = Convert.ToInt32(false);
                range4_2.InsertParagraphAfter();
                range4_2.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                //владеть
                Word.Range range4_3 = wordTable.Cell(current_row + 2, 4).Range;
                range4_3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4_3.InsertAfter("Владеть:");
                range4_3.Font.Bold = Convert.ToInt32(true);
                range4_3.InsertParagraphAfter();
                range4_3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                range4_3.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                range4_3.InsertAfter(own.ToLower() + ".");
                range4_3.Font.Bold = Convert.ToInt32(false);
                range4_3.InsertParagraphAfter();
                range4_3.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                //Столбец5-8
                var results = current_competence.CompetencyAchivmentMarkCriteries;
                var knowResult = results[0];
                var ableResult = results[1];
                var ownResult = results[2];
                //знать
                wordTable.Cell(current_row, 5).Range.Text = knowResult.Excelent;
                wordTable.Cell(current_row, 6).Range.Text = knowResult.Good;
                wordTable.Cell(current_row, 7).Range.Text = knowResult.Satisfactory;
                wordTable.Cell(current_row, 8).Range.Text = knowResult.Unsatisfactory;
                //уметь
                wordTable.Cell(current_row + 1, 5).Range.Text = ableResult.Excelent;
                wordTable.Cell(current_row + 1, 6).Range.Text = ableResult.Good;
                wordTable.Cell(current_row + 1, 7).Range.Text = ableResult.Satisfactory;
                wordTable.Cell(current_row + 1, 8).Range.Text = ableResult.Unsatisfactory;
                //владеть
                wordTable.Cell(current_row + 2, 5).Range.Text = ownResult.Excelent;
                wordTable.Cell(current_row + 2, 6).Range.Text = ownResult.Good;
                wordTable.Cell(current_row + 2, 7).Range.Text = ownResult.Satisfactory;
                wordTable.Cell(current_row + 2, 8).Range.Text = ownResult.Unsatisfactory;

                //форматирование - объединение строк, столбцов
                wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row + 2, 1));
                wordTable.Cell(current_row, 2).Merge(wordTable.Cell(current_row + 2, 2));
                wordTable.Cell(current_row, 3).Merge(wordTable.Cell(current_row + 2, 3));

                //переход новую компенцию
                current_row = current_row + 3;
            }
            //форматирование
            wordTable.Borders.Enable = Convert.ToInt32(true);
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.Font.ColorIndex = WdColorIndex.wdAuto;
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
        }

        //6.3.1.2 Содержание оценочного средства
        private void createTable7()
        {
            List<GeneralModel.CompetencyTestTasksClass> test_computer = generalModel.CompetencyTestTasks;

            DataTable dt = new DataTable();
            dt.Columns.Add("Код");
            dt.Columns.Add("Вопрос");

            int max_answers = test_computer.Max(a => a.Tasks.Count);
            for (int i = 1; i <= max_answers; i++)
                dt.Columns.Add(i + "");

            int row_count = 2 + test_computer.Count + test_computer.Select(a => a.SemesterNumber).Distinct().Count() * 2;
            for (int i = 1; i <= row_count; i++)
                dt.Rows.Add();

            app.Selection.Find.Execute("<TABLE7>");
            Word.Range wordRange = app.Selection.Range;
            Word.Table wordTable = wordDocument.Tables.Add(wordRange, dt.Rows.Count, dt.Columns.Count);

            // Шапка
            wordTable.Cell(1, 1).Range.Text = "Код компетенции";
            wordTable.Cell(1, 2).Range.Text = "Тестовые вопросы";
            wordTable.Cell(1, 3).Range.Text = "Варианты ответов";
            for (int i = 1; i <= max_answers; i++)
                wordTable.Cell(2, 2 + i).Range.Text = i.ToString();

            // Форматирование
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Borders.Enable = Convert.ToInt32(true);
            wordTable.Cell(1, 1).Merge(wordTable.Cell(2, 1));
            wordTable.Cell(1, 2).Merge(wordTable.Cell(2, 2));
            wordTable.Cell(1, 3).Merge(wordTable.Cell(1, max_answers + 2));

            int current_row = 3;
            foreach (var semester in generalModel.Semesters)
            {
                var list1 = test_computer.First(a => a.SemesterNumber == semester.SemesterNumber && a.moduleNumber == 1).Tasks;
                var list2 = test_computer.First(a => a.SemesterNumber == semester.SemesterNumber && a.moduleNumber == 2).Tasks;

                if (list1.Count is 0 && list2.Count is 0)
                    continue;
                //МОДУЛЬ_1
                wordTable.Cell(current_row, 1).Range.Text = "Дисциплинарный модуль " + semester.SemesterNumber + ".1";
                wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, max_answers + 2));
                wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
                current_row++;

                //данные
                wordTable.Cell(current_row, 1).Range.Text = string.Join(", ", generalModel.Competencies);
                wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
                wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row + list1.Count - 1, 1));

                foreach (var item in list1)
                {
                    wordTable.Cell(current_row, 2).Range.Text = item.Question;
                    wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    for (int i = 0; i < item.Answers.Count; i++)
                    {
                        wordTable.Cell(current_row, 3 + i).Range.Text = item.Answers[i];
                        wordTable.Cell(current_row, 3 + i).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    }

                    current_row++;
                }

                //МОДУЛЬ_2
                wordTable.Cell(current_row, 1).Range.Text = "Дисциплинарный модуль " + semester.SemesterNumber + ".2";
                wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, max_answers + 2));
                wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
                current_row++;

                //данные
                wordTable.Cell(current_row, 1).Range.Text = string.Join(", ", generalModel.Competencies);
                wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
                wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row + list2.Count - 1, 1));

                foreach (var item in list2)
                {
                    wordTable.Cell(current_row, 2).Range.Text = item.Question;
                    wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    for (int i = 0; i < item.Answers.Count; i++)
                    {
                        wordTable.Cell(current_row, 3 + i).Range.Text = item.Answers[i];
                        wordTable.Cell(current_row, 3 + i).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    }

                    current_row++;
                }

            }

            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitFixed);
        }

        //Экзамен 6.3.4.3. Содержание оценочного средства
        private void createTable8()
        {
            app.Selection.Find.Execute("<TABLE8>");
            var wordRange = app.Selection.Range;

            int colomn_count = 2 + generalModel.Competencies.Count();
            int row_count = 1 + generalModel.QuestionCodes.Count;

            Word.Table wordTable = wordDocument.Tables.Add(wordRange, row_count, colomn_count);
            wordTable.Borders.Enable = Convert.ToInt32(true);

            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            //заполнение
            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Примерные вопросы к экзамену";
            for (int i = 0; i < generalModel.Competencies.Count(); i++)
                wordTable.Cell(1, 3 + i).Range.Text = generalModel.Competencies[i];

            int current_row = 2;
            for (int i = 0; i < generalModel.QuestionCodes.Count; i++)
            {
                var question = generalModel.QuestionCodes[i];
                wordTable.Cell(current_row, 1).Range.Text = (i + 1).ToString();
                wordTable.Cell(current_row, 2).Range.Text = question.Question;
                for (int j = 0; j < generalModel.Competencies.Count(); j++)
                {
                    var current_competence = generalModel.Competencies[j];

                    for (int k = 0; k < question.Competencies.Count; k++)
                    {
                        if (question.Competencies[k] == current_competence)
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
        private void createTable9()
        {
            List<GeneralModel.TestTasksClass> test = generalModel.testTasks;
            
            DataTable dt = new DataTable();
            dt.Columns.Add("Код");
            dt.Columns.Add("Вопрос");
            int max_answers = test.Max(a => a.Tasks.Max(x => x.Answers.Count()));
            for (int i = 1; i <= max_answers; i++)
                dt.Columns.Add(i + "");
            int row_count = 2 + test.Sum(x => x.Tasks.Count());
            for (int i = 1; i <= row_count; i++)
                dt.Rows.Add();

            app.Selection.Find.Execute("<TABLE9>");
            Word.Range wordRange = app.Selection.Range;
            Word.Table wordTable = wordDocument.Tables.Add(wordRange, dt.Rows.Count, dt.Columns.Count);
            //шапка
            wordTable.Cell(1, 1).Range.Text = "Код компетенции";
            wordTable.Cell(1, 2).Range.Text = "Тестовые вопросы";
            wordTable.Cell(1, 3).Range.Text = "Варианты ответов";
            for (int i = 1; i <= max_answers; i++)
                wordTable.Cell(2, 2 + i).Range.Text = i.ToString();

            //форматирование
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Borders.Enable = Convert.ToInt32(true);
            wordTable.Cell(1, 1).Merge(wordTable.Cell(2, 1));
            wordTable.Cell(1, 2).Merge(wordTable.Cell(2, 2));
            wordTable.Cell(1, 3).Merge(wordTable.Cell(1, max_answers + 2));

            int current_row = 3;

            //данные
            foreach (var competencyTasks in test)
            {
                wordTable.Cell(current_row, 1).Range.Text = string.Join(", ", competencyTasks.CompetencyCode);
                wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);

                wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row + competencyTasks.Tasks.Count - 1, 1));

                foreach (var item in competencyTasks.Tasks)
                {
                    wordTable.Cell(current_row, 2).Range.Text = item.Question;
                    wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    for (int i = 0; i < item.Answers.Count; i++)
                    {
                        wordTable.Cell(current_row, 3 + i).Range.Text = item.Answers[i];
                        wordTable.Cell(current_row, 3 + i).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    }
                    current_row++;
                }
            }

            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitFixed);
        }

        //ДОРАБОТАТЬ
        private void createTable10()
        {
            string main_key = "<TABLE10>";
            app.Selection.Find.Execute(main_key);
            Word.Range wordRange = app.Selection.Range;

            Dictionary<int, Dictionary<string, string>> text_keys = new Dictionary<int, Dictionary<string, string>>();
            Dictionary<int, List<string>> table_keys = new Dictionary<int, List<string>>();

            //вставка тэгов в ворд
            foreach (var semester in generalModel.Semesters.Select(x => x.SemesterNumber))
            {
                List<string> list_key_text = new List<string>()
                {
                    $"<TEXT10_{semester}.0>",
                    $"<TEXT10_{semester}.1>",
                    $"<TEXT10_{semester}.2>"
                };
                List<string> list_key_table = new List<string>()
                {
                    $"<TABLE10_{semester}.0>",
                    $"<TABLE10_{semester}.1>",
                    $"<TABLE10_{semester}.2>",
                };

                text_keys.Add(semester, new Dictionary<string, string>()
                {
                    [list_key_text[0]] = $"Семестр {semester}",
                    [list_key_text[1]] = $"Дисциплинарный модуль {semester}.1",
                    [list_key_text[2]] = $"Дисциплинарный модуль {semester}.2",
                });

                table_keys.Add(semester, list_key_table);

                for (int i = 0; i < 3; i++)
                {
                    wordRange.Collapse(WdCollapseDirection.wdCollapseStart);
                    wordRange.InsertAfter(list_key_text[i]);
                    wordRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    wordRange.Bold = Convert.ToInt32(true);
                    wordRange.Font.Size = 14;
                    wordRange.InsertParagraphAfter();
                    wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                    wordRange.Collapse(WdCollapseDirection.wdCollapseStart);
                    wordRange.InsertAfter(list_key_table[i]);
                    wordRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    wordRange.Bold = Convert.ToInt32(false);
                    wordRange.Font.Size = 12;
                    wordRange.InsertParagraphAfter();
                    wordRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }
            }

            //удаление main_key из word
            app.Selection.Find.Wrap = WdFindWrap.wdFindContinue;
            app.Selection.Find.Execute(main_key);
            Word.Range wordRangeDelete = app.Selection.Range;
            wordRangeDelete.Delete();

            foreach (var item in text_keys)
                replaceText(item.Value);

            int current_lecture = 1;
            int current_laboratories = 1;
            int current_practical = 1;

            //замена тэгов на таблицы семестров
            foreach (var semester in table_keys)
            {
                List<string> current_keys = semester.Value;

                //ГЛАВНАЯ ТАБЛИЦА
                var dt_main = new DataTable();
                for (int i = 0; i < 5; i++)
                    dt_main.Rows.Add();
                for (int i = 0; i < 3; i++)
                    dt_main.Columns.Add();
                app.Selection.Find.Wrap = WdFindWrap.wdFindContinue;
                app.Selection.Find.Execute(current_keys[0]);
                Word.Range wordRangeTable = app.Selection.Range;
                var wordTable = wordDocument.Tables.Add(wordRangeTable,
                    dt_main.Rows.Count, dt_main.Columns.Count);
                wordTable.Cell(1, 1).Range.Text = "Дисциплинарный модуль";
                //!!!! лабораторные устные опросы ДОРАБОТАТЬ!!!!!!!!!
                wordTable.Cell(2, 1).Range.Text = "Текущий контроль (лабораторные работы, практические задачи)";
                wordTable.Cell(3, 1).Range.Text = "Текущий контроль (тестирование)";
                wordTable.Cell(4, 1).Range.Text = "Общее количество баллов";
                wordTable.Cell(5, 1).Range.Text = "Итоговый балл:";
                wordTable.Cell(1, 2).Range.Text = $"ДМ {semester.Key}.1";
                wordTable.Cell(1, 3).Range.Text = $"ДМ {semester.Key}.2";
                //!!!! Доработать, чтобы баллы брались $$откуда-то$$
                //баллы за практические, лабы, устные
                wordTable.Cell(2, 2).Range.Text = "9-15";
                wordTable.Cell(2, 3).Range.Text = "9-15";
                //баллы за тестирование
                wordTable.Cell(3, 2).Range.Text = "8-15";
                wordTable.Cell(3, 3).Range.Text = "9-15";
                //общее кол-во баллов
                wordTable.Cell(4, 2).Range.Text = "17-30";
                wordTable.Cell(4, 3).Range.Text = "18-30";
                //итоговый балл
                /* 
                string score = "";
                if (экзамен || зачет)
                    score = "35-60";
                else if(зачет с оценкой)
                    score = "35-60";
                */
                wordTable.Cell(5, 2).Range.Text = "35-60";
                //форматирование
                wordTable.Borders.Enable = Convert.ToInt32(true);
                wordTable.Range.ParagraphFormat.SpaceBefore = 0;
                wordTable.Range.ParagraphFormat.SpaceAfter = 0;
                wordTable.Cell(1, 1).Range.Bold = Convert.ToInt32(true);
                wordTable.Cell(1, 2).Range.Bold = Convert.ToInt32(true);
                wordTable.Cell(1, 3).Range.Bold = Convert.ToInt32(true);
                wordTable.Cell(4, 1).Range.Bold = Convert.ToInt32(true);
                wordTable.Cell(4, 2).Range.Bold = Convert.ToInt32(true);
                wordTable.Cell(4, 3).Range.Bold = Convert.ToInt32(true);
                wordTable.Cell(5, 1).Range.Bold = Convert.ToInt32(true);
                wordTable.Cell(5, 2).Range.Bold = Convert.ToInt32(true);
                for (int i = 1; i <= 5; i++)
                    wordTable.Cell(i, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                for (int i = 1; i <= 5; i++)
                    for (int j = 2; j <= 3; j++)
                        wordTable.Cell(i, j).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                wordTable.Cell(5, 2).Merge(wordTable.Cell(5, 3));
                wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);

                //ТАБЛИЦЫ - дисциплинарный модуль1,2
                for (int module = 1; module <= 2; module++)
                {
                    List<ChildThemeModel> lecture_list = discipline.themes.Where(a => a.semester.Equals(semester.Key) && a.module.Equals(module) && a.lectures is not null).SelectMany(a => a.lectures.Where(b => b.score is not 0)).ToList();
                    List<ChildThemeModel> laboratory_list = discipline.themes.Where(a => a.semester.Equals(semester.Key) && a.module.Equals(module) && a.laboratories is not null).SelectMany(a => a.laboratories.Where(b => b.score is not 0)).ToList();
                    List<ChildThemeModel> practical_list = discipline.themes.Where(a => a.semester.Equals(semester.Key) && a.module.Equals(module) && a.practicals is not null).SelectMany(a => a.practicals.Where(b => b.score is not 0)).ToList();
                    int row = 2 + lecture_list.Count + laboratory_list.Count + practical_list.Count + 4;
                    int column = 3;
                    app.Selection.Find.Wrap = WdFindWrap.wdFindContinue;
                    app.Selection.Find.Execute(current_keys[module]);
                    wordRangeTable = app.Selection.Range;
                    wordTable = wordDocument.Tables.Add(wordRangeTable, row, column);
                    wordTable.Cell(1, 1).Range.Text = "№п/п";
                    wordTable.Cell(1, 2).Range.Text = "Виды работ";
                    wordTable.Cell(1, 3).Range.Text = "Максимальный балл";
                    wordTable.Cell(2, 1).Range.Text = "Текущий контроль";
                    //форматирование
                    wordTable.Borders.Enable = Convert.ToInt32(true);
                    wordTable.Range.ParagraphFormat.SpaceBefore = 0;
                    wordTable.Range.ParagraphFormat.SpaceAfter = 0;
                    wordTable.Range.Font.Size = 12;
                    wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                    wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    wordTable.Cell(1, 1).Range.Bold = Convert.ToInt32(true);
                    wordTable.Cell(1, 2).Range.Bold = Convert.ToInt32(true);
                    wordTable.Cell(1, 3).Range.Bold = Convert.ToInt32(true);
                    wordTable.Cell(2, 1).Range.Bold = Convert.ToInt32(true);
                    wordTable.Cell(2, 1).Merge(wordTable.Cell(2, 3));
                    int current_row = 3;
                    if (lecture_list is not null)
                        for (int index = 0; index < lecture_list.Count; index++)
                        {
                            wordTable.Cell(current_row, 1).Range.Text = (index + 1).ToString();
                            wordTable.Cell(current_row, 2).Range.Text = $"Лекция-{current_lecture} {lecture_list[index].name}";
                            wordTable.Cell(current_row, 3).Range.Text = lecture_list[index].score.ToString();
                            //форматирование
                            wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                            current_lecture++;
                            current_row++;
                        }
                    if (laboratory_list is not null)
                        for (int index = 0; index < laboratory_list.Count; index++)
                        {
                            wordTable.Cell(current_row, 1).Range.Text = (index + 1).ToString();
                            wordTable.Cell(current_row, 2).Range.Text = $"Л.Р.-{current_laboratories} {laboratory_list[index].name}";
                            wordTable.Cell(current_row, 3).Range.Text = laboratory_list[index].score.ToString();
                            //форматирование
                            wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                            current_laboratories++;
                            current_row++;
                        }
                    if (practical_list is not null)
                        for (int index = 0; index < practical_list.Count; index++)
                        {
                            wordTable.Cell(current_row, 1).Range.Text = (index + 1).ToString();
                            wordTable.Cell(current_row, 2).Range.Text = $"П.З.-{current_practical} {practical_list[index].name}";
                            wordTable.Cell(current_row, 3).Range.Text = practical_list[index].score.ToString();
                            //форматирование
                            wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                            current_practical++;
                            current_row++;
                        }
                    wordTable.Cell(current_row, 1).Range.Text = "Итого:";
                    wordTable.Cell(current_row, 3).Range.Text = "15";
                    //форматирование
                    wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
                    wordTable.Cell(current_row, 3).Range.Bold = Convert.ToInt32(true);
                    wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, 2));
                    wordTable.Cell(current_row, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    current_row++;
                    wordTable.Cell(current_row, 1).Range.Text = "Текущий контроль";
                    //форматирование
                    wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
                    wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, 3));
                    current_row++;
                    wordTable.Cell(current_row, 1).Range.Text = "1";
                    wordTable.Cell(current_row, 2).Range.Text = "Тестирование";
                    wordTable.Cell(current_row, 3).Range.Text = "15";
                    //форматирование
                    wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    wordTable.Cell(current_row, 3).Range.Bold = Convert.ToInt32(true);
                    current_row++;
                    wordTable.Cell(current_row, 1).Range.Text = $"Итого по ДМ {semester.Key}.{module}";
                    wordTable.Cell(current_row, 3).Range.Text = "30";
                    //форматирование
                    wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
                    wordTable.Cell(current_row, 3).Range.Bold = Convert.ToInt32(true);
                    wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, 2));
                    wordTable.Cell(current_row, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    //финальное форматирование
                    wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);

                }
            }
        }

        private void createTable11()
        {
            GeneralModel.LiteratureBooksModel literatureBooks = generalModel.LiteratureBooks;

            //основная литература
            literatureBooks.Main = new List<GeneralModel.LiteratureModel>()
            {
                new GeneralModel.LiteratureModel("Мирный, В. И. Законодательная метрология : учебное пособие / В. И. Мирный", 101, 1),
                new GeneralModel.LiteratureModel("Воробьева, Г. Н. Метрология, стандартизация и сертификация", "http://www.iprbookshop.ru/57097.html", 0),
                new GeneralModel.LiteratureModel("Семенов, И. В. Метрология, стандартизация и сертификация", "https://www.iprbookshop.ru/115857.html", 1),
            };
            //дополнительная литература
            literatureBooks.Additional = new List<GeneralModel.LiteratureModel>()
            {
                new GeneralModel.LiteratureModel("Лепявко, А. П. Измерительные преобразователи давления", "https://www.iprbookshop.ru/118037.html", 1),
                new GeneralModel.LiteratureModel("Мирный, В. И. Прикладная метрология : учебное пособие / В. И. Мирный", "https://www.iprbookshop.ru/118037.html", 1),
                new GeneralModel.LiteratureModel("Многофазные испытательные стенды газожидкостных смесей: метрология, моделирование, подобие : монография", "https://www.iprbookshop.ru/118037.html", 1),
            };
            //учебно-методические издания
            literatureBooks.Methodical = new List<GeneralModel.LiteratureModel>()
            {
                new GeneralModel.LiteratureModel("Ситдикова И.П., Ахметзянов Р.Р. Метрология, стандартизация и сертификация: методические указания для выполнения лабораторных работ", "http://elibrary.agni-rt.ru", 1),
                new GeneralModel.LiteratureModel("Ситдикова И.П., Ахметзянов Р.Р. Метрология, стандартизация и сертификация: методические указания по выполнению курсовой работы", "http://elibrary.agni-rt.ru", 1),
                new GeneralModel.LiteratureModel("Ситдикова И.П., Ахметзянов Р.Р. Метрология, стандартизация и сертификация: методические указания для проведения практических занятий", "http://elibrary.agni-rt.ru", 1),
            };

            List<GeneralModel.LiteratureModel> main = literatureBooks.Main;
            List<GeneralModel.LiteratureModel> additional = literatureBooks.Additional;
            List<GeneralModel.LiteratureModel> methodical = literatureBooks.Methodical;

            int row = 2 + main.Count + 1 + additional.Count + 1 + methodical.Count;
            int column = 4;
            app.Selection.Find.Execute("<TABLE11>");
            Word.Range wordTableRange = app.Selection.Range;
            Word.Table wordTable = app.ActiveDocument.Tables.Add(wordTableRange, row, column);
            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Библиографическое описание";
            wordTable.Cell(1, 3).Range.Text = "Количество печатных экземпляров или адрес электронного ресурса";
            wordTable.Cell(1, 4).Range.Text = "Коэффициент обеспеченности";
            //форматирование
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
            int current_row = 2;
            //основная литература
            wordTable.Cell(current_row, 1).Range.Text = "Основная литература";
            wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
            wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, 4));
            current_row++;
            for (int i = 0; i < main.Count; i++)
            {
                wordTable.Cell(current_row, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(current_row, 2).Range.Text = $"{main[i].name}";
                wordTable.Cell(current_row, 3).Range.Text = main[i].link is not null ? $"Режим доступа:\n{main[i].link}" : $"{main[i].count} экз.";
                wordTable.Cell(current_row, 4).Range.Text = main[i].coefficient is 0 ? "" : $"{main[i].coefficient}";
                //форматирование
                wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(current_row, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                current_row++;
            }
            //дополнительная литература
            wordTable.Cell(current_row, 1).Range.Text = "Дополнительная литература";
            wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
            wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, 4));
            current_row++;
            for (int i = 0; i < additional.Count; i++)
            {
                wordTable.Cell(current_row, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(current_row, 2).Range.Text = $"{additional[i].name}";
                wordTable.Cell(current_row, 3).Range.Text = additional[i].link is not null ? $"Режим доступа:\n{additional[i].link}" : $"{additional[i].count} экз.";
                wordTable.Cell(current_row, 4).Range.Text = additional[i].coefficient is 0 ? "" : $"{additional[i].coefficient}";
                //форматирование
                wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(current_row, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                current_row++;
            }
            //Учебно-методические издания
            wordTable.Cell(current_row, 1).Range.Text = "Учебно-методические издания";
            wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
            wordTable.Cell(current_row, 1).Merge(wordTable.Cell(current_row, 4));
            current_row++;
            for (int i = 0; i < methodical.Count; i++)
            {
                wordTable.Cell(current_row, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(current_row, 2).Range.Text = $"{methodical[i].name}";
                wordTable.Cell(current_row, 3).Range.Text = methodical[i].link is not null ? $"Режим доступа:\n{methodical[i].link}" : $"{methodical[i].count} экз.";
                wordTable.Cell(current_row, 4).Range.Text = methodical[i].coefficient is 0 ? "" : $"{methodical[i].coefficient}";
                //форматирование
                wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(current_row, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                current_row++;
            }

            //wordTable.Range.Rows[1].Height = app.CentimetersToPoints(3.22f);
            //wordTable.Range.Columns[4].Width = app.CentimetersToPoints(1.28f);


            wordTable.Borders.Enable = Convert.ToInt32(true);
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.Font.Size = 12;
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
        }


        private void createTable12()
        {
            generalModel.SiteList = new List<GeneralModel.LiteratureModel>()
            {
                new GeneralModel.LiteratureModel("Учебно-методическая литература для учащихся и студентов, размещенная на сайте «Studmed.ru»", "http://www.studmed.ru "),
                new GeneralModel.LiteratureModel("Единое окно доступа к информационным ресурсам", "http://window.edu.ru/ "),
                new GeneralModel.LiteratureModel("Российская государственная библиотека", "http://www.rsl.ru "),
                new GeneralModel.LiteratureModel("Электронная библиотека Elibrary", "http://elibrary.ru "),
                new GeneralModel.LiteratureModel("Электронно-библиотечная система IPRbooks", "http://elibrary.ru "),
                new GeneralModel.LiteratureModel("Электронная библиотека АГНИ", "http://elibrary.agni-rt.ru "),
                new GeneralModel.LiteratureModel("Энциклопедия России «Библиотекарь»", "http://bibliotekar.ru "),
            };

            List<GeneralModel.LiteratureModel> site_list = generalModel.SiteList;

            int row = 1 + site_list.Count;
            int column = 3;
            app.Selection.Find.Execute("<TABLE12>");
            Word.Range wordTableRange = app.Selection.Range;
            Word.Table wordTable = wordDocument.Tables.Add(wordTableRange, row, column);
            //форматирование
            wordTable.Borders.Enable = Convert.ToInt32(true);
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            wordTable.Rows[1].Range.Bold = Convert.ToInt32(true);
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            wordTable.Cell(1, 1).Range.Text = "№ п/п";
            wordTable.Cell(1, 2).Range.Text = "Наименование";
            wordTable.Cell(1, 3).Range.Text = "Алрес в Интернете";
            int current_row = 2;
            for (int i = 0; i < site_list.Count; i++)
            {
                wordTable.Cell(current_row, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(current_row, 2).Range.Text = $"{site_list[i].name}";
                wordTable.Cell(current_row, 3).Range.Text = $"{site_list[i].link}";
                //форматирование
                wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(current_row, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                current_row++;
            }
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
        }

        private void createTable13() //10
        {
            List<SoftwareModel> software_list = new List<SoftwareModel>()
            {
                new SoftwareModel("Microsoft Office Professional Plus 2016 Rus Academic OLP (Word, Excel, PowerPoint, Access)", "№67892163 от 26.12.2016г.", "№0297/136 от 23.12.2016г."),
                new SoftwareModel("Microsoft Office Standard 2016 Rus Academic OLP (Word, Excel, PowerPoint)", "№67892163 от 26.12.2016г.", "№0297/136 от 23.12.2016г."),
                new SoftwareModel("Microsoft Windows Professional 10 Rus Upgrade Academic OLP", "№67892163 от 26.12.2016г.", "№0297/136 от 23.12.2016г."),
                new SoftwareModel("ABBYY Fine Reader 12 Professional", "№197059 от 26.12.2016г.", "№0297/136 от 23.12.2016г."),
                new SoftwareModel("Kaspersky Endpoint Security для бизнеса – Стандартный Russian Edition", "№ 24С4-221222-121357-913-1225", "№691447/581-2022 от 16.12.2022г."), //Доработать текущий год поставить
                new SoftwareModel("Электронно-библиотечная система IPRbooks", "", "Лицензионный договор №409-2022 от 03.11.2022г."),
                new SoftwareModel("Образовательная платформа для подготовки кадров в цифровой экономике DATALIB.RU", "", "Лицензионный договор №428-2022/22d/B от 09.11.2022г."),
                new SoftwareModel("ПО «Автоматизированная тестирующая система", "Свидетельство государственной регистрации программ для ЭВМ №2014614238 от 01.04.2014г.", ""),
            };

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
            int current_row = 2;
            for (int i = 0; i < software_list.Count; i++)
            {
                wordTable.Cell(current_row, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(current_row, 2).Range.Text = $"{software_list[i].name}";
                wordTable.Cell(current_row, 3).Range.Text = $"{software_list[i].license}";
                wordTable.Cell(current_row, 4).Range.Text = $"{software_list[i].contract}";

                // форматирование
                wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(current_row, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(current_row, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                current_row++;
            }
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
        }

        private class SoftwareModel //10
        {
            public string name { get; set; }
            public string license { get; set; }
            public string contract { get; set; }

            public SoftwareModel(string name, string license, string contract)
            {
                this.name = name;
                this.license = license;
                this.contract = contract;
            }
        }

        //11 материально техническая база
        private void createTable14()
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
            int current_row = 2;
            for (int i = 0; i < generalModel.PlaceTheirEquipments.Count; i++)
            {
                wordTable.Cell(current_row, 1).Range.Text = $"{i + 1}";
                wordTable.Cell(current_row, 2).Range.Text = $"{generalModel.PlaceTheirEquipments[i].PlaceName}";
                string equipment = "";
                for (int j = 0; j < generalModel.PlaceTheirEquipments[i].EquipmentsName.Count; j++)
                {
                    if (generalModel.PlaceTheirEquipments[i].EquipmentsName.Count == 1)
                        equipment = generalModel.PlaceTheirEquipments[i].EquipmentsName[j];
                    else
                        equipment = equipment + $"{j + 1}. {generalModel.PlaceTheirEquipments[i].EquipmentsName[j]}\n";
                }
                wordTable.Cell(current_row, 3).Range.Text = equipment;
                //форматирование
                wordTable.Cell(current_row, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordTable.Cell(current_row, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                current_row++;
            }
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
        }

        private void createTable16()
        {
            List<Semester> semesters = generalModel.Semesters;

            //Место дисциплины в структуру ОПОП ВО
            string place = 
                generalModel.Index + ". "
                + "Дисциплина «" + generalModel.DisciplineName + "» входит в состав "
                + generalModel.ParentBlock_1 + " и относится к "
                + generalModel.ParentSubBlock_1 + " части. "
                + "\nДисциплина изучается на "
                + string.Join(", ", semesters.Select(semester => ((int)((semester.SemesterNumber - 1) / 2) + 1).ToString() + " курсе в " + semester.SemesterNumber + " семестре")) + ".";

            //общая трудоемкость дисциплины (в зачетных единицах в часах)
            string laboriousness =
                "Зачетных единиц по учебному плану: " + generalModel.Expert + " ЗЕ.\n"
                + "Часов по учебному плану: "
                + (Convert.ToInt32(generalModel.Actual) * Convert.ToInt32(generalModel.HoursPerCreditUnit)).ToString() + " ч.\n";

            //виды учебной работы
            string work = "Контактная работа обучающихся с преподавателем:\r\n"
                + "\r\n- лекции " + semesters.Sum(semester => Convert.ToInt32(semester.Lectures)).ToString() + " ч.;"
                + "\r\n- практические занятия " + semesters.Sum(semester => Convert.ToInt32(semester.PracticeWorks)).ToString() + " ч.;"
                + "\r\n- лабораторные работы " + semesters.Sum(semester => Convert.ToInt32(semester.LaboratoryWorks)).ToString() + " ч."
                + "\r\nСамостоятельная работа " + semesters.Sum(semester => Convert.ToInt32(semester.IndependentWork)).ToString() + " ч."
                + "\r\nКонтроль (экзамен) " + semesters.Sum(semester => Convert.ToInt32(semester.Control)).ToString() + " ч.";

            //изучаемые темы (разделы)
            string theme = string.Join("\r\n", generalModel.DisciplineThematicPlan.Select(x => x.ThemeName)) + "\r\n";
            
            //форма промежуточной аттестации
            string attestation = "экзамен в 4 семестре";
            Dictionary<string, string> tableData = new Dictionary<string, string>()
            {
                { "Место дисциплины в структуру ОПОП ВО", place},
                { "Общая трудоемкость дисциплины (в зачетных единицах в часах)", laboriousness},
                { "Виды учебной работы", work},
                { "Изучаемые темы (разделы)", theme},
                { "Форма промежуточной аттестации", attestation},

            };
            int row = tableData.Count;
            int column = 2;
            app.Selection.Find.Execute("<TABLE16>");
            Word.Range wordTableRange = app.Selection.Range;
            Word.Table wordTable = wordDocument.Tables.Add(wordTableRange, row, column);
            //форматирование
            wordTable.Borders.Enable = Convert.ToInt32(true);
            wordTable.Range.ParagraphFormat.SpaceBefore = 0;
            wordTable.Range.ParagraphFormat.SpaceAfter = 0;
            wordTable.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            wordTable.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            wordTable.Range.Font.Size = 12;
            int current_row = 1;
            foreach (var item in tableData)
            {
                wordTable.Cell(current_row, 1).Range.Text = item.Key;
                wordTable.Cell(current_row, 2).Range.Text = item.Value;
                //форматирование
                wordTable.Cell(current_row, 1).Range.Bold = Convert.ToInt32(true);
                current_row++;
            }
            wordTable.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
        }
    }
}
