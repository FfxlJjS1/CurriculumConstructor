using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CurriculumConstructor.SettingMenu.Model.GeneralModel;

namespace CurriculumConstructor.SettingMenu.Model
{
    public class GeneralModel
    {
        public GeneralModel((string, string) Block_Part, TitleDataClass titleData, DisciplineRow disciplineRow)
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

            competencyCode_Names = titleData.CompetencyCode_Names;


            Index = disciplineRow.Index;
            DisciplineName = disciplineRow.DisciplineName;
            DepartmentName = disciplineRow.DisciplineName;
            DisciplineCompetencies = disciplineRow.Competencies;

            competencyPlanningResults.AddRange(DisciplineCompetencies.Select(x => new CompetencyPlanningResult(x)));


            Exam = disciplineRow.Exam;
            Offset = disciplineRow.Offset;
            OffsetWithMark = disciplineRow.OffsetWithMark;
            Control = disciplineRow.Control;
            Expert = disciplineRow.Expert;
            Actual = disciplineRow.Actual;
            HoursPerCreditUnit = disciplineRow.HoursPerCreditUnit;
            ContansHours = disciplineRow.ContansHours;

            Semesters = disciplineRow.Semesters;

            NeedTotalLectureHours = disciplineRow.Semesters.Sum(x => Convert.ToInt32(x.Lectures));
            NeedTotalPracticeHours = disciplineRow.Semesters.Sum(x => Convert.ToInt32(x.PracticeWorks));
            NeedTotalLaboratoryWorkHours = disciplineRow.Semesters.Sum(x => Convert.ToInt32(x.LaboratoryWorks));
            NeedTotalIndependentHours = disciplineRow.Semesters.Sum(x => Convert.ToInt32(x.IndependentWork));
        }

        // Block and sub block of discipline
        public string ParentBlock { get; set; } = ""; // Excel
        public string ParentBlock_1 { get; set; } = "";

        public string ParentSubBlock { get; set; } = ""; // Excel
        public string ParentSubBlock_1 { get; set; } = "";


        // Undefined but need
        public string Exam { get; set; } = ""; // Excel
        public string Offset { get; set; } = ""; // Excel
        public string OffsetWithMark { get; set; } = ""; // Excel
        public string Control{ get; set; } = ""; // Excel
        public string Expert{ get; set; } = ""; // Excel
        public string Actual { get; set; } = ""; // Excel
        public int HoursPerCreditUnit { get; set; } // Excel
        public int ContansHours { get; set; } // Excel


        public List<Semester> Semesters { get; set; } // Excel
        public string[] DisciplineCompetencies { get; set; } // Excel
        public List<CompetencyCode_Name> competencyCode_Names { get; set; } // Excel

        public class CompetencyCode_Name
        {
            public string Code { get; set; } = "";
            public string CodeName { get; set; } = "";
        }

        // Title
        public string Index { get; set; } = ""; // Excel
        public string Author { get; set; } = "";
        public string AuthorInTheInstrumentalCase { get; set; } = "";
        public string Reviewer { get; set; } = "";
        public string DepartmentChair { get; set; } = "";
        public string ProfileNumber { get; set; } = ""; // Excel
        public string ProfileName { get; set; } = ""; // Excel
        public string DisciplineName { get; set; } = ""; // Excel

        public string Qualification { get; set; } = ""; // Excel
        public string DepartmentName { get; set; } = ""; // Excel
        public string EducationForm { get; set; } = ""; // Excel
        public string EducationPeriod { get; set; } = ""; // Excel
        public string StartYear { get; set; } = ""; // Excel


        // 1
        public List<CompetencyPlanningResult> competencyPlanningResults { get; set; } = new List<CompetencyPlanningResult>();

        public class CompetencyPlanningResult
        {
            public CompetencyPlanningResult() { }

            public CompetencyPlanningResult(string Code)
            {
                this.Code = Code;
            }

            public string Code { get; set; } = "";

            public Dictionary<int, string> CompetencyAchivmentIndicators { get; set; } = new Dictionary<int, string>();

            public List<string> ToKnowResult { get; set; } = new List<string>();
            public List<string> ToAbilityResult { get; set; } = new List<string>();
            public List<string> ToOwnResult { get; set; } = new List<string>();

            public CompetencyAchivmentMarkCriteriesClass CompAchivMarkCriteriesToKnow { get; set; } = new CompetencyAchivmentMarkCriteriesClass();
            public CompetencyAchivmentMarkCriteriesClass CompAchivMarkCriteriesToAble { get; set; } = new CompetencyAchivmentMarkCriteriesClass();
            public CompetencyAchivmentMarkCriteriesClass CompAchivMarkCriteriesToOwn { get; set; } = new CompetencyAchivmentMarkCriteriesClass();

            public class CompetencyAchivmentMarkCriteriesClass
            {
                public string Excelent { get; set; } = "";
                public string Good { get; set; } = "";
                public string Satisfactory { get; set; } = "";
                public string Unsatisfactory { get; set; } = "";
            }
        }


        // 4. Thematic plan of discipline
        public List<DisciplineThematicTheme> DisciplineThematicPlan { get; set; } = new List<DisciplineThematicTheme>();

        public int NeedTotalLectureHours { get; set; } // Excel + Processing
        public int NeedTotalPracticeHours { get; set; } // Excel + Processing
        public int NeedTotalLaboratoryWorkHours { get; set; } // Excel + Processing
        public int NeedTotalIndependentHours { get; set; } // Excel + Processing

        public class DisciplineThematicTheme
        {
            public string ThemeName { get; set; } = "";
            public int Semester { get; set; }  // Excel
            public int SemesterModule { get; set; } // Excel

            public int LectureHours => ThemeContents.Where(x => x.ThemeType == ThemeContent.ThemeTypeEnum.Lecture).Sum(x => x.Hour);
            public int PracticeHours => ThemeContents.Where(x => x.ThemeType == ThemeContent.ThemeTypeEnum.PracticeWork).Sum(x => x.Hour);
            public int LaboratoryWorkHours => ThemeContents.Where(x => x.ThemeType == ThemeContent.ThemeTypeEnum.LaboratoryWork).Sum(x => x.Hour);
            
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
                    Lecture = 0,
                    PracticeWork = 1,
                    LaboratoryWork = 2,
                }

                public int Hour { get; set; } = 2;

                public ThemeTypeEnum ThemeType { get; set; }
                public string ThemeText { get; set; } = "";
                public string UsingMethod { get; set; } = "";
                public List<string> FormingCompetency { get; set; } = new List<string>();

                public int MaxPoints { get; set; }
            }
        }


        // 5.
        public string MethodBook { get; set; } = "Ситдикова И.П., Ахметзянов Р.Р. Метрология, стандартизация и сертификация: методические указания для выполнения лабораторных работ и организации самостоятельной работы по дисциплине «Метрология, стандартизация и сертификация» для бакалавров направления подготовки 15.03.04 «Автоматизация технологических процессов и производств» очной формы обучения. – Альметьевск: АГНИ, 2021г.";


        // 6.1.
        public List<EvaluationToolModel> Controls { get; set; } = new List<EvaluationToolModel>()
        {
                new EvaluationToolModel("лабораторная работа",
                    "Темы, задания для выполнения лабораторных работ; вопросы к их защите",
                    "Может выполняться в индивидуальном порядке или группой обучающихся. Задания в лабораторных работах должны включать элемент командной работы. Позволяет оценить умения, обучающихся самостоятельно конструировать свои знания в процессе решения практических задач и оценить уровень сформированности аналитических, исследовательских навыков, а также навыков практического мышления. Позволяет оценить способность к профессиональным трудовым действиям"
                ),
                new EvaluationToolModel("Практическая задача",
                    "Комплект задач и заданий",
                    "Средство оценки умения применять полученные теоретические знания в практической ситуации. Задача должна быть направлена на оценивание тех компетенций, которые подлежат освоению в данной дисциплине, должна содержать четкую инструкцию по выполнению или алгоритм действий"
                ),
                new EvaluationToolModel("Тестирование компьютерное",
                    "Фонд тестовых заданий",
                    "Система стандартизированных заданий, позволяющая автоматизировать процедуру измерения уровня знаний и умений, обучающегося по соответствующим компетенциям. Обработка результатов тестирования на компьютере обеспечивается специальными программами. Позволяет проводить самоконтроль (репетиционное тестирование), может выступать в роли тренажера при подготовке к зачету или экзамену"
                ),
            };

        public List<EvaluationToolModel> Attestations = new List<EvaluationToolModel>()
            {
                new EvaluationToolModel("Экзамен",
                    "Перечень вопросов, фонд тестовых заданий",
                    "Итоговая форма определения степени достижения запланированных результатов обучения (оценивания уровня освоения компетенций). Экзамен нацелен на комплексную проверку освоения дисциплины. Экзамен проводится в форме тестирования по всем темам дисциплины"
                ),
            };

        public class EvaluationToolModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Path { get; set; }

            public EvaluationToolModel(string name, string path, string description)
            {
                this.Name = name;
                this.Path = path;
                this.Description = description;
            }
        }

        // Test tasks. 6.3.1.2
        public List<TestTasksClass> testTasks { get; set; } = new List<TestTasksClass>();

        public class TestTasksClass
        {
            public List<string> CompetencyCode { get; set; } = new List<string>();
            public List<TestTaskLine> Tasks { get; set; } = new List<TestTaskLine>();

            public class TestTaskLine
            {
                public string Question { get; set; } = "";

                public List<string> Answers { get; set; } = new List<string>();
            }
        }

        public List<CompetencyTestTasksClass> CompetencyTestTasks { get; set; } = new List<CompetencyTestTasksClass>();

        public class CompetencyTestTasksClass : TestTasksClass
        {
            public int SemesterNumber { get; set; }
            public int moduleNumber { get; set; }
        }

        // 6.3.2.2 - 6.3.2.3
        public EvaluationCriteriesClass EvaluationCriteries { get; set; } = new EvaluationCriteriesClass();

        public class EvaluationCriteriesClass
        {
            public string CriteriaForExcellent { get; set; } = "";
            public string CriteriaForGood { get; set; } = "";
            public string CriteriaForsatisfactory { get; set; } = "";
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

        public List<LiteratureModel> SiteList { get; set; } = new List<LiteratureModel>()
            {
                new LiteratureModel("Учебно-методическая литература для учащихся и студентов, размещенная на сайте «Studmed.ru»", "http://www.studmed.ru "),
                new LiteratureModel("Единое окно доступа к информационным ресурсам", "http://window.edu.ru/ "),
                new LiteratureModel("Российская государственная библиотека", "http://www.rsl.ru "),
                new LiteratureModel("Электронная библиотека Elibrary", "http://elibrary.ru "),
                new LiteratureModel("Электронно-библиотечная система IPRbooks", "http://elibrary.ru "),
                new LiteratureModel("Электронная библиотека АГНИ", "http://elibrary.agni-rt.ru "),
                new LiteratureModel("Энциклопедия России «Библиотекарь»", "http://bibliotekar.ru "),
            };

        public class LiteratureModel
        {
            public string Name { get; set; } = "";
            public string Link { get; set; } = "";

            public LiteratureModel() { }

            public LiteratureModel(string name, string link)
            {
                Name = name;
                Link = link;
            }
        }


        // 6.
        public EducationLiteratureModelComplex EducationLiteraturesComplex { get; set; } = new EducationLiteratureModelComplex();


        public class EducationLiteratureModelComplex
        {
            public List<EducationLiteratureModel> MainLiteratures { get; set; } = new List<EducationLiteratureModel>();
            public List<EducationLiteratureModel> AdditionalLiteratures { get; set; } = new List<EducationLiteratureModel>();
            public List<EducationLiteratureModel> EducationMethodicalLiteratures { get; set; } = new List<EducationLiteratureModel>();

            public class EducationLiteratureModel : LiteratureModel
            {
                public int Coefficient { get; set; }
                public int? Count { get; set; }
            }
        }


        // 10.
        public List<SoftwareInfo> SoftwareInfos { get; set; } = new List<SoftwareInfo>();

        public class SoftwareInfo
        {
            public string Name { get; set; } = "";
            public string License { get; set; } = "";
            public string Agreement { get; set; } = "";
        }

        public bool CheckModelForCorrect()
        {
            return true;
        }
    }
}
