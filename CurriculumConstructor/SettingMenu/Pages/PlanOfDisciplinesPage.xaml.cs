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
        private GeneralModel.SemesterModuleData _model;

        private GeneralModel.SemesterModuleData.DisciplineThematicTheme _themeDisciplines;
        private bool IsEdit;

        public PlanOfDisciplinesPage(ref GeneralModel generalModel)
        {
            InitializeComponent();


            this.generalModel = generalModel;

            comboBoxSemesterModuleNumber.Items.Clear();
            comboBoxSemesterModuleNumber.ItemsSource = new int[] { 1, 2 };
            comboBoxSemesterModuleNumber.SelectedIndex = 0;
        }

        
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (IsEdit == false)
            {
                _model.DisciplineThematicPlan.Add(_themeDisciplines);
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

            _model.DisciplineThematicPlan.Remove(_themeDisciplines);
            Reload();
        }

        private void ThemeDisciplinesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeDisciplinesListBox.SelectedItem == null)
            {
                return;
            }

            IsEdit = true;

            GeneralModel.SemesterModuleData.DisciplineThematicTheme theme = ThemeDisciplinesListBox.SelectedItem as GeneralModel.SemesterModuleData.DisciplineThematicTheme;

            _themeDisciplines = theme;
            DataContext = _themeDisciplines;
        }

        private void Reload()
        {
            ThemeDisciplinesListBox.ItemsSource = _model.DisciplineThematicPlan;

            _themeDisciplines = new GeneralModel.SemesterModuleData.DisciplineThematicTheme();

            IsEdit = false;
            
            ThemeDisciplinesListBox.SelectedItem = null;
            ThemeDisciplinesListBox.Items.Refresh();

            DataContext = _themeDisciplines;

            ReloadHoursValues();
        }

        private void ReloadHoursValues()
        {
            var semesterDicsiplinePlan = generalModel.DisciplineThematicPlan[new GeneralModel.SemesterModuleNumbers((int)comboBoxSemesterNumber.SelectedItem, (int)comboBoxSemesterModuleNumber.SelectedItem)].DisciplineThematicPlan;
            var semesterValues = generalModel.Semesters.First(x => x.SemesterNumber == (int)comboBoxSemesterNumber.SelectedItem);

            txtboxLecture.Text = "Лекций: " + semesterDicsiplinePlan.Sum(x => x.LectureHours).ToString() + "/" + semesterValues.Lectures.ToString();
            txtboxPractice.Text = "Практический занятий: " + semesterDicsiplinePlan.Sum(x => x.PracticeHours).ToString() + "/" + semesterValues.PracticeWorks.ToString();
            txtboxLaboratory.Text = "Лабораторных занятий: " + semesterDicsiplinePlan.Sum(x => x.LaboratoryWorkHours).ToString() + "/" + semesterValues.LaboratoryWorks.ToString();
            txtboxIndependent.Text = "СРС: " + semesterDicsiplinePlan.Sum(x => x.IndependentHours).ToString() + "/" + semesterValues.IndependentWork.ToString();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxSemesterNumber.ItemsSource = this.generalModel.Semesters.Select(x => x.SemesterNumber).ToList();
            comboBoxSemesterNumber.SelectedIndex = 0;

            Reload();
        }

        private void ContentClick(object sender, RoutedEventArgs e)
        {
            if (ThemeDisciplinesListBox.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать тему!");
                return;
            }

            var semesterValues = generalModel.Semesters[(int)comboBoxSemesterNumber.SelectedItem];

            DisciplineContentWindow disciplineContentWindow = new DisciplineContentWindow(ref _themeDisciplines, generalModel.competencyCode_Names, (semesterValues.Lectures > 0, semesterValues.PracticeWorks > 0, semesterValues.LaboratoryWorks > 0));

            disciplineContentWindow.ShowDialog();

            ReloadHoursValues();
        }

        private bool isOnTxtBoxChanged = true;

        private void comboBoxSemesterOrModuleNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(comboBoxSemesterNumber.SelectedItem == null || comboBoxSemesterModuleNumber.SelectedItem == null)
            {
                return;
            }

            _model = generalModel.DisciplineThematicPlan[new GeneralModel.SemesterModuleNumbers((int)comboBoxSemesterNumber.SelectedItem, (int)comboBoxSemesterModuleNumber.SelectedItem)];

            isOnTxtBoxChanged = false;

            txtBoxMinLabPrac.DataContext = _model;
            txtBoxMaxLabPrac.DataContext = _model;
            txtBoxMinTesting.DataContext = _model;
            txtBoxMaxTesting.DataContext = _model;

            txtBoxMinTotal.DataContext = _model;
            txtBoxMaxTotal.DataContext = _model;

            isOnTxtBoxChanged = true;

            Reload();
        }

        private void txtBoxMinMaxLabPrac_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isOnTxtBoxChanged)
            {
                return;
            }

            int forTryParse = 0;

            if(int.TryParse(txtBoxMinLabPrac.Text, out forTryParse))
            {
                _model.CurrentControl_Laboratory_Practice.Item1 = forTryParse;
            }
            if (int.TryParse(txtBoxMaxLabPrac.Text, out forTryParse))
            {
                _model.CurrentControl_Laboratory_Practice.Item2 = forTryParse;
            }
            if (int.TryParse(txtBoxMinTesting.Text, out forTryParse))
            {
                _model.CurrentControl_Testing.Item1 = forTryParse;
            }
            if (int.TryParse(txtBoxMaxTesting.Text, out forTryParse))
            {
                _model.CurrentControl_Testing.Item2 = forTryParse;
            }

            _model.TotalPointsCount.Item1 = _model.CurrentControl_Laboratory_Practice.Item1
                + _model.CurrentControl_Testing.Item1;

            _model.TotalPointsCount.Item2 = _model.CurrentControl_Laboratory_Practice.Item2
                + _model.CurrentControl_Testing.Item2;

            txtBoxMinTotal.DataContext = _model;
            txtBoxMaxTotal.DataContext = _model;
        }
    }
}
