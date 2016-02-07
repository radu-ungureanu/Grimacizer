using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Grimacizer7.DAL.Tables
{
    [Table]
    public class PictureItem : NotifyItem
    {
        private string _name;
        [Column]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    NotifyPropertyChanging(() => Name);
                    _name = value;
                    NotifyPropertyChanged(() => Name);
                }
            }
        }

        private int _width;
        [Column]
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (_width != value)
                {
                    NotifyPropertyChanging(() => Width);
                    _width = value;
                    NotifyPropertyChanged(() => Width);
                }
            }
        }

        private int _height;
        [Column]
        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (_height != value)
                {
                    NotifyPropertyChanging(() => Height);
                    _height = value;
                    NotifyPropertyChanged(() => Height);
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
