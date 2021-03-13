using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class User : INotifyPropertyChanged
    {
        private string _Name;
        private string _SamAccountName;
        private string _UserPrincipalName;

        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
                this.OnPropertyChanged("FirstName");
            }
        }

        public string SamAccountName
        {
            get
            {
                return this._SamAccountName;
            }
            set
            {
                this._SamAccountName = value;
                this.OnPropertyChanged("FirstName");
            }
        }

        public string UserPrincipalName
        {
            get
            {
                return this._UserPrincipalName;
            }
            set
            {
                this._UserPrincipalName = value;
                this.OnPropertyChanged("FirstName");
            }
        }

        public User(string name)
        {
            _Name = name;
        }

        public User(string name, string samAccountName)
        {
            _Name = name;
            _SamAccountName = samAccountName;
        }

        public User(string name, string samAccountName, string userPrincipalName)
        {
            _Name = name;
            _SamAccountName = samAccountName;
            _UserPrincipalName = userPrincipalName;
        }

        protected void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
