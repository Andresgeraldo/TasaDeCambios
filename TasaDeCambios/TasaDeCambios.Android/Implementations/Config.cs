using Xamarin.Forms;

[assembly: Dependency(typeof(TasaDeCambios.Droid.Implementations.Config))]
namespace TasaDeCambios.Droid.Implementations
{
    using SQLite.Net.Interop;
    using TasaDeCambios.Interfaces;

    public class Config : IConfig
    {
        string directoryDB;
        ISQLitePlatform platform;

        public string DirectoryDB
        {
            get {
                if (string.IsNullOrEmpty(directoryDB))
                {
                    directoryDB = System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.Personal);
                }

                return directoryDB;
            }
        }

        public ISQLitePlatform Platform
        {
            get {
                if (platform == null)
                {
                    platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
                }

                return platform;

            }
        }
    }
}