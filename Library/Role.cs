using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Library
{
    [Serializable]
    public class Role : PropertyChangedBase
    {
        private bool isSelected;
        private bool isVisible = true;

        public bool IsSelected
        {
            get { return isSelected; }
            set { RaisePropertyChanged(ref isSelected, value); }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set { RaisePropertyChanged(ref isVisible, value); }
        }

        public RoleType RoleType { get; set; }
    }
}
