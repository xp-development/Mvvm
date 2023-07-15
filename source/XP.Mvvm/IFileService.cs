using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XP.Mvvm
{
  public interface IFileService
  {
    string OpenFileDialog();
    string SaveFileDialog();
    Stream GetOrCreateUserProfileStream(string fileName, bool createNew);
    Stream OpenFile(string filePath);
    bool IsFileExists(string filePath);
  }
}