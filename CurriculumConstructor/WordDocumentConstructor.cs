using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;

namespace CurriculumConstructor
{
    internal class WordDocumentConstructor
    {
        static string[] MainContents =
        {
            "Перечень планируемых результатов обучения по дисциплине, соотнесенных с планируемыми результатами освоения образовательной программы",
            "Место дисциплины в структуре основной профессиональной образовательной программы высшего образования",
            "Объем дисциплины в зачетных единицах с указанием количества часов, выделенных на контактную работу обучающихся с преподавателем и на самостоятельную работу обучающихся",
            "Содержание дисциплины, структурированное по темам (разделам) с указанием отведенного на них количества академических часов и видов учебных занятий",
            "Перечень учебно-методического обеспечения для самостоятельной работы обучающихся по дисциплине",
            "Фонд оценочных средств по дисциплине",
            "Перечень основной, дополнительной учебной литературы и учебно-методических изданий, необходимых для освоения дисциплины",
            "Перечень профессиональных баз данных, информационных справочных систем и информационных ресурсов, необходимых для освоения дисциплины",
            "Методические указания для обучающихся по освоению дисциплины",
            "Перечень программного обеспечения",
            "Материально-техническая база, необходимая для осуществления образовательного процесса по дисциплине",
            "Средства адаптации преподавания дисциплины к потребностям обучающихся лиц с ограниченными возможностями здоровья"
        };

        static string[] Contents4 =
        {
            "Структура и тематический план контактной и самостоятельной работы по дисциплине",
            "Содержание дисциплины"
        };

        static string[] SubContents4 =
        {
            "Структура и тематический план контактной и самостоятельной работы по дисциплине",
            "Содержание дисциплины"
        };

        static string[] Contents6 =
        {
            "Перечень оценочных средств",
            "Уровень освоения компетенций и критерии оценивания результатов обучения",
            "Варианты оценочных средств",
            "Методические материалы, определяющие процедуры оценивания знаний, умений, навыков, характеризующих этапы формирования компетенций"
        };

        static string[] SubContents6 =
        {
            "Перечень оценочных средств",
            "Уровень освоения компетенций и критерии оценивания результатов обучения",
            "Варианты оценочных средств "
        };

        static string[] Contentd12 =
        {
            "ПРИЛОЖЕНИЯ",
            "Приложение 1. Аннотация рабочей программы дисциплины",
            "Приложение 2. Лист внесения изменений",
            "Приложение 3. Фонд оценочных средств"
        };

        public void ExportWordToFile(string WordFileName)
        {
            try
            {
                var app = new Word.Application();
                var wrodDocument = app.Documents.Open("shablon.docx");

                // Replace simple text
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                keyValuePairs.Add("<INDEX>", DocumentReplaceObject.Index);
                keyValuePairs.Add("<DISCIPLINE>", DocumentReplaceObject.Discipline);
                keyValuePairs.Add("<DIRECTION>", DocumentReplaceObject.Direction);
                keyValuePairs.Add("<PROFILE>", DocumentReplaceObject.Profile);
                keyValuePairs.Add("<QUALIFICATION>", DocumentReplaceObject.Qualification);
                keyValuePairs.Add("<FORM_STUDY>", DocumentReplaceObject.FormStudy);
                keyValuePairs.Add("<LANGUAGE_STUDY>", DocumentReplaceObject.LanguageStudy);
                keyValuePairs.Add("<YEAR_START>", DocumentReplaceObject.YearStart);
                keyValuePairs.Add("<AUTHOR>", DocumentReplaceObject.Author);
                //keyValuePairs.Add("<AUTHOR'S>", DocumentReplaceObject.);
                keyValuePairs.Add("<REVIEWER>", DocumentReplaceObject.Reviewer);
                keyValuePairs.Add("<DEPARTMENT_CHAIR>", DocumentReplaceObject.DepartmentChar);
                keyValuePairs.Add("<YEAR>", DocumentReplaceObject.Year);
                keyValuePairs.Add("<CURRICULUM_HOURS>", DocumentReplaceObject.CurriculumHours.ToString());
                keyValuePairs.Add("<TEACHER_CONTACT_TIME>", DocumentReplaceObject.TeacherContactTime.ToString());
                keyValuePairs.Add("<LECTORE_TIME>", DocumentReplaceObject.LectoreTime.ToString());
                keyValuePairs.Add("<PRACTICE_TIME>", DocumentReplaceObject.PracticeTime.ToString());
                keyValuePairs.Add("<LABORATORY_TIME>", DocumentReplaceObject.LaboratoryTime.ToString());
                keyValuePairs.Add("<SELFWORK_TIME>", DocumentReplaceObject.SelfworkTime.ToString());
                keyValuePairs.Add("<EXAM_TIME>", DocumentReplaceObject.ExamTime.ToString());

                foreach (var item in keyValuePairs)
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
                        MatchSoundsLike: Type.Missing,
                        MatchAllWordForms: false,
                        Forward: true,
                        Wrap: wrap,
                        Format: false,
                        ReplaceWith: Type.Missing, Replace: replace
                    );
                }

                // Set tables with data

            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка! " + ex.Message);
            }
        }

        public void PreviewWordFile()
        {

        }
    }

    public static class DocumentReplaceObject
    {
        // Variables with object data type will restructured so don't use it
        public static string Discipline;
        public static string Index;
        public static string Direction;
        public static string Profile;
        public static string Qualification;
        public static string FormStudy;
        public static string LanguageStudy;
        public static string YearStart;
        public static string Author;
        public static string Reviewer;
        public static string DepartmentChar;
        public static string Year;
        public static object TableOfFormedCompetenciesOfTheDiscipline;
        public static int CurriculumHours;
        public static int TeacherContactTime;
        public static int LectoreTime;
        public static int LaboratoryTime;
        public static int PracticeTime;
        public static int SelfworkTime;
        public static int ExamTime;
        public static object DisciplineThematicPlanTable;
        public static object DisciplineContentTable;
        public static object LaboratoryExaluationCriteria86;
        public static object LaboratoryExaluationCriteria71;
        public static object LaboratoryExaluationCriteria55;
        public static object LaboratoryExaluationCriteria0;
        public static string LaboratoryExercise;
        public static object PracticeExaluationCriteria86;
        public static object PracticeExaluationCriteria71;
        public static object PracticeExaluationCriteria55;
        public static object PracticeExaluationCriteria0;
        public static object EvaluationToolContentTable;
        public static object TableOfSampleVariantsOfTestItems;
        public static object AdditionalPointsForActivities;


    }
}
