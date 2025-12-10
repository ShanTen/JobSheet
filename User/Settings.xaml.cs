using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Security.Credentials;

namespace Work_winui_.User
{
    public sealed partial class Settings : UserControl
    {
        private const string CredentialResourceFtp = "Work_WinUI_FtpPassword";
        private const string CredentialResourceFormula = "Work_WinUI_FormulaPassword";
        private const string DefaultUsername = "User";

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
                var vault = new PasswordVault();

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

                // Load FTP password from Credential Locker
                try
                {
                    var ftpCredential = vault.Retrieve(CredentialResourceFtp, DefaultUsername);
                    ftpCredential.RetrievePassword();
                    CredentialsPasswordBox.Password = ftpCredential.Password;
                }
                catch (Exception)
                {
                    // Credential not found or error retrieving - leave password box empty
                    CredentialsPasswordBox.Password = string.Empty;
                }

                // Load Formula password from Credential Locker
                try
                {
                    var formulaCredential = vault.Retrieve(CredentialResourceFormula, DefaultUsername);
                    formulaCredential.RetrievePassword();
                    FormulaPasswordBox.Password = formulaCredential.Password;
                }
                catch (Exception)
                {
                    // Credential not found or error retrieving - leave password box empty
                    FormulaPasswordBox.Password = string.Empty;
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
                var vault = new PasswordVault();

                // Save non-sensitive settings
                localSettings.Values["Endpoints"] = EndpointsTextBox.Text;
                localSettings.Values["ApiKey"] = ApiKeyPasswordBox.Password;
                localSettings.Values["FtpUsername"] = UsernameTextBox.Text;

                // Save FTP password to Credential Locker
                if (!string.IsNullOrEmpty(CredentialsPasswordBox.Password))
                {
                    try
                    {
                        // Remove existing credential if it exists
                        var existingFtpCred = vault.Retrieve(CredentialResourceFtp, DefaultUsername);
                        vault.Remove(existingFtpCred);
                    }
                    catch (Exception)
                    {
                        // Credential doesn't exist yet, which is fine
                    }

                    var ftpCredential = new PasswordCredential(
                        CredentialResourceFtp,
                        DefaultUsername,
                        CredentialsPasswordBox.Password
                    );
                    vault.Add(ftpCredential);
                }

                // Save Formula password to Credential Locker
                if (!string.IsNullOrEmpty(FormulaPasswordBox.Password))
                {
                    try
                    {
                        // Remove existing credential if it exists
                        var existingFormulaCred = vault.Retrieve(CredentialResourceFormula, DefaultUsername);
                        vault.Remove(existingFormulaCred);
                    }
                    catch (Exception)
                    {
                        // Credential doesn't exist yet, which is fine
                    }

                    var formulaCredential = new PasswordCredential(
                        CredentialResourceFormula,
                        DefaultUsername,
                        FormulaPasswordBox.Password
                    );
                    vault.Add(formulaCredential);
                }

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

        private void PDFBrowseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TemplateBrowseBrowseButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
