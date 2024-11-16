using System.IO;
using System.Text.Json;
using System.Windows;
using Library.Models.Dcs.Modules.Oh58;
using Microsoft.Win32;

namespace Planner.Core.Service;

// Usage:
//var cancellationTokenSource = new CancellationTokenSource();
//await MissionFileExporter.ExportMissionFileAsync(yourMissionFileInstance, cancellationTokenSource.Token);

public static class MissionFileExporter
{
	public static async Task ExportMissionFileAsync(MissionFile missionFile, CancellationToken cancellationToken)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog
		{
			Filter = "JSON files (*.json)|*.json",
			Title = "Save Mission File",
			FileName ="Mission1.json"
		};

		if (saveFileDialog.ShowDialog() == true)
		{
			string filePath = saveFileDialog.FileName;

			try
			{
				string jsonString = JsonSerializer.Serialize(missionFile, new JsonSerializerOptions { WriteIndented = true });
				await File.WriteAllTextAsync(filePath, jsonString, cancellationToken);
				MessageBox.Show("Mission file saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (OperationCanceledException)
			{
				MessageBox.Show("File save operation was canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to save mission file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}

