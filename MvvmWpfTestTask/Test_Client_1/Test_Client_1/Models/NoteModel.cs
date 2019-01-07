using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Test_Client_1.Models
{
    public class NoteModel : INotifyPropertyChanged
    {
        public string NoteID { get; set; }

        public string Date { get; set; }

        private string _headline;

        public string Headline
        {
            get { return _headline; }
            set { _headline = value; OnPropertyChanged("Headline"); }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged("Description"); }
        }


        private string _image;

        public string Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged("Image"); }
        }

        public bool IsSelected { get; set; }

        public NoteModel() { }

        public NoteModel(string headline, string description, string date, string image)
        {
            this.Headline = headline;
            this.Description = description;
            this.Date = date;
            this.Image = image;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
