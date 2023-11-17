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
using static CurriculumConstructor.SettingMenu.Model.GeneralModel.TestTasksClass;

namespace CurriculumConstructor.SettingMenu.Pages
{
    /// <summary>
    /// Логика взаимодействия для TestTasksForDetermineTheLevelOfCompetenciesPage.xaml
    /// </summary>
    public partial class TestTasksForDetermineTheLevelOfCompetenciesPage : Page
    {
        private GeneralModel generalModel;
        private GeneralModel.TestTasksClass _model;
        private List<GeneralModel.TestTasksClass.TestTaskLine> _sub_model;
        private List<CompetenciesComboBoxItem> _competenciesComboBoxItems = new List<CompetenciesComboBoxItem>();

        private TestTaskLine _testTask;
        private bool IsEdit;

        public TestTasksForDetermineTheLevelOfCompetenciesPage(ref GeneralModel generalModel)
        {
            InitializeComponent();

            this.generalModel = generalModel;

            comboBoxSemesterModuleNumber.Items.Clear();
            comboBoxSemesterModuleNumber.ItemsSource = new int[] { 1, 2 };
            comboBoxSemesterModuleNumber.SelectedIndex = 0;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (IsEdit == false && ComboBoxCompetenciesCode.SelectedItem != null)
            {
                _sub_model.Add(_testTask);
            }

            Reload();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void DelClick(object sender, RoutedEventArgs e)
        {
            if (listBoxCompetenciesTestTasks.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать тему!");
                return;
            }

            if(ComboBoxCompetenciesCode.SelectedItem == null)
            {
                return;
            }

            _sub_model.Remove(_testTask);
            Reload();
        }

        private void Reload()
        {
            if(ComboBoxCompetenciesCode.SelectedItem == null)
            {
                listBoxCompetenciesTestTasks.ItemsSource = null;
                listBoxCompetenciesTestTasks.Items.Clear();
            }
            else
            {
                listBoxCompetenciesTestTasks.ItemsSource = _sub_model;
            }

            _testTask = new GeneralModel.TestTasksClass.TestTaskLine();

            IsEdit = false;

            listBoxCompetenciesTestTasks.SelectedItem = null;
            listBoxCompetenciesTestTasks.Items.Refresh();

            DataContext = _testTask;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxSemesterNumber.ItemsSource = this.generalModel.Semesters.Select(x => x.SemesterNumber).ToList();
            comboBoxSemesterNumber.SelectedIndex = 0;

            Reload();
        }

        private void comboBoxSemesterOrModuleNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxSemesterNumber.SelectedItem == null || comboBoxSemesterModuleNumber.SelectedItem == null)
            {
                return;
            }

            _model = generalModel.TestTasksByDiscipModule[((int)comboBoxSemesterNumber.SelectedItem, (int)comboBoxSemesterModuleNumber.SelectedItem)];

            _competenciesComboBoxItems = _model.competencyFormingTestTasks.Select(x => new CompetenciesComboBoxItem() { CompetenciesCode = x.Key }).ToList();

            ComboBoxCompetenciesCode.ItemsSource = _competenciesComboBoxItems;

            Reload();
        }

        public class CompetenciesComboBoxItem
        {
            public string FullCompetenciesString => string.Join(", ", CompetenciesCode);
            public List<string> CompetenciesCode = new List<string>();
        }

        private void AddCompetenciesList_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedCompetenciesCodeAsItem = new List<string>();

            CompetenciesCodeItemSelectWindow competenciesCodeItemSelect =
                new CompetenciesCodeItemSelectWindow(generalModel.DisciplineCompetencies, ref selectedCompetenciesCodeAsItem);

            competenciesCodeItemSelect.ShowDialog();

            if (selectedCompetenciesCodeAsItem.Count <= 0
                || generalModel.examTestTasksVariantTemplate.Keys.ToList().Contains(selectedCompetenciesCodeAsItem))
            {
                return;
            }

            _model.competencyFormingTestTasks.Add(selectedCompetenciesCodeAsItem, new List<GeneralModel.TestTasksClass.TestTaskLine>());

            _competenciesComboBoxItems.Add(new CompetenciesComboBoxItem() { CompetenciesCode = selectedCompetenciesCodeAsItem });

            ComboBoxCompetenciesCode.SelectedItem = selectedCompetenciesCodeAsItem;

            ComboBoxCompetenciesCode.Items.Refresh();

            Reload();
        }

        private void ComboBoxCompetenciesCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxCompetenciesCode.SelectedItem == null)
            {
                return;
            }

            var competenciesComboboxItem = ComboBoxCompetenciesCode.SelectedItem as CompetenciesComboBoxItem;

            _sub_model = _model.competencyFormingTestTasks[competenciesComboboxItem.CompetenciesCode];

            Reload();
        }

        private void listBoxCompetenciesTestTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxCompetenciesTestTasks.SelectedItem == null)
            {
                return;
            }

            IsEdit = true;

            GeneralModel.TestTasksClass.TestTaskLine testTaskLine = listBoxCompetenciesTestTasks.SelectedItem as GeneralModel.TestTasksClass.TestTaskLine;

            _testTask = testTaskLine;
            DataContext = _testTask;
        }
    }
}
