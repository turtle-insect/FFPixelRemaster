using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace FFPixelRemaster
{
	internal class ViewModel : INotifyPropertyChanged
	{
		public CommandAction FileOpenCommand { get; set; }
		public CommandAction FileSaveCommand { get; set; }
		public CommandAction FileImportCommand { get; set; }
		public CommandAction FileExportCommand { get; set; }
		public CommandAction ItemAppendCommand { get; set; }

		private FF6SaveData? mSaveData;
		public FF6SaveData? SaveData
		{
			get { return mSaveData; }
			set
			{
				mSaveData = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveData)));
			}
		}
		private String mFilename = "";

		public event PropertyChangedEventHandler? PropertyChanged;

		public ViewModel()
		{
			FileOpenCommand = new CommandAction(FileOpen);
			FileSaveCommand = new CommandAction(FileSave);
			FileImportCommand = new CommandAction(FileImport);
			FileExportCommand = new CommandAction(FileExport);
			ItemAppendCommand = new CommandAction(ItemAppend);
		}

		private void FileOpen(object? obj)
		{
			var dlg = new OpenFileDialog();
			if (dlg.ShowDialog() == false) return;

			var json = FFPixelRemaster.SaveData.Open(dlg.FileName);
			var ff = new FF6SaveData();
			if (!ff.Open(json)) return;

			mFilename = dlg.FileName;
			SaveData = ff;
			MessageBox.Show("success");
		}

		private void FileSave(object? obj)
		{
			if (SaveData == null) return;

            FFPixelRemaster.SaveData.Save(mFilename, SaveData.Save());
		}

		private void FileImport(object? obj)
		{
			if (SaveData == null) return;

			var dlg = new OpenFileDialog();
			if (dlg.ShowDialog() == false) return;

			if (!File.Exists(dlg.FileName)) return;
			SaveData.Open(File.ReadAllText(dlg.FileName));
		}

		private void FileExport(object? obj)
		{
			if (SaveData == null) return;

			var dlg = new SaveFileDialog();
			if (dlg.ShowDialog() == false) return;

			File.WriteAllText(dlg.FileName, SaveData.Save());
		}

		private void ItemAppend(object? obj)
		{
			String type = (obj as String) ?? "";

			var user = (SaveData as FF6SaveData)?.UserData;
			var targets = user?.NormalItems.Targets;
			switch(type)
			{
				case "important":
					targets = user?.ImportantItems.Targets;
					break;
			}

			targets?.Add(new FF6Item() { Count = 1, ID = 2 });
		}
	}
}
