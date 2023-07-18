using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CurriculumConstructor.SettingMenu.Model
{
    public class ThemeDisciplines : INotifyPropertyChanged
    {
        private string theme;
        private string semestr;
        private int lecture;
        private int practice;
        private int lab;
        private int crc;

        public string Theme
        {
            get => theme;
            set
            {
                theme = value;
                OnPropertyChanged(nameof(Theme));
            }
        }
        public string Semestr
        {
            get => semestr;
            set
            {
                semestr = value; OnPropertyChanged(nameof(Semestr));
            }
        }
        public int Lecture
        {
            get => lecture;
            set
            {
                lecture = value;
                OnPropertyChanged(nameof(Lecture));
            }
        }
        public int Practice
        {
            get => practice;
            set
            {
                practice = value; OnPropertyChanged( nameof(Practice));
            }
        }
        public int Lab
        {
            get => lab;
            set
            {
                lab = value; OnPropertyChanged(nameof(Lab));    
            }
        }
        public int Crc
        {
            get => crc;
            set
            {
                crc = value;
                OnPropertyChanged(nameof(Crc));
            }
        }

        /// <summary>
        /// Подсчитывает сумму всех часов у данной темы
        /// </summary>
        public int AllHour
        {
            get
            {
                return Lecture + Practice + Lab + Crc;
            }
        }

        /// <summary>
        /// Список дисциплинарных модулей у темы
        /// </summary>
        public List<DisciplinaryModule> disciplinaryModules;



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString()
        {
            return Theme;
        }
    }
}
