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
    /// Логика взаимодействия для ListOfResultPage.xaml
    /// </summary>
    public partial class ListOfResultPage : Page
    {
        public ListOfResultPage(ListOfResultsModel model)
        {
            InitializeComponent();
            if (model == null)
            {
                _model = new ListOfResultsModel();
            }
            else
            {
                _model = model;
            }
            DataContext = _model;
        }
        private ListOfResultsModel _model;
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_model.Col1);
        }
    }
}
