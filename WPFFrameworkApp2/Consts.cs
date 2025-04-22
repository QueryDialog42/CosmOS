namespace WPFFrameworkApp
{
    public struct Errors
    {
        public const string ADD_ERR = "Add Error";
        public const string READ_ERR = "Read Error";
        public const string WRT_ERR = "Write Error";
        public const string OPEN_ERR = "Open Error";
        public const string SAVE_ERR = "Save Error";
        public const string COPY_ERR = "Copy Error";
        public const string MOVE_ERR = "Move Error";
        public const string IMP_ERR = "Import Error";
        public const string CRT_ERR = "Create Error";
        public const string DEL_ERR = "Delete Error";
        public const string REL_ERR = "Reload Error";
        public const string RUN_ERR = "EXE run Error";
        public const string PRMS_ERR = "Permission Error";
        public const string UNSUPP_ERR = "Un Supported File Error";
        public const string ADD_ERR_MSG = "An Error occured while adding ";
        public const string RUN_ERR_MSG = "An error occured while running ";
        public const string WRT_ERR_MSG = "An error occured while writing ";
        public const string SAVE_ERR_MSG = "An error occured while saving ";
        public const string COPY_ERR_MSG = "An error occured while coping ";
        public const string MOVE_ERR_MSG = "An error occured while moving ";
        public const string READ_ERR_MSG = "An error occured while reading ";
        public const string OPEN_ERR_MSG = "An error occured while opening ";
        public const string CRT_ERR_MSG = "An error occured while creating ";
        public const string DEL_ERR_MSG = "An error occured while deleting ";
        public const string REL_ERR_MSG = "An error occured while reloading ";
        public const string IMP_ERR_MSG = "An error occured while importing ";
    }

    public struct ImagePaths
    {
        public const string NSAVE_IMG = "NoteImages/Save.png";
        public const string NMOVE_IMG = "NoteImages/move.png";
        public const string RENM_IMG = "NoteImages/rename.png";
        public const string NOTE_IMG = "NoteImages/gennote.png";
        public const string NADD_IMG = "NoteImages/noteadd.png";
        public const string JPG_IMG = "DesktopImages/JPGpic.png";
        public const string RTF_IMG = "DesktopImages/rtffile.png";
        public const string SADD_IMG = "MusicImages/soundadd.png";
        public const string NCOPY_IMG = "NoteImages/notecopy.png";
        public const string FRSH_IMG = "DesktopImages/refresh.png";
        public const string TXT_IMG = "DesktopImages/textfile.png";
        public const string WAV_IMG = "DesktopImages/soundwav.png";
        public const string MSC_IMG = "DesktopImages/Genmusic.png";
        public const string COPYPIC = "DesktopImages/copypict.png";
        public const string NDEL_IMG = "NoteImages/notedelete.png";
        public const string WRNG_IMG = "DesktopImages/warning1.png";
        public const string QST_IMG = "DesktopImages/question2.png";
        public const string MP4_IMG = "DesktopImages/Videoicon.png";
        public const string ROTATE_IMG = "DesktopImages/Rotate.png";
        public const string PVAPP_IMG = "DesktopImages/picmovie.png";
        public const string FULL_IMG = "DesktopImages/trashfull.png";
        public const string EXE_IMG = "DesktopImages/exepenguin.png";
        public const string FOLDER_IMG = "DesktopImages/folder2.png";
        public const string SDEL_IMG = "MusicImages/sounddelete.png";
        public const string EMPT_IMG = "DesktopImages/trashempty.png";
        public const string ADDMP4_IMG = "DesktopImages/movieadd.png";
        public const string UNKNOWN_IMG = "DesktopImages/unknown.png";
        public const string NEWFOL_IMG = "DesktopImages/newfolder.png";
        public const string LOGO_IMG = "DesktopImages/paperplane2.png";
        public const string MP3_IMG = "DesktopImages/sound128x128.png";
        public const string DELEXE_IMG = "DesktopImages/deleteexe.png";
        public const string PNG_IMG = "DesktopImages/Picture96x96.png";
        public const string SCOPY_IMG = "MusicImages/soundbutcopy.png";
        public const string INFO_IMG = "DesktopImages/information.png";
        public const string DELPNG_IMG = "DesktopImages/delpicture.png";
        public const string ADDPIC_IMG = "DesktopImages/addpicture.png";
        public const string LMSC_IMG = "DesktopImages/genmusic32x32.png";
        public const string LWAV_IMG = "DesktopImages/soundwav48x48.png";
        public const string LMP3_IMG = "DesktopImages/soundmp348x48.png";
        public const string FDEL_IMG = "DesktopImages/folderdelete2.png";
        public const string DELMP4_IMG = "DesktopImages/videodelete.png";
        public const string COPYMP4_IMG = "DesktopImages/moviebutcopy.png";
        public const string HLOGO_IMG = "DesktopImages/paperplane128x128.png";
    }

    public struct HiddenFolders
    {
        public const string HAUD_FOL = ".audios$";
        public const string HTRSH_FOL = ".trash$";
        public const string HPV_FOL = ".picsvids$";
    }

    public struct Configs
    {
        public const string CDESK = "C_DESKTOP";
        public const string CCOL = "Ccolor.txt";
        public const string CFONT = "Cfont.txt";
        public const string CPATH = "CDesktop.txt";
        public const string C_CONFIGS = "C_CONFIGS";
    }

    public struct SupportedFiles
    {
        public const string TXT = ".txt";
        public const string RTF = ".rtf";
        public const string WAV = ".wav";
        public const string MP3 = ".mp3";
        public const string EXE = ".exe";
        public const string PNG = ".png";
        public const string JPG = ".jpg";
        public const string MP4 = ".mp4";
    }

    public struct Versions
    {
        public const string GOS_VRS = "GencOS v2.3.5";
        public const string MAIL_VRS = "GenMail v0.0.5";
        public const string NOTE_VRS = "GenNote v1.5.0";
        public const string MUSIC_VRS = "GenMusic v1.7.1";
        public const string PICMOV_VRS = "GenMovie v0.0.6";
    }

    public struct MainItems
    {
        public const string MAIN_WIN = "GencOS main";
    }

    public struct Messages
    {
        public const string ABT_DFLT_MSG = "Made with C# 8.0.115 in Visual Studio\n2025 - No licence"; // about page's default message
    }

    public struct Defaults
    {
        public const string FONT = "Arial";
        public const string FONT_SIZE = "12";
        public const string FONT_COL = "Black";
        public const string MENU_COL = "#f7f7f7";
        public const string FONT_STYLE = "Normal";
        public const string SAFARI_COL = "#5c5c5c";
        public const string FONT_WEIGHT = "Regular";
        public const string FOL_DESK_COL = "#9c9268";
        public const string MAIN_DESK_COl = "LightGray";
    }

    public struct AppTitles
    {
        public const string APP_WIN = "GencOS";
        public const string APP_MAIL = "GenMail";
        public const string APP_NOTE = "GenNote";
        public const string APP_MUSIC = "GenMusic";
        public const string APP_PICMOV = "GenMovie";
    }
}
