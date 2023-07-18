using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CurriculumConstructor.SettingMenu.Model
{
    public class TitleModel : INotifyPropertyChanged
    {
        private string author;
        private string reviewer;
        private string head;

        public string Author
        {
            get { return author; }
            set
            {
                author = value;
                OnPropertyChanged(nameof(Author));
            }
        }
        public string Reviewer
        {
            get => reviewer;
            set
            {
                reviewer = value; 
                OnPropertyChanged("Reviewer");
            }
        }
       public string Head
        {
            get
            {
                return head;
            }
            set
            {
                head = value;
                OnPropertyChanged("Head");
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
