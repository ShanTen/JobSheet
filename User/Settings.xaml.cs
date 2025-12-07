using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;

namespace Work_winui_.User
{
    public sealed partial class Settings : UserControl
    {
        public Settings()
        {
            this.InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;

                // Load saved settings
                if (localSettings.Values.ContainsKey("Endpoints"))
                {
                    EndpointsTextBox.Text = localSettings.Values["Endpoints"] as string;
                }

                if (localSettings.Values.ContainsKey("ApiKey"))
                {
                    ApiKeyPasswordBox.Password = localSettings.Values["ApiKey"] as string;
                }

                if (localSettings.Values.ContainsKey("FtpUsername"))
                {
                    UsernameTextBox.Text = localSettings.Values["FtpUsername"] as string;
                }

                if (localSettings.Values.ContainsKey("FtpPassword"))
                {
                    CredentialsPasswordBox.Password = localSettings.Values["FtpPassword"] as string;
                }

                if (localSettings.Values.ContainsKey("FormulaPassword"))
                {
                    FormulaPasswordBox.Password = localSettings.Values["FormulaPassword"] as string;
                }
            }
            catch (Exception ex)
            {
                // Handle loading errors silently or show error dialog
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }
        }

        private async void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;

                // Save settings
                localSettings.Values["Endpoints"] = EndpointsTextBox.Text;
                localSettings.Values["ApiKey"] = ApiKeyPasswordBox.Password;
                localSettings.Values["FtpUsername"] = UsernameTextBox.Text;
                localSettings.Values["FtpPassword"] = CredentialsPasswordBox.Password;
                localSettings.Values["FormulaPassword"] = FormulaPasswordBox.Password;

                // Show success message
                var dialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "Settings saved successfully!",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                // Show error message
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to save settings: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Reload settings to discard changes
            LoadSettings();
        }
    }
}
