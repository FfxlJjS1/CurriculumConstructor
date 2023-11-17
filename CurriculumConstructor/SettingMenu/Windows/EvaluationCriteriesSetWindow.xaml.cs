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
    /// Логика взаимодействия для EvaluationCriteriesSetWindow.xaml
    /// </summary>
    public partial class EvaluationCriteriesSetWindow : Window
    {
        private GeneralModel.CompetencyPlanningResult.CompetencyAchivmentMarkCriteriesClass model;
        private int btnIdentification;

        public EvaluationCriteriesSetWindow(ref GeneralModel.CompetencyPlanningResult parent_model, int btnIdentification)
        {
            InitializeComponent();

            this.btnIdentification = btnIdentification;

            this.model =
                this.btnIdentification == 1
                ? parent_model.CompAchivMarkCriteriesToKnow
                : this.btnIdentification == 2
                ? parent_model.CompAchivMarkCriteriesToAble
                : this.btnIdentification == 3
                ? parent_model.CompAchivMarkCriteriesToOwn : new GeneralModel.CompetencyPlanningResult.CompetencyAchivmentMarkCriteriesClass();

            this.Title += this.btnIdentification == 1 ? " (знать)"
                : this.btnIdentification == 2 ? " (уметь)"
                : this.btnIdentification == 3 ? " (владеть)" : "";

            txtBoxExcelent.Text = model.Excelent;
            txtBoxGood.Text = model.Good;
            txtBoxSatisfactory.Text = model.Satisfactory;
            txtBoxUnsatisfactory.Text = model.Unsatisfactory;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            model.Excelent = txtBoxExcelent.Text ;
            model.Good = txtBoxGood.Text ;
            model.Satisfactory = txtBoxSatisfactory.Text ;
            model.Unsatisfactory = txtBoxUnsatisfactory.Text;

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
