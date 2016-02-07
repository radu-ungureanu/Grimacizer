
namespace Grimacizer7.Utils
{
    public static class Pages
    {
        private const string _folderMain = "/";
        private const string _folderViews = _folderMain + "Views/";
        private const string _folderAdventureLevels = _folderViews + "AdventureLevels/";
        private const string _folderCreateProfile = _folderViews + "CreateProfile/";
        private const string _folderGeneralGameplay = _folderViews + "GeneralGameplay/";
        private const string _folderOther = _folderViews + "Other/";
        private const string _folderSavedPictures = _folderViews + "SavedPictures/";

        private const string _xaml = ".xaml";

        public const string Intro = _folderViews + "Intro" + _xaml;
        public const string MainPage = _folderViews + "MainPage" + _xaml;

        public const string CreateProfile_CartoonCreator = _folderCreateProfile + "CartoonCreator" + _xaml;
        public const string CreateProfile_CreateProfile = _folderCreateProfile + "CreateProfile" + _xaml;
        public const string CreateProfile_ProfileCreated = _folderCreateProfile + "ProfileCreated" + _xaml;
        public const string CreateProfile_TakePhotoDefault = _folderCreateProfile + "TakePhoto" + _xaml;

        public const string GeneralGameplay_Adventure = _folderGeneralGameplay + "Adventure" + _xaml;
        public const string GeneralGameplay_Multiplayer = _folderGeneralGameplay + "Multiplayer" + _xaml;
        public const string GeneralGameplay_Survival = _folderGeneralGameplay + "Survival" + _xaml;

        public const string AdventureLevels_ShowPicture = _folderAdventureLevels + "ShowPicture" + _xaml;
        public const string AdventureLevels_TakePhoto = _folderAdventureLevels + "TakePhoto" + _xaml;

        public const string Other_EditProfile = _folderOther + "EditProfile" + _xaml;
        public const string Other_HowToPlay = _folderOther + "HowToPlay" + _xaml;
        public const string Other_Ranks = _folderOther + "Ranks" + _xaml;

        public const string PictureHistory = _folderSavedPictures + "PictureHistory" + _xaml;
        public const string PictureFullSize = _folderSavedPictures + "PictureFullSize" + _xaml;
        public const string ShareFacebook = _folderSavedPictures + "ShareFacebook" + _xaml;
    }
}
