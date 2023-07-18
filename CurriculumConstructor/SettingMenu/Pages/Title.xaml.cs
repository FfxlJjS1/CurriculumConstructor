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
    /// Логика взаимодействия для Title.xaml
    /// </summary>
    public partial class Title : Page
    {
        public Title()
        {
            InitializeComponent();
            TitleModel titleModel = TitleModel.Title; 
            _titleModel = titleModel;
            if (titleModel == null) _titleModel = new TitleModel();
            DataContext = _titleModel;
        }
        private TitleModel _titleModel;
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_titleModel.ToString());
        }
    }
}
