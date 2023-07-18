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
    /// Логика взаимодействия для IntermediateCertificationPage.xaml
    /// </summary>
    public partial class IntermediateCertificationPage : Page
    {
        public IntermediateCertificationPage()
        {
            InitializeComponent();
            _model = IntermediateCertificationModel.IntermediateCertification;
            if (_model == null) _model = new IntermediateCertificationModel();
            DataContext = _model;
        }

        IntermediateCertificationModel _model;

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"{_model.Title}") ;
        }
    }
}
