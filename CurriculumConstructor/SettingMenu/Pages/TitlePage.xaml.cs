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
    /// Логика взаимодействия для TitlePage.xaml
    /// </summary>
    public partial class TitlePage : Page
    {
        private GeneralModel generalModel;

        public TitlePage(ref GeneralModel generalModel)
        {
            InitializeComponent();

            this.generalModel = generalModel;

            txtBoxAuthor.Text = generalModel.Author;
            txtBoxAuthorInTheInstrumentalCase.Text = generalModel.AuthorInTheInstrumentalCase;
            txtBoxReviewer.Text = generalModel.Reviewer;
            txtBoxHead.Text = generalModel.Head;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            generalModel.Author = txtBoxAuthor.Text;
            generalModel.AuthorInTheInstrumentalCase = txtBoxAuthorInTheInstrumentalCase.Text;
            generalModel.Reviewer = txtBoxReviewer.Text;
            generalModel.Head = txtBoxHead.Text;
        }
    }
}
