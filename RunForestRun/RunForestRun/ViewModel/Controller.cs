using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RunForestRun.ViewModel
{
    class Controller : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private string _tijd;

        public string tijd
        {
            get { return _tijd; }
            set
            {
                _tijd = value;
                this.OnPropertyChanged();
            }
        }

        private string _afstand;

        public string afstand
        {
            get { return _afstand; }
            set
            {
                _afstand = value;
                this.OnPropertyChanged();
            }
        }

        private string _snelheid;

        public string snelheid
        {
            get { return _snelheid; }
            set
            {
                _snelheid = value;
                this.OnPropertyChanged();
            }
        }

        private string _tempo;

        public string tempo
        {
            get { return _tempo; }
            set
            {
                _tempo = value;
                this.OnPropertyChanged();
            }
        }


        public Controller()
        {
            _snelheid = "0";
            _afstand = "0";
            _tempo = "0";
            _tijd = "0";
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
