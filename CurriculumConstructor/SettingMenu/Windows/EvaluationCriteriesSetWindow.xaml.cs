﻿using CurriculumConstructor.SettingMenu.Model;
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
        private int btnIdentificator;

        public EvaluationCriteriesSetWindow(ref GeneralModel.CompetencyPlanningResult parent_model, int btnIdentification)
        {
            InitializeComponent();

            this.model =
                btnIdentificator == 1
                ? parent_model.CompAchivMarkCriteriesToKnow
                : btnIdentificator == 2
                ? parent_model.CompAchivMarkCriteriesToAble
                : btnIdentificator == 3
                ? parent_model.CompAchivMarkCriteriesToOwn : new GeneralModel.CompetencyPlanningResult.CompetencyAchivmentMarkCriteriesClass();

            this.Title += btnIdentificator == 1 ? " (знать)"
                : btnIdentificator == 2 ? " (уметь)"
                : btnIdentificator == 3 ? " (владеть)" : "";

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
