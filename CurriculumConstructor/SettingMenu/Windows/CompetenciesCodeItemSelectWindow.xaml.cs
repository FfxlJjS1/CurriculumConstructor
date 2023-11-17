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
    /// Логика взаимодействия для CompetenciesCodeItemSelectWindow.xaml
    /// </summary>
    public partial class CompetenciesCodeItemSelectWindow : Window
    {
        private List<string> selectedCompetenciesCodeItem;
        public string[] availableCompetencies;

        public CompetenciesCodeItemSelectWindow(string[] availableCompetencies, ref List<string> selectedCompetenciesCodeItem)
        {
            InitializeComponent();

            this.selectedCompetenciesCodeItem = selectedCompetenciesCodeItem;
            this.availableCompetencies = availableCompetencies;

            listBoxAvailableCompetencyForSelect.ItemsSource = availableCompetencies;
            listboxSelectedCompetency.ItemsSource = this.selectedCompetenciesCodeItem;
        }


        private void SelectCompetency_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxAvailableCompetencyForSelect.SelectedItem == null)
            {
                MessageBox.Show("Выберите компетенцию для добавления");

                return;
            }

            string selectedCompetency = listBoxAvailableCompetencyForSelect.SelectedItem as string;

            selectedCompetenciesCodeItem.Add(selectedCompetency);

            listBoxAvailableCompetencyForSelect.ItemsSource = availableCompetencies.Where(x => !selectedCompetenciesCodeItem.Contains(x)).ToList();
            listBoxAvailableCompetencyForSelect.Items.Refresh();
            listboxSelectedCompetency.Items.Refresh();
        }

        private void UnselectCompetency_Click(object sender, RoutedEventArgs e)
        {
            if (listboxSelectedCompetency.SelectedItem == null)
            {
                MessageBox.Show("Выберите компетенцию для исключения");

                return;
            }

            string forUnselectCompetency = listboxSelectedCompetency.SelectedItem as string;

            selectedCompetenciesCodeItem.Remove(forUnselectCompetency);

            listBoxAvailableCompetencyForSelect.ItemsSource = availableCompetencies.Where(x => !selectedCompetenciesCodeItem.Contains(x)).ToList();
            listBoxAvailableCompetencyForSelect.Items.Refresh();
            listboxSelectedCompetency.Items.Refresh();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            selectedCompetenciesCodeItem.Clear();

            Close();
        }
    }
}
