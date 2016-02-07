using Microsoft.Phone.Controls;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Grimacizer7.DAL.Tables
{
    [Table]
    public class SettingsItem : NotifyItem
    {
        private string _playerName;
        [Column]
        public string PlayerName
        {
            get
            {
                return _playerName;
            }
            set
            {
                if (_playerName != value)
                {
                    NotifyPropertyChanging(() => PlayerName);
                    _playerName = value;
                    NotifyPropertyChanged(() => PlayerName);
                }
            }
        }

        private SexType _sex;
        [Column]
        public SexType Sex
        {
            get
            {
                return _sex;
            }
            set
            {
                if (_sex != value)
                {
                    NotifyPropertyChanging(() => Sex);
                    _sex = value;
                    NotifyPropertyChanged(() => Sex);
                }
            }
        }

        private HumanRaceType _race;
        [Column]
        public HumanRaceType Race
        {
            get
            {
                return _race;
            }
            set
            {
                if (_race != value)
                {
                    NotifyPropertyChanging(() => Race);
                    _race = value;
                    NotifyPropertyChanged(() => Race);
                }
            }
        }
        
        private DateTime _lastMomentOfDeath;
        [Column]
        public DateTime LastMomentOfDeath
        {
            get
            {
                return _lastMomentOfDeath;
            }
            set
            {
                if (_lastMomentOfDeath != value)
                {
                    NotifyPropertyChanging(() => LastMomentOfDeath);
                    _lastMomentOfDeath = value;
                    NotifyPropertyChanged(() => LastMomentOfDeath);
                }
            }
        }

        private int _passedLevels;
        [Column]
        public int _16_PassedLevels
        {
            get
            {
                return _passedLevels;
            }
            set
            {
                if (_passedLevels != value)
                {
                    NotifyPropertyChanging(() => _16_PassedLevels);
                    _passedLevels = value;
                    NotifyPropertyChanged(() => _16_PassedLevels);
                }
            }
        }

        private int _survivalScore;
        [Column]
        public int _17_SurvivalScore
        {
            get
            {
                return _survivalScore;
            }
            set
            {
                if (_survivalScore != value)
                {
                    NotifyPropertyChanging(() => _17_SurvivalScore);
                    _survivalScore = value;
                    NotifyPropertyChanged(() => _17_SurvivalScore);
                }
            }
        }

        private int _multiplayerWinScore;
        [Column]
        public int _18_MultiplayerWinScore
        {
            get
            {
                return _multiplayerWinScore;
            }
            set
            {
                if (_multiplayerWinScore != value)
                {
                    NotifyPropertyChanging(() => _18_MultiplayerWinScore);
                    _multiplayerWinScore = value;
                    NotifyPropertyChanged(() => _18_MultiplayerWinScore);
                }
            }
        }

        private int _multiplayerLoseScore;
        [Column]
        public int _19_MultiplayerLoseScore
        {
            get
            {
                return _multiplayerLoseScore;
            }
            set
            {
                if (_multiplayerLoseScore != value)
                {
                    NotifyPropertyChanging(() => _19_MultiplayerLoseScore);
                    _multiplayerLoseScore = value;
                    NotifyPropertyChanged(() => _19_MultiplayerLoseScore);
                }
            }
        }

        private int _numberLivesLeft;
        [Column]
        public int _20_NumberLivesLeft
        {
            get
            {
                return _numberLivesLeft;
            }
            set
            {
                if (_numberLivesLeft != value)
                {
                    NotifyPropertyChanging(() => _20_NumberLivesLeft);
                    _numberLivesLeft = value;
                    NotifyPropertyChanged(() => _20_NumberLivesLeft);
                }
            }
        }

        private int _defaultImagePixelWidth;
        [Column]
        public int DefaultImagePixelWidth
        {
            get
            {
                return _defaultImagePixelWidth;
            }
            set
            {
                if (_defaultImagePixelWidth != value)
                {
                    NotifyPropertyChanging(() => DefaultImagePixelWidth);
                    _defaultImagePixelWidth = value;
                    NotifyPropertyChanged(() => DefaultImagePixelWidth);
                }
            }
        }

        private int _defaultImagePixelHeight;
        [Column]
        public int DefaultImagePixelHeight
        {
            get
            {
                return _defaultImagePixelHeight;
            }
            set
            {
                if (_defaultImagePixelHeight != value)
                {
                    NotifyPropertyChanging(() => DefaultImagePixelHeight);
                    _defaultImagePixelHeight = value;
                    NotifyPropertyChanged(() => DefaultImagePixelHeight);
                }
            }
        }

        private PageOrientation _phoneOrientation;
        [Column]
        public PageOrientation PhoneOrientation
        {
            get
            {
                return _phoneOrientation;
            }
            set
            {
                if (_phoneOrientation != value)
                {
                    NotifyPropertyChanging(() => PhoneOrientation);
                    _phoneOrientation = value;
                    NotifyPropertyChanged(() => PhoneOrientation);
                }
            }
        }

        private bool _initialized;
        [Column(CanBeNull = false)]
        public bool Initialized
        {
            get
            {
                return _initialized;
            }
            set
            {
                if (_initialized != value)
                {
                    NotifyPropertyChanging(() => Initialized);
                    _initialized = value;
                    NotifyPropertyChanged(() => Initialized);
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
