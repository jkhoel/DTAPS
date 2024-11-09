using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;

namespace CAPS.Models.Files;

public static class MissionFileImporter
{
	public static async Task<MissionFile?> ImportMissionFileAsync(CancellationToken cancellationToken)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Filter = "JSON files (*.json)|*.json",
			Title = "Open Mission File"
		};

		if (openFileDialog.ShowDialog() == true)
		{
			string filePath = openFileDialog.FileName;

			try
			{
				using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					if (stream.CanRead == false)
					{
						throw new Exception("Cannot read file");
					}

					var missionFile = await JsonSerializer.DeserializeAsync<MissionFile>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

					if (missionFile is null)
					{
						throw new Exception("Failed to load mission file");
					}

					MessageBox.Show("Mission file loaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					return missionFile;
				}
			}
			catch (OperationCanceledException)
			{
				MessageBox.Show("File load operation was canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to load mission file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		return null;
	}
}
