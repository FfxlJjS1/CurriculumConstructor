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
using System.Windows.Shapes;

namespace CurriculumConstructor.SettingMenu.Windows
{
    /// <summary>
    /// Логика взаимодействия для DisciplineContentWindow.xaml
    /// </summary>
    public partial class DisciplineContentWindow : Window
    {
        private List<GeneralModel.CompetencyCode_Name> competenciesCode_Name;

        public DisciplineContentWindow(ref GeneralModel.SemesterModuleData.DisciplineThematicTheme discThematicTheme, List<GeneralModel.CompetencyCode_Name> competencies)
        {
            InitializeComponent();
            this.Title = $"{discThematicTheme.ThemeName} ({discThematicTheme.AllHour} ч.)";

            this.competenciesCode_Name = competencies;
            _themeDisciplines = discThematicTheme;

            comboBoxThemeType.ItemsSource = new List<ComboBoxThemeType>() {
                new ComboBoxThemeType(GeneralModel.SemesterModuleData.DisciplineThematicTheme.ThemeContent.ThemeTypeEnum.Lecture, "Лекция"),
                new ComboBoxThemeType(GeneralModel.SemesterModuleData.DisciplineThematicTheme.ThemeContent.ThemeTypeEnum.PracticeWork, "Практическое занятие"),
                new ComboBoxThemeType(GeneralModel.SemesterModuleData.DisciplineThematicTheme.ThemeContent.ThemeTypeEnum.LaboratoryWork, "Лабораторное занятие")
            };

            Reload();
        }

        private class ComboBoxThemeType
        {
            public GeneralModel.SemesterModuleData.DisciplineThematicTheme.ThemeContent.ThemeTypeEnum ThemeTypeNumber;
            public string ThemeTypeName;

            public ComboBoxThemeType(GeneralModel.SemesterModuleData.DisciplineThematicTheme.ThemeContent.ThemeTypeEnum themeTypeNumber, string themeTypeName)
            {
                ThemeTypeNumber = themeTypeNumber;
                ThemeTypeName = themeTypeName;
            }
        }

        private GeneralModel.SemesterModuleData.DisciplineThematicTheme _themeDisciplines;
        private GeneralModel.SemesterModuleData.DisciplineThematicTheme.ThemeContent _discThemeContent;
        private bool IsEdit;

        private void Reload()
        {
            ThemeListBox.ItemsSource = _themeDisciplines.ThemeContents;
            ThemeListBox.Items.Refresh();

            ThemeListBox.SelectedItem = null;
            listBoxAvailableCompetencyForSelect.ItemsSource = competenciesCode_Name.Select(x => x.Code).ToList();

            _discThemeContent = new GeneralModel.SemesterModuleData.DisciplineThematicTheme.ThemeContent();
            IsEdit = false;
            
            DataContext = _discThemeContent;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (IsEdit == false)
            {
                _themeDisciplines.ThemeContents.Add(_discThemeContent);
            }

            Reload();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            _themeDisciplines.ThemeContents.Remove(_discThemeContent);

            Reload();
        }
        private void AddClick(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void ThemeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeListBox.SelectedItem == null)
            {
                return;
            }

            _discThemeContent = ThemeListBox.SelectedItem as GeneralModel.SemesterModuleData.DisciplineThematicTheme.ThemeContent;
            DataContext = _discThemeContent;

            listBoxAvailableCompetencyForSelect.ItemsSource = competenciesCode_Name.Select(x => x.Code).Where(x => !_discThemeContent?.FormingCompetency.Contains(x) ?? true);

            IsEdit = true;
        }

        private void SelectCompetency_Click(object sender, RoutedEventArgs e)
        {
            if(listBoxAvailableCompetencyForSelect.SelectedItem == null)
            {
                MessageBox.Show("Выберите компетенцию для добавления");

                return;
            }

            string selectedCompetency = listBoxAvailableCompetencyForSelect.SelectedItem as string;

            _discThemeContent.FormingCompetency.Add(selectedCompetency);

            listBoxAvailableCompetencyForSelect.ItemsSource = competenciesCode_Name.Select(x => x.Code).Where(x => !_discThemeContent?.FormingCompetency.Contains(x) ?? true);
        }

        private void UnselectCompetency_Click(object sender, RoutedEventArgs e)
        {
            if(listBoxSelectedCompetencies.SelectedItem == null)
            {
                MessageBox.Show("Выберите компетенции для исключения");

                return;
            }

            string competencyForUnselect = listBoxSelectedCompetencies.SelectedItem as string;

            _discThemeContent.FormingCompetency.Remove(competencyForUnselect);
        }
    }
}
