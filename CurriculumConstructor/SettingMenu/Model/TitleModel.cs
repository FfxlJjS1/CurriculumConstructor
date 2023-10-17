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
        public static TitleModel Title = new TitleModel();

        private string index;
        private string author;
        private string reviewer;
        private string head;
        private string department_chair;
        private string profileNumber;
        private string profile;
        private string discipline;

        private string qualification;
        private string department_name;
        private string education_form;
        private string education_period;
        private string start_year;

        public string Index
        {
            get { return index; }
            set
            {
                index = value;
                OnPropertyChanged("Index");
            }
        }
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
        public string DepartmentName
        {
            get
            {
                return department_name;
            }
            set
            {
                department_name= value;
                OnPropertyChanged("DepartmentName");
            }
        }
        public string Department_chair
        {
            get
            {
                return department_chair;
            }
            set
            {
                department_chair = value;
                OnPropertyChanged("Department_chair");
            }
        }

        public string ProfileName
        {
            get
            {
                return profile;
            }
            set
            {
                profile = value;
                OnPropertyChanged("Profile_name");
            }
        }

        public string ProfileNumber
        {
            get
            {
                return profileNumber;
            }
            set
            {
                profileNumber = value;
                OnPropertyChanged("Profile_number");
            }
        }

        public string Discipline
        {
            get
            {
                return discipline;
            }
            set
            {
                discipline = value;
                OnPropertyChanged("Discipline");
            }
        }

        public string Qualification
        {
            get
            {
                return qualification;
            }
            set
            {
                qualification = value;
                OnPropertyChanged("Qualification");
            }
        }

        public string StartYear
        {
            get
            {
                return start_year;
            }
            set
            {
                start_year = value;
                OnPropertyChanged("StartYear");
            }
        }

        public string EducationForm
        {
            get
            {
                return education_form;
            }
            set
            {
                education_form = value;
                OnPropertyChanged("EducationForm");
            }
        }

        public string EducationPeriod
        {
            get
            {
                return education_period;
            }
            set
            {
                education_period = value;
                OnPropertyChanged("EducationPeriod");
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
