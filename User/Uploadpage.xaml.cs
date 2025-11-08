using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using Windows.Storage.Provider;

namespace Work_winui_.User
{
    public sealed partial class Uploadpage : UserControl
    {
        public Uploadpage()
        {
            this.InitializeComponent();
            
            // Set default date to today
            DatePicker.Date = DateTime.Today;
        }

        private void HeaterTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Hide all forms first
            CartridgeHeaterForm.Visibility = Visibility.Collapsed;
            //TubularHeaterForm.Visibility = Visibility.Collapsed;
            //MicaBandHeaterForm.Visibility = Visibility.Collapsed;
            //CeramicBandHeaterForm.Visibility = Visibility.Collapsed;

            if (HeaterTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "HIGH DENSITY CARTRIDGE HEATER":
                        CartridgeHeaterForm.Visibility = Visibility.Visible;
                        break;
                    case "TUBULAR HEATER":
                        TubularHeaterForm.Visibility = Visibility.Visible;
                       break;
                    //case "MICA BAND HEATER":
                       // MicaBandHeaterForm.Visibility = Visibility.Visible;
                       // break;
                    //case "CERAMIC BAND HEATER":
                       // CeramicBandHeaterForm.Visibility = Visibility.Visible;
                        //break;
                }
            }
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(CustomerNameTextBox.Text))
            {
                ShowErrorMessage("Please enter a customer name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(OrderNoTextBox.Text))
            {
                ShowErrorMessage("Please enter an order number.");
                return;
            }

            if (string.IsNullOrWhiteSpace(DiameterTextBox.Text) || 
                string.IsNullOrWhiteSpace(LengthTextBox.Text) ||
                string.IsNullOrWhiteSpace(WattsTextBox.Text) ||
                string.IsNullOrWhiteSpace(VoltsTextBox.Text))
            {
                ShowErrorMessage("Please enter all dimensions and electrical specifications.");
                return;
            }

            try
            {
                // Open file picker to select a file
                var file = await PickFileAsync();
                
                if (file != null)
                {
                    // Process the selected file
                    await ProcessSelectedFileAsync(file);
                    
                    // Show success message
                    ShowSuccessMessage("Jobsheet uploaded successfully!");
                    
                    // Clear form
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"An error occurred: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task<StorageFile> PickFileAsync()
        {
            // Create file picker
            var openPicker = new FileOpenPicker();
            
            // Get the window handle from the current application main window
            // Use the Application's MainWindow property which we added to App.xaml.cs
            if (App.Current is App app && app.MainWindow != null)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(app.MainWindow);
                // Initialize the file picker with the window handle
                InitializeWithWindow.Initialize(openPicker, hwnd);
            }
            
            // Configure file picker
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".txt");
            openPicker.FileTypeFilter.Add(".csv");
            openPicker.FileTypeFilter.Add(".pdf");
            openPicker.FileTypeFilter.Add(".docx");
            
            // Open the picker for the user to select a file
            return await openPicker.PickSingleFileAsync();
        }

        private async System.Threading.Tasks.Task ProcessSelectedFileAsync(StorageFile file)
        {
            try {
                // Create a jobsheet content string
                string jobsheetContent = CreateJobsheetContent();
                
                // Get the file properties
                var properties = await file.GetBasicPropertiesAsync();
                
                // Create a folder in the application's local folder to store the uploaded files
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFolder uploadFolder = await localFolder.CreateFolderAsync("Uploads", CreationCollisionOption.OpenIfExists);
                
                // Create a new file with a unique name
                string orderNo = OrderNoTextBox.Text.Replace("/", "_").Replace("\\", "_").Replace(":", "_");
                string fileName = $"Jobsheet_{orderNo}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                StorageFile outputFile = await uploadFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                
                // Write the jobsheet content to the file
                await FileIO.WriteTextAsync(outputFile, jobsheetContent);
                
                System.Diagnostics.Debug.WriteLine($"Created file: {outputFile.Path}");
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error processing file: {ex.Message}");
                throw; // Re-throw to be handled by the caller
            }
        }

        private string CreateJobsheetContent()
        {
            // Get the selected heater type
            string heaterType = HeaterTypeComboBox.SelectedItem is ComboBoxItem selectedItem 
                ? selectedItem.Content.ToString() 
                : "Not specified";

            // Format the date
            string formattedDate = DatePicker.Date.ToString("yyyy-MM-dd");

            // Create a basic jobsheet content string
            string content = $"JOBSHEET\n\n" +
                             $"Heater Type: {heaterType}\n" +
                             $"Customer Name: {CustomerNameTextBox.Text}\n" +
                             $"Order No: {OrderNoTextBox.Text}\n" +
                             $"Date: {formattedDate}\n" +
                             $"Dimensions: {DiameterTextBox.Text} mm x {LengthTextBox.Text} mm\n" +
                             $"Electrical: {WattsTextBox.Text} Watts / {VoltsTextBox.Text} Volts\n";

            return content;
        }

        private void ClearForm()
        {
            CustomerNameTextBox.Text = string.Empty;
            OrderNoTextBox.Text = string.Empty;
            DatePicker.Date = DateTime.Today;
            DiameterTextBox.Text = string.Empty;
            LengthTextBox.Text = string.Empty;
            WattsTextBox.Text = string.Empty;
            VoltsTextBox.Text = string.Empty;
            
            // Reset combo box selection
            HeaterTypeComboBox.SelectedIndex = -1;
        }

        private async void ShowErrorMessage(string message)
        {
            await ShowMessageAsync("Error", message);
        }

        private async void ShowSuccessMessage(string message)
        {
            await ShowMessageAsync("Success", message);
        }
        
        private async System.Threading.Tasks.Task ShowMessageAsync(string title, string message)
        {
            try
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = title,
                    Content = message,
                    CloseButtonText = "OK"
                };

                if (this.XamlRoot != null)
                {
                    dialog.XamlRoot = this.XamlRoot;
                    await dialog.ShowAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"{title}: {message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"{title}: {message}");
            }
        }
    }
}