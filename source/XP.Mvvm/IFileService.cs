using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XP.Mvvm
{
  public interface IFileService
  {
    string OpenFile();
    string SaveFile();
    Stream GetOrCreateUserProfileStream(string fileName, bool createNew);
    Task<XDocument> LoadXDocumentAsync(Stream stream);
    Task SaveXDocumentAsync(Stream stream, XElement element);
  }
}