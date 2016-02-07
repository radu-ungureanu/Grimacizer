using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Grimacizer7.DAL.Tables
{
    [Table]
    public class FaceCalculationsItem : NotifyItem
    {
        private float _tamplaStanga;
        /// <summary>
        /// Distanta: 68-69
        /// </summary>
        [Column]
        public float _0_TamplaStanga
        {
            get
            {
                return _tamplaStanga;
            }
            set
            {
                if (_tamplaStanga != value)
                {
                    NotifyPropertyChanging(() => _0_TamplaStanga);
                    _tamplaStanga = value;
                    NotifyPropertyChanged(() => _0_TamplaStanga);
                }
            }
        }

        private float _tamplaDreapta;
        /// <summary>
        /// Distanta: 86-85
        /// </summary>
        [Column]
        public float _1_TamplaDreapta
        {
            get
            {
                return _tamplaDreapta;
            }
            set
            {
                if (_tamplaDreapta != value)
                {
                    NotifyPropertyChanging(() => _1_TamplaDreapta);
                    _tamplaDreapta = value;
                    NotifyPropertyChanged(() => _1_TamplaDreapta);
                }
            }
        }

        private float _barbie;
        /// <summary>
        /// Distanta: 75-79
        /// </summary>
        [Column]
        public float _2_Barbie
        {
            get
            {
                return _barbie;
            }
            set
            {
                if (_barbie != value)
                {
                    NotifyPropertyChanging(() => _2_Barbie);
                    _barbie = value;
                    NotifyPropertyChanged(() => _2_Barbie);
                }
            }
        }

        private float _spranceanaDreapta;
        /// <summary>
        /// Unghi: 26-27
        /// </summary>
        [Column]
        public float _3_SpranceanaDreapta
        {
            get
            {
                return _spranceanaDreapta;
            }
            set
            {
                if (_spranceanaDreapta != value)
                {
                    NotifyPropertyChanging(() => _3_SpranceanaDreapta);
                    _spranceanaDreapta = value;
                    NotifyPropertyChanged(() => _3_SpranceanaDreapta);
                }
            }
        }

        private float _arieOchiStang;
        /// <summary>
        /// Arie ochi stang
        /// </summary>
        [Column]
        public float _4_ArieOchiStang
        {
            get
            {
                return _arieOchiStang;
            }
            set
            {
                if (_arieOchiStang != value)
                {
                    NotifyPropertyChanging(() => _4_ArieOchiStang);
                    _arieOchiStang = value;
                    NotifyPropertyChanged(() => _4_ArieOchiStang);
                }
            }
        }

        private float _arieOchiDrept;
        /// <summary>
        /// Arie ochi drept
        /// </summary>
        [Column]
        public float _5_ArieOchiDrept
        {
            get
            {
                return _arieOchiDrept;
            }
            set
            {
                if (_arieOchiDrept != value)
                {
                    NotifyPropertyChanging(() => _5_ArieOchiDrept);
                    _arieOchiDrept = value;
                    NotifyPropertyChanged(() => _5_ArieOchiDrept);
                }
            }
        }

        private float _marimeOchiStang;
        /// <summary>
        /// Marime ochi stang
        /// </summary>
        [Column]
        public float _6_MarimeOchiStang
        {
            get
            {
                return _marimeOchiStang;
            }
            set
            {
                if (_marimeOchiStang != value)
                {
                    NotifyPropertyChanging(() => _6_MarimeOchiStang);
                    _marimeOchiStang = value;
                    NotifyPropertyChanged(() => _6_MarimeOchiStang);
                }
            }
        }

        private float _marimeOchiDrept;
        /// <summary>
        /// Marime ochi drept
        /// </summary>
        [Column]
        public float _7_MarimeOchiDrept
        {
            get
            {
                return _marimeOchiDrept;
            }
            set
            {
                if (_marimeOchiDrept != value)
                {
                    NotifyPropertyChanging(() => _7_MarimeOchiDrept);
                    _marimeOchiDrept = value;
                    NotifyPropertyChanged(() => _7_MarimeOchiDrept);
                }
            }
        }

        private float _inaltimeGura;
        /// <summary>
        /// Distanta: 62-66
        /// </summary>
        [Column]
        public float _8_InaltimeGura
        {
            get
            {
                return _inaltimeGura;
            }
            set
            {
                if (_inaltimeGura != value)
                {
                    NotifyPropertyChanging(() => _8_InaltimeGura);
                    _inaltimeGura = value;
                    NotifyPropertyChanged(() => _8_InaltimeGura);
                }
            }
        }

        private float _unghi_60_67;
        /// <summary>
        /// Unghi: 60-67
        /// </summary>
        [Column]
        public float _9_Unghi_60_67
        {
            get
            {
                return _unghi_60_67;
            }
            set
            {
                if (_unghi_60_67 != value)
                {
                    NotifyPropertyChanging(() => _9_Unghi_60_67);
                    _unghi_60_67 = value;
                    NotifyPropertyChanged(() => _9_Unghi_60_67);
                }
            }
        }

        private float _unghi_64_65;
        /// <summary>
        /// Unghi: 64-65
        /// </summary>
        [Column]
        public float _10_Unghi_64_65
        {
            get
            {
                return _unghi_64_65;
            }
            set
            {
                if (_unghi_64_65 != value)
                {
                    NotifyPropertyChanging(() => _10_Unghi_64_65);
                    _unghi_64_65 = value;
                    NotifyPropertyChanged(() => _10_Unghi_64_65);
                }
            }
        }

        private float _lungimeGuraExterior;
        /// <summary>
        /// Distanta: 48-45
        /// </summary>
        [Column]
        public float _11_LungimeGuraExterior
        {
            get
            {
                return _lungimeGuraExterior;
            }
            set
            {
                if (_lungimeGuraExterior != value)
                {
                    NotifyPropertyChanging(() => _11_LungimeGuraExterior);
                    _lungimeGuraExterior = value;
                    NotifyPropertyChanged(() => _11_LungimeGuraExterior);
                }
            }
        }

        private float _unghiNasStanga;
        /// <summary>
        /// Unghi: 39-38
        /// </summary>
        [Column]
        public float _12_UnghiNasStanga
        {
            get
            {
                return _unghiNasStanga;
            }
            set
            {
                if (_unghiNasStanga != value)
                {
                    NotifyPropertyChanging(() => _12_UnghiNasStanga);
                    _unghiNasStanga = value;
                    NotifyPropertyChanged(() => _12_UnghiNasStanga);
                }
            }
        }

        private float _unghiNasDreapta;
        /// <summary>
        /// Unghi: 45-44
        /// </summary>
        [Column]
        public float _13_UnghiNasDreapta
        {
            get
            {
                return _unghiNasDreapta;
            }
            set
            {
                if (_unghiNasDreapta != value)
                {
                    NotifyPropertyChanging(() => _13_UnghiNasDreapta);
                    _unghiNasDreapta = value;
                    NotifyPropertyChanged(() => _13_UnghiNasDreapta);
                }
            }
        }

        private float _arieFata;
        /// <summary>
        /// Patrat incadrator ar fetei
        /// </summary>
        [Column]
        public float _14_ArieFata
        {
            get
            {
                return _arieFata;
            }
            set
            {
                if (_arieFata != value)
                {
                    NotifyPropertyChanging(() => _14_ArieFata);
                    _arieFata = value;
                    NotifyPropertyChanged(() => _14_ArieFata);
                }
            }
        }

        private float _arieGura;
        /// <summary>
        /// Arie gura
        /// </summary>
        [Column]
        public float _15_ArieGura
        {
            get
            {
                return _arieGura;
            }
            set
            {
                if (_arieGura != value)
                {
                    NotifyPropertyChanging(() => _15_ArieGura);
                    _arieGura = value;
                    NotifyPropertyChanged(() => _15_ArieGura);
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

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;
    }
}
