using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public class Path : INotifyPropertyChanged
    {
        private string _Name;
        private string _Type;
        private IdentityReference _IdentityReference;
        private AccessControlType _AccessControlType;
        private FileSystemRights _FileSystemRights;

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

        public string Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
                this.OnPropertyChanged("FirstName");
            }
        }

        public IdentityReference IdentityReference
        {
            get
            {
                return this._IdentityReference;
            }
            set
            {
                this._IdentityReference = value;
                this.OnPropertyChanged("FirstName");
            }
        }

        public AccessControlType AccessControlType
        {
            get
            {
                return this._AccessControlType;
            }
            set
            {
                this._AccessControlType = value;
                this.OnPropertyChanged("FirstName");
            }
        }

        public FileSystemRights FileSystemRights
        {
            get
            {
                return this._FileSystemRights;
            }
            set
            {
                this._FileSystemRights = value;
                this.OnPropertyChanged("FirstName");
            }
        }

        public Path(string name)
        {
            _Name = name;
        }

        public Path(string name, string type)
        {
            _Name = name;
            _Type = type;
        }

        public Path(string name, string type, IdentityReference identityReference, AccessControlType accessControlType, FileSystemRights fileSystemRights)
        {
            _Name = name;
            _Type = type;
            _IdentityReference = identityReference;
            _AccessControlType = accessControlType;
            _FileSystemRights = fileSystemRights;
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
