using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace Grimacizer7.DAL.Tables
{
    [Table]
    public class LevelItem : NotifyItem
    {
        private int _level;
        [Column]
        public int Level
        {
            get
            {
                return _level;
            }
            set
            {
                if (_level != value)
                {
                    NotifyPropertyChanging(() => Level);
                    _level = value;
                    NotifyPropertyChanged(() => Level);
                }
            }
        }

        private int _stars;
        [Column]
        public int Stars
        {
            get
            {
                return _stars;
            }
            set
            {
                if (_stars != value)
                {
                    NotifyPropertyChanging(() => Stars);
                    _stars = value;
                    NotifyPropertyChanged(() => Stars);
                }
            }
        }

        private int _id;
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging(() => Id);
                    _id = value;
                    NotifyPropertyChanged(() => Id);
                }
            }
        }

        [Column(IsVersion = true)]
        private Binary _version;
    }
}
