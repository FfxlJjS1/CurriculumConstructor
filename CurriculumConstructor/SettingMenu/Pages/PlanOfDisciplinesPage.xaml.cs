using CurriculumConstructor.SettingMenu.Model;
using CurriculumConstructor.SettingMenu.Windows;
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
    /// Логика взаимодействия для PlanOfDisciplinesPage.xaml
    /// </summary>
    public partial class PlanOfDisciplinesPage : Page
    {
        private GeneralModel generalModel;
        private List<GeneralModel.DisciplineThematicTheme> _model;

        private GeneralModel.DisciplineThematicTheme _themeDisciplines;
        private bool IsEdit;

        public PlanOfDisciplinesPage(ref GeneralModel generalModel)
        {
            InitializeComponent();


            this.generalModel = generalModel;
            this._model = generalModel.DisciplineThematicPlan;
        }

        
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (IsEdit == false)
            {
                _model.Add(_themeDisciplines);
            }
            
            Reload();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void DelClick(object sender, RoutedEventArgs e)
        {
            if (ThemeDisciplinesListBox.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать тему!");
                return;
            }

            _model.Remove(_themeDisciplines);
            Reload();
        }

        private void ThemeDisciplinesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeDisciplinesListBox.SelectedItem == null)
            {
                return;
            }

            IsEdit = true;

            GeneralModel.DisciplineThematicTheme theme = ThemeDisciplinesListBox.SelectedItem as GeneralModel.DisciplineThematicTheme;

            _themeDisciplines = theme;
            DataContext = _themeDisciplines;
        }

        private void Reload()
        {
            ThemeDisciplinesListBox.ItemsSource = _model;

            _themeDisciplines = new GeneralModel.DisciplineThematicTheme();

            IsEdit = false;
            
            ThemeDisciplinesListBox.SelectedItem = null;
            ThemeDisciplinesListBox.Items.Refresh();

            DataContext = _themeDisciplines;

            txtboxLecture.Text = "Лекций: " + _model.Sum(x => x.LectureHours).ToString() + "/" + generalModel.NeedTotalLectureHours.ToString();
            txtboxPractice.Text = "Практический занятий: " + _model.Sum(x => x.PracticeHours).ToString() + "/" + generalModel.NeedTotalPracticeHours.ToString();
            txtboxLaboratory.Text = "Лабораторных занятий: " + _model.Sum(x => x.LaboratoryWorkHours).ToString() + "/" + generalModel.NeedTotalLaboratoryWorkHours.ToString();
            txtboxIndependent.Text = "СРС: " + _model.Sum(x => x.IndependentHours).ToString() + "/" + generalModel.NeedTotalIndependentHours.ToString();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void ContentClick(object sender, RoutedEventArgs e)
        {
            if (ThemeDisciplinesListBox.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать тему!");
                return;
            }

            DisciplineContentWindow disciplineContentWindow = new DisciplineContentWindow(ref _themeDisciplines, generalModel.competencyCode_Names);

            disciplineContentWindow.ShowDialog();
        }
    }
}
