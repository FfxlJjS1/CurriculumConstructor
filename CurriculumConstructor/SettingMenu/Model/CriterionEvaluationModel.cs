using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CurriculumConstructor.SettingMenu.Model
{
    /// <summary>
    /// Критерий оценивания
    /// </summary>
    public class CriterionEvaluationModel : INotifyPropertyChanged
    {
        /// <summary>
        /// критерий оценивания для лаб
        /// </summary>
        public static CriterionEvaluationModel CriterionEvaluationLab { get; set; } = new CriterionEvaluationModel();
        /// <summary>
        /// критерий оценивания для практики
        /// </summary>
        public static CriterionEvaluationModel CriterionEvaluationPractic { get; set; } = new CriterionEvaluationModel();

        /// <summary>
        /// Вопросы
        /// </summary>
        public List<string> Questions = new List<string>(); 

        private string five;
        private string four;
        private string three;
        private string two;
        private string title;
        private string task;

        public string Five
        {
            get
            {
                return five;
            }
            set
            {
                five = value;
                OnPropertyChanged(nameof(Five));
            }
        }

        public string Four
        {
            get { return four;}
            set
            {
                four = value;
                OnPropertyChanged(nameof(Four));
            }
        }
        public string Two
        {
            get => two;
            set
            {
                two = value;
                OnPropertyChanged(nameof(Two));
            }
        }
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        public string Task
        {
            get
            {
                return task;
            }
            set
            {
                task = value;
                OnPropertyChanged(nameof(Task));
            }
        }
        public string Three
        {
            get
            {
                return three;
            }
            set
            {
                three = value;
                OnPropertyChanged(nameof(Three));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
