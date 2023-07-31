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
    /// Тестовые задания
    /// </summary>
    public class TestTasksModel : INotifyPropertyChanged
    {
        /// <summary>
        /// список тестовых заданий
        /// </summary>
        public static List<TestTasksModel> TestTasks { get; set; } = new List<TestTasksModel>();
        /// <summary>
        /// список тестовых заданий для экзамена
        /// </summary>
        public static List<TestTasksModel> TestTasksExam { get; set; } = new List<TestTasksModel>();

        private string question;
        private string ans1;
        private string ans2;
        private string ans3;
        private string ans4;

        public string Question
        {
            get { return question; } set
            {
                question = value;
                OnPropertyChanged(nameof(Question));
            }
        }
        public string Ans1
        {
            get
            {
                return ans1;
            }
            set
            {
                ans1 = value;
                OnPropertyChanged(nameof(Ans1));
            }
        }
        public string Ans2
        {
            get
            {
                return ans2;
            }
            set
            {
                ans2 = value;
                OnPropertyChanged(nameof(Ans2));
            }
        }
        public string Ans3
        {
            get
            {
                return ans3;
            }
            set
            {
                ans3 = value;
                OnPropertyChanged(nameof(Ans3));
            }
        }
        public string Ans4
        {
            get
            {
                return ans4;
            }
            set
            {
                ans4 = value;
                OnPropertyChanged(nameof(Ans4));
            }
        }
        public override string ToString()
        {
            return Question;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
