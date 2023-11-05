using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CurriculumConstructor.SettingMenu.Model
{
    public class GeneralModel
    {
        public GeneralModel((string, string) Block_Part, TitleData titleData, DisciplineRow disciplineRow)
        {
            ParentBlock = Block_Part.Item1;
            ParentSubBlock = Block_Part.Item2;

            ProfileNumber = titleData.ProfileNumber;
            ProfileName = titleData.ProfileName;
            Qualification = titleData.Qualification;
            DepartmentName = titleData.DepartmentName;
            EducationForm = titleData.EducationForm;
            EducationPeriod = titleData.EducationPeriod;
            StartYear = titleData.StartYear;

            Index = disciplineRow.Index;
            DisciplineName = disciplineRow.DisciplineName;
            DepartmentName = disciplineRow.DisciplineName;
            Competencies = disciplineRow.Competencies;

            Exam = disciplineRow.Exam;
            Offset = disciplineRow.Offset;
            OffsetWithMark = disciplineRow.OffsetWithMark;
            Control = disciplineRow.Control;
            Expert = disciplineRow.Expert;
            Actual = disciplineRow.Actual;
            HoursPerCreditUnit = disciplineRow.HoursPerCreditUnit;
            ContansHours = disciplineRow.ContansHours;

            Semesters = disciplineRow.Semesters;
        }

        // Block and sub block of discipline
        public string ParentBlock { get; set; } = "";
        public string ParentBlock_1 { get; set; } = "";

        public string ParentSubBlock { get; set; } = "";
        public string ParentSubBlock_1 { get; set; } = "";


        // Undefined but need
        public string Exam { get; set; } = "";
        public string Offset { get; set; } = "";
        public string OffsetWithMark { get; set; } = "";
        public string Control{ get; set; } = "";
        public string Expert{ get; set; } = "";
        public string Actual { get; set; } = "";
        public int HoursPerCreditUnit { get; set; }
        public int ContansHours { get; set; }


        public List<Semester> Semesters { get; set; }
        public string[] Competencies { get; set; }

        // Title
        public string Index { get; set; } = "";
        public string Author { get; set; } = "";
        public string AuthorInTheInstrumentalCase { get; set; } = "";
        public string Reviewer { get; set; } = "";
        public string Head { get; set; } = "";
        public string DepartmentChair { get; set; } = "";
        public string ProfileNumber { get; set; } = ""; 
        public string ProfileName { get; set; } = "";
        public string DisciplineName { get; set; } = "";

        public string Qualification { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string EducationForm { get; set; } = "";
        public string EducationPeriod { get; set; } = "";
        public string StartYear { get; set; } = "";


        // 1
        public List<CompetencyPlanningResult> competencyPlanningResults { get; set; } = new List<CompetencyPlanningResult>();

        public class CompetencyPlanningResult
        {
            public string Code { get; set; } = "";
            public string CodeName { get; set; } = "";

            public string CompetencyAchivmentIndicators { get; set; } = "";

            public string ToKnowResult { get; set; } = "";
            public string ToAbilityResult { get; set; } = "";
            public string ToOwnResult { get; set; } = "";
        }


        // 4. Thematic plan of discipline
        public List<DisciplineThematicTheme> DisciplineThematicPlan { get; set; } = new List<DisciplineThematicTheme>();

        public int NeedTotalLectureHours { get; set; }
        public int NeedTotalPracticeHours { get; set; }
        public int NeedTotalLaboratoryWorkHours { get; set; }
        public int NeedTotalIndependentHours { get; set; }

        public class DisciplineThematicTheme
        {
            public string ThemeName { get; set; } = "";
            public int Semester { get; set; }
            public int SemesterModule { get; set; }
            public int LectureHours { get; set; }
            public int PracticeHours { get; set; }
            public int LaboratoryWorkHours { get; set; }
            public int IndependentHours { get; set; }

            public int AllHour
            {
                get
                {
                    return LectureHours + PracticeHours + LaboratoryWorkHours + IndependentHours;
                }
            }

            // 4.2
            public List<ThemeContent> ThemeContents { get; set; } = new List<ThemeContent>();

            public class ThemeContent
            {
                public enum ThemeTypeEnum
                {
                    Lecture,
                    PracticeWork,
                    LaboratoryWork,
                }

                public ThemeTypeEnum ThemeType { get; set; }
                public string ThemeText { get; set; } = "";
                public string UsingMethod { get; set; } = "";
                public string[] FormingCompetency { get; set; } = new string[0];
            }
        }


        // 5.
        public string MethodBook { get; set; } = "Ситдикова И.П., Ахметзянов Р.Р. Метрология, стандартизация и сертификация: методические указания для выполнения лабораторных работ и организации самостоятельной работы по дисциплине «Метрология, стандартизация и сертификация» для бакалавров направления подготовки 15.03.04 «Автоматизация технологических процессов и производств» очной формы обучения. – Альметьевск: АГНИ, 2021г.";


        // Test tasks. 6.3.1.2
        class CompetencyTestTasks
        {
            public string CompetencyCode { get; set; } = "";

            public List<TestTaskLine> Tasks { get; set; } = new List<TestTaskLine>();

            public class TestTaskLine
            {
                public string Question { get; set; } = "";
                public string Answer1 { get; set; } = "";
                public string Answer2 { get; set; } = "";
                public string Answer3 { get; set; } = "";
                public string Answer4 { get; set; } = "";

            }
        }

        // 6.3.2.2 - 6.3.2.3
        public EvaluationCriteriesClass EvaluationCriteries { get; set; } = new EvaluationCriteriesClass();

        public class EvaluationCriteriesClass
        {
            public string CriteriaForExcellent { get; set; } = "";
            public string CriteriaForGood { get; set; } = "";
            public string CriteriaForЫatisfactory { get; set; } = "";
            public string CriteriaForUnsatisfactory { get; set; } = "";

            public string TaskAndQuestionExampleForDefenceLabWork { get; set; } = "";
            public string TaskTextExampleForDefenceLabWork { get; set; } = "";
            public List<string> QuestionsExampleForDefenceLabWork { get; set; } = new List<string>();
        }


        // 6.3.4.3
        public List<QuestionCodesClass> QuestionCodes { get; set; } = new List<QuestionCodesClass>();

        public class QuestionCodesClass
        {
            public string Question { get; set; } = "";
            public List<string> Competencies { get; set; } = new List<string>();
        }


        // 6.4
        public List<string> AdditionaPointsForActivity { get; set; } = new List<string>();


        // 11
        public List<PlaceTheirEquipmentsClass> PlaceTheirEquipments = new List<PlaceTheirEquipmentsClass>();

        public class PlaceTheirEquipmentsClass
        {
            public string PlaceName { get; set; } = "";
            public List<string> EquipmentsName { get; set; } = new List<string>();
        }
    }
}
