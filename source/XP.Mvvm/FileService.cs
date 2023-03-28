﻿using System;
using System.IO;
using System.Reflection;
using Windows.Storage.Pickers;

namespace XP.Mvvm
{
  public class FileService : IFileService
  {
    public string OpenFileDialog()
    {
      var dialog = new FileOpenPicker();
      var task = dialog.PickSingleFileAsync().AsTask().ContinueWith(
              x => x.Result);
      return task.Result.Path;
    }

    public string SaveFileDialog()
    {
      var dialog = new FileSavePicker();
      var task = dialog.PickSaveFileAsync().AsTask().ContinueWith(
        x => x.Result);
      return task.Result.Path;
    }

    public Stream GetOrCreateUserProfileStream(string fileName, bool createNew)
    {
      var userDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      var applicationName = Assembly.GetEntryAssembly().GetName().Name;
      var userDataFilePath = Path.Combine(userDataFolder, applicationName, fileName);
      Directory.CreateDirectory(Path.Combine(userDataFolder, applicationName));
      if (createNew)
        File.Delete(userDataFilePath);

      return File.Open(userDataFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
    }

    public Stream OpenFile(string filePath)
    {
      return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete | FileShare.Inheritable);
    }
  }
}