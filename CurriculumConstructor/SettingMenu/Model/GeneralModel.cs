using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            //DepartmentChair = titleData.DepartmentName;
            EducationForm = titleData.EducationForm;
            EducationPeriod = titleData.EducationPeriod;
            StartYear = titleData.StartYear;

            competencyCode_Names = titleData.CompetencyCode_Names.Where(x => disciplineRow.Competencies.Contains(x.Code)).ToList();


            Index = disciplineRow.Index;
            DisciplineName = disciplineRow.DisciplineName;
            DisciplineCompetencies = disciplineRow.Competencies;

            competencyPlanningResults.AddRange(DisciplineCompetencies.Select(x => new CompetencyPlanningResult(x)));


            ExamSemesterNumbers = disciplineRow.Exam.ToCharArray().Select(x => x - '0').ToArray();
            OffsetSemesterNumbers = disciplineRow.Offset.ToCharArray().Select(x => x - '0').ToArray(); 
            OffsetWithMarkSemesterNumbers = disciplineRow.OffsetWithMark.ToCharArray().Select(x => x - '0').ToArray();

            CourseworkSemesters = disciplineRow.CourseworkSemesters;

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


            foreach(var semester in Semesters)
            {
                DisciplineThematicPlan.Add((semester.SemesterNumber, 1), new SemesterModuleData());
                DisciplineThematicPlan.Add((semester.SemesterNumber, 2), new SemesterModuleData());

                TestTasksByDiscipModule.Add((semester.SemesterNumber, 1), new TestTasksClass());
                TestTasksByDiscipModule.Add((semester.SemesterNumber, 2), new TestTasksClass());

                SemesterQuestionCodes.Add(semester.SemesterNumber, new List<QuestionCodesClass>());
                ExamTestTasksVariantTemplate.Add(semester.SemesterNumber, new Dictionary<List<string>, List<TestTasksClass.TestTaskLine>>());
            }
        }

        // Block and sub block of discipline
        public string ParentBlock { get; set; } = ""; // Excel
        public string ParentBlock_1 { get; set; } = "";

        public string ParentSubBlock { get; set; } = ""; // Excel
        public string ParentSubBlock_1 { get; set; } = "";


        // Undefined but need
        public int[] ExamSemesterNumbers { get; set; } // Excel
        public bool IsExam => ExamSemesterNumbers.Count() > 0; // Excel

        public int[] OffsetSemesterNumbers { get; set; } // Excel
        public bool IsOffset => OffsetSemesterNumbers.Count() > 0; // Excel 

        public int[] OffsetWithMarkSemesterNumbers { get; set; } // Excel
        public bool IsOffsetWithMark => OffsetWithMarkSemesterNumbers.Count() > 0; // Excel

        public int[] CourseworkSemesters { get; set; } // Excel

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
        public string DepartmentChair { get; set; } = ""; // Excel
        public string ProfileNumber { get; set; } = ""; // Excel
        public string ProfileName { get; set; } = ""; // Excel
        public string DisciplineName { get; set; } = ""; // Excel

        public string Qualification { get; set; } = ""; // Excel
        public string EducationForm { get; set; } = ""; // Excel
        public string EducationPeriod { get; set; } = ""; // Excel
        public string StartYear { get; set; } = ""; // Excel


        // 1, 6.2
        public List<CompetencyPlanningResult> competencyPlanningResults { get; } = new List<CompetencyPlanningResult>();

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
        public Dictionary<(int semesterNumber, int semesterModuleNumber), SemesterModuleData> DisciplineThematicPlan { get; } = new Dictionary<(int semesterNumber, int semesterModuleNumber), SemesterModuleData>();

        public int NeedTotalLectureHours { get; set; } // Excel + Processing
        public int NeedTotalPracticeHours { get; set; } // Excel + Processing
        public int NeedTotalLaboratoryWorkHours { get; set; } // Excel + Processing
        public int NeedTotalIndependentHours { get; set; } // Excel + Processing

        public class SemesterModuleData
        {
            // 4.
            public List<DisciplineThematicTheme> DisciplineThematicPlan { get; set; } = new List<DisciplineThematicTheme>();


            // 6.4
            public TurpleIntInt CurrentControl_Laboratory_Practice { get; set; } = new TurpleIntInt();
            public TurpleIntInt CurrentControl_Testing { get; set; } = new TurpleIntInt();
            public TurpleIntInt TotalPointsCount { get; set; } = new TurpleIntInt();


            public class TurpleIntInt
            {
                public int Item1 { get; set; }
                public int Item2 { get; set; }
            }

            public class DisciplineThematicTheme
            {
                public string ThemeName { get; set; } = "";
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
        }
        


        // 5.
        public string MethodBook { get; set; } = "Ситдикова И.П., Ахметзянов Р.Р. Метрология, стандартизация и сертификация: методические указания для выполнения лабораторных работ и организации самостоятельной работы по дисциплине «Метрология, стандартизация и сертификация» для бакалавров направления подготовки 15.03.04 «Автоматизация технологических процессов и производств» очной формы обучения. – Альметьевск: АГНИ, 2021г.";


        // Test tasks. 6.3.1.2
        public Dictionary<(int semesterNumber, int semesterModuleNumber), TestTasksClass> TestTasksByDiscipModule { get; } = new Dictionary<(int semesterNumber, int semesterModuleNumber), TestTasksClass>();

        public class TestTasksClass
        {
            // Competency codes - question-answwers tasks
            public Dictionary<List<string>, List<TestTaskLine>> competencyFormingTestTasks { get; set; } = new Dictionary<List<string>, List<TestTaskLine>>();

            // Also using in 6.3.4.3 for exam
            public class TestTaskLine
            {
                public TestTaskLine()
                {
                    Answers = new List<string>()
                    {
                        "", "", "", ""
                    };
                }
                public string Question { get; set; } = "";

                public List<string> Answers { get; set; } // Default count of rows is four
            }
        }


        // 6.3.2.3, 6.3.3.3
        public EvaluationCriteriesClass EvaluationCriteries { get; set; } = new EvaluationCriteriesClass();
        
        public class EvaluationCriteriesClass
        {
            public LaboratoryEvaluationClass laboratory { get; set; } = new LaboratoryEvaluationClass();
            public PracticeEvaluationClass practice { get; set; } = new PracticeEvaluationClass();

            public class LaboratoryEvaluationClass
            {
                public string LaboratoryTaskWithNumber { get; set; } = "";
                public string TaskTextExampleForDefenceLabWork { get; set; } = "";
                public List<QuestionCodeClass> QuestionsCodeExampleForDefenceLabWork { get; set; } = new List<QuestionCodeClass>();

                public class QuestionCodeClass
                {
                    public string Question { get; set; } = "";
                    public string CompetencyCode { get; set; } = "";
                }
            }

            public class PracticeEvaluationClass
            {
                public string CompetencyCode { get; set; } = "";
                public string PracticeTask { get; set; } = "";
                public string PracticeTaskDescription { get; set; } = "";
            }
        }


        // 6.3.4(5)(6).3
        public Dictionary<int, List<QuestionCodesClass>> SemesterQuestionCodes { get; set; } = new Dictionary<int, List<QuestionCodesClass>>();

        public class QuestionCodesClass
        {
            public string Question { get; set; } = "";
            public List<string> Competencies { get; set; } = new List<string>();
        }

        // Only for exam format
        // Competency codes - question-answers tasks
        public Dictionary<int, Dictionary<List<string>, List<TestTasksClass.TestTaskLine>>> ExamTestTasksVariantTemplate { get; set; } = new Dictionary<int, Dictionary<List<string>, List<TestTasksClass.TestTaskLine>>>();


        // 7.
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


        // 8.
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


        // 10.
        public List<SoftwareInfo> SoftwareInfos { get; set; } = new List<SoftwareInfo>()
        {
                new SoftwareInfo("Microsoft Office Professional Plus 2016 Rus Academic OLP (Word, Excel, PowerPoint, Access)", "№67892163 от 26.12.2016г.", "№0297/136 от 23.12.2016г."),
                new SoftwareInfo("Microsoft Office Standard 2016 Rus Academic OLP (Word, Excel, PowerPoint)", "№67892163 от 26.12.2016г.", "№0297/136 от 23.12.2016г."),
                new SoftwareInfo("Microsoft Windows Professional 10 Rus Upgrade Academic OLP", "№67892163 от 26.12.2016г.", "№0297/136 от 23.12.2016г."),
                new SoftwareInfo("ABBYY Fine Reader 12 Professional", "№197059 от 26.12.2016г.", "№0297/136 от 23.12.2016г."),
                new SoftwareInfo("Kaspersky Endpoint Security для бизнеса – Стандартный Russian Edition", "№ 24С4-221222-121357-913-1225", "№691447/581-2022 от 16.12.2022г."), //Доработать текущий год поставить
                new SoftwareInfo("Электронно-библиотечная система IPRbooks", "", "Лицензионный договор №409-2022 от 03.11.2022г."),
                new SoftwareInfo("Образовательная платформа для подготовки кадров в цифровой экономике DATALIB.RU", "", "Лицензионный договор №428-2022/22d/B от 09.11.2022г."),
                new SoftwareInfo("ПО «Автоматизированная тестирующая система", "Свидетельство государственной регистрации программ для ЭВМ №2014614238 от 01.04.2014г.", ""),
        };

        public class SoftwareInfo
        {
            public SoftwareInfo() { }

            public SoftwareInfo(string name, string license, string agreement, bool agreementIsExist = true)
            {
                Name = name;
                License = license;
                Agreement = agreement;
                AgreementIsExist = agreementIsExist;
            }

            public string Name { get; set; } = "";
            public string Agreement { get; set; } = "";
            public string License { get; set; } = "";
            public bool AgreementIsExist { get; set; } = true;
        }


        // 11
        public List<PlaceTheirEquipmentsClass> PlaceTheirEquipments = new List<PlaceTheirEquipmentsClass>();

        public class PlaceTheirEquipmentsClass
        {
            public string PlaceName { get; set; } = "";
            public List<string> EquipmentsName { get; set; } = new List<string>();
        }


        // Methods
        public bool CheckModelForCorrect()
        {
            return true;
        }
    }
}
