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

		private FFSaveData? mFFSave;
		public FFSaveData? FFSave
		{
			get { return mFFSave; }
			set
			{
				mFFSave = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FFSave)));
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
		}

		private void FileOpen(object? obj)
		{
			var dlg = new OpenFileDialog();
			if (dlg.ShowDialog() == false) return;

			var json = SaveData.Open(dlg.FileName);
			var ff = new FF6SaveData();
			if (!ff.Open(json)) return;

			mFilename = dlg.FileName;
			FFSave = ff;
			MessageBox.Show("success");
		}

		private void FileSave(object? obj)
		{
			if (FFSave == null) return;

			SaveData.Save(mFilename, FFSave.Save());
		}

		private void FileImport(object? obj)
		{
			if (FFSave == null) return;

			var dlg = new OpenFileDialog();
			if (dlg.ShowDialog() == false) return;

			if (!File.Exists(dlg.FileName)) return;
			FFSave.Open(File.ReadAllText(dlg.FileName));
		}

		private void FileExport(object? obj)
		{
			if (FFSave == null) return;

			var dlg = new SaveFileDialog();
			if (dlg.ShowDialog() == false) return;

			File.WriteAllText(dlg.FileName, FFSave.Save());
		}
	}
}
