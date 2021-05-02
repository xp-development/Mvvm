using Microsoft.Win32;

namespace XP.Mvvm
{
  public class FileService : IFileService
  {
    public string OpenFile()
    {
      var dialog = new OpenFileDialog();
      dialog.ShowDialog();
      return dialog.FileName;
    }

    public string SaveFile()
    {
      var dialog = new SaveFileDialog();
      dialog.ShowDialog();
      return dialog.FileName;
    }
  }
}