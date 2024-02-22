using CurriculumConstructor.SettingMenu.Model;
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

namespace CurriculumConstructor.SettingMenu.Pages
{
    /// <summary>
    /// Логика взаимодействия для GeneralInformationPage.xaml
    /// </summary>
    public partial class GeneralInformationPage : Page
    {
        private GeneralModel generalModel;

        public GeneralInformationPage(ref GeneralModel generalModel)
        {
            InitializeComponent();

            this.generalModel = generalModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            textBlockForGeneralText.Text =
                "Направление: " + generalModel.ProfileNumber + " - " + generalModel.ProfileName + "\n"
                + "Дисциплина: " + generalModel.DisciplineName + "\n"
                + "Семестры изучения: " + string.Join(", ", generalModel.Semesters.Select(x => x.SemesterNumber).Select(x =>
                        (generalModel.OffsetSemesterNumbers.Contains(x) ? "зачёт в "
                        : generalModel.OffsetWithMarkSemesterNumbers.Contains(x) ? "зачёт с оценкой в "
                        : generalModel.ExamSemesterNumbers.Contains(x) ? "экзамен в "
                        : "неизвестное в ")
                        + x.ToString() + " семестре"
                        )
                ) + "\n"
                + "Курсовые: " + (generalModel.CourseworkSemesters.Count() <= 0 ? "нет" : string.Join(", ", generalModel.CourseworkSemesters.Select(x => x.ToString() + " семестр"))) + "\n"
                + "Закрепленная кафедра: " + generalModel.DepartmentName + "\n"
                + "Компетенции: " + string.Join(", ", generalModel.DisciplineCompetencies);
        }
    }
}
