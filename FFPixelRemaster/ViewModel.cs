using Microsoft.Win32;
using System.Windows;

namespace FFPixelRemaster
{
	internal class ViewModel
	{
		public CommandAction FileOpenCommand { get; set; }
		public CommandAction FileSaveCommand { get; set; }
		public CommandAction FileImportCommand { get; set; }
		public CommandAction FileExportCommand { get; set; }

		private SaveData mSaveData = new SaveData();

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

			MessageBox.Show(mSaveData.Open(dlg.FileName) ? "success" : "fail");
		}

		private void FileSave(object? obj)
		{
			mSaveData.Save();
		}

		private void FileImport(object? obj)
		{
			var dlg = new OpenFileDialog();
			if (dlg.ShowDialog() == false) return;

			mSaveData.Import(dlg.FileName);
		}

		private void FileExport(object? obj)
		{
			var dlg = new SaveFileDialog();
			if (dlg.ShowDialog() == false) return;

			mSaveData.Export(dlg.FileName);
		}
	}
}
