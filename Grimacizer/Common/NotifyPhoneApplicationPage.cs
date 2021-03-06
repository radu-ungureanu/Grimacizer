﻿using Microsoft.Phone.Controls;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Grimacizer7.Common
{
    public class NotifyPhoneApplicationPage : PhoneApplicationPage, INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void NotifyPropertyChanged<T>(Expression<Func<T>> propertySelector)
        {
            RaisePropertyChanged(propertySelector.GetLastMemberName());
        }

        public event PropertyChangingEventHandler PropertyChanging;
        private void RaisePropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }
        public void NotifyPropertyChanging<T>(Expression<Func<T>> propertySelector)
        {
            RaisePropertyChanging(propertySelector.GetLastMemberName());
        }
    }
}
