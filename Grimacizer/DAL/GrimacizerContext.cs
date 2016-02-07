using Grimacizer7.DAL.Tables;
using System.Data.Linq;

namespace Grimacizer7.DAL
{
    public class GrimacizerContext : DataContext
    {
        public static string ConnectionString = "Data Source=isostore:/grimacizer.sdf";
        public GrimacizerContext(string connectionString) : base(connectionString) { }

        public Table<FaceCalculationsItem> FaceCalculations;
        public Table<SettingsItem> Settings;
        public Table<PictureItem> SavedPictures;
        public Table<LevelItem> Levels;
    }
}
