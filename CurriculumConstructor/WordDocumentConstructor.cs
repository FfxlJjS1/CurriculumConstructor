using System;
using System.Collections.Generic;
using System.Linq;
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

        }

        public void PreviewWordFile()
        {

        }
    }

    public class DecumentReplaceObject
    {
        // Variables with object data type will restructured so don't use it
        public string Discipline;
        public string Index;
        public string Direction;
        public string Profile;
        public string Qualification;
        public string FormStrude;
        public string LanguageStudy;
        public string YearStart;
        public string Author;
        public string Reviewer;
        public string DepartmentChar;
        public string Year;
        public object TableOfFormedCompetenciesOfTheDiscipline;
        public int CurriculumHours;
        public int TeacherContactTime;
        public int LectoreTime;
        public int PricticeTime;
        public int SelfworkTime;
        public int ExamTime;
        public object DisciplineThematicPlanTable;
        public object DisciplineContentTable;
        public object LaboratoryExaluationCriteria86;
        public object LaboratoryExaluationCriteria71;
        public object LaboratoryExaluationCriteria55;
        public object LaboratoryExaluationCriteria0;
        public string LaboratoryExercise;
        public object PracticeExaluationCriteria86;
        public object PracticeExaluationCriteria71;
        public object PracticeExaluationCriteria55;
        public object PracticeExaluationCriteria0;
        public object EvaluationToolContentTable;
        public object TableOfSampleVariantsOfTestItems;
        public object AdditionalPointsForActivities;


    }
}
