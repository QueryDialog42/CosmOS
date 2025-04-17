using System.Windows;

namespace WPFFrameworkApp
{
    public interface IFile
    {
        void NewFile(object sender, RoutedEventArgs e);
        void OpenFile(object sender, RoutedEventArgs e);
        void SaveFile(object sender, RoutedEventArgs e);
        void SaveAsFile(object sender, RoutedEventArgs e);
        void CopyFile(object sender, RoutedEventArgs e);
        void MoveFile(object sender, RoutedEventArgs e);
        void RenameFile(object sender, RoutedEventArgs e);
        void DeleteFile(object sender, RoutedEventArgs e);
        void AboutPage(object sender, RoutedEventArgs e);
    }

    public interface IWindow
    {
        void ReloadWindow();
    }

    public interface IControlable
    {
        void Play(object sender, RoutedEventArgs e);
        void Pause(object sender, RoutedEventArgs e);
        void Restart(object sender, RoutedEventArgs e);
        void Back(object sender, RoutedEventArgs e);
        void Forward(object sender, RoutedEventArgs e);
    }
}
