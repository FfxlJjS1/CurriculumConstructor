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
    /// Дисциплинарный модуль 4.1
    /// </summary>
    public class DisciplinaryModule : INotifyPropertyChanged
    {
        private string theme;
        private string method;

        /// <summary>
        /// Тема
        /// </summary>
        public string Theme
        {
            get
            {
                return theme;
            }
            set
            {
                theme = value;
                OnPropertyChanged(nameof(Theme));
            }
        }
        /// <summary>
        /// Используемый метод
        /// </summary>
        public string Method
        {
            get { return method; }
            set
            {
                method = value; OnPropertyChanged(nameof(Method));
            }
        }

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
