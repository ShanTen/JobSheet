using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Work_winui_.User
{
    public sealed partial class Generatepage : UserControl
    {
        // Define the export directory
        private const string ExportDirectory = @"C:\Work\Work(winui)\data";

        public Generatepage()
        {
            this.InitializeComponent();
            
            // Ensure the export directory exists
            EnsureExportDirectoryExists();
            //set default date to today
            DatePicker.Date = DateTime.Today;
            //DatePickerTube.Date = DateTime.Today;
            //DeliveryDateTube.Date = DateTime.Today;
            //DatePickerSwag.Date = DateTime.Today;
            HeaterTypeComboBox.AddHandler(PointerWheelChangedEvent, new PointerEventHandler(OnPointerWheelChangedSuppressScroll), true);
        }

        private void EnsureExportDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(ExportDirectory))
                {
                    Directory.CreateDirectory(ExportDirectory);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating export directory: {ex.Message}");
            }
        }

        private void HeaterTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Don't do anything if the form hasn't been initialized yet.
            if (CartridgeHeaterForm == null)
            {
                return;
            }

            // Hide all forms first
            CartridgeHeaterForm.Visibility = Visibility.Collapsed;
            //TubularHeaterForm.Visibility = Visibility.Collapsed;
            //SwaggingForm.Visibility = Visibility.Collapsed;

            if (HeaterTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "HIGH DENSITY CARTRIDGE HEATER":
                        CartridgeHeaterForm.Visibility = Visibility.Visible;
                        break;
                    //case "TUBULAR HEATER":
                    //    TubularHeaterForm.Visibility = Visibility.Visible;
                    //    break;
                    //case "SWAGGING TO FINISHING":
                    //    SwaggingForm.Visibility = Visibility.Visible;
                    //    break;
                }
            }
        }

        private void OnPointerWheelChangedSuppressScroll(object sender, PointerRoutedEventArgs e)
        {
            // Set Handled to true to prevent the scroll event from propagating to the control's selection logic
            e.Handled = true;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Parent is Grid grid)
            {
                var textBox = grid.Children.OfType<TextBox>().FirstOrDefault();
                if (textBox != null)
                {
                    textBox.IsReadOnly = false;
                    // Clear any locally-set Background so theme resource applies
                    textBox.ClearValue(Control.BackgroundProperty);
                    textBox.Focus(FocusState.Programmatic);
                }
            }
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // First check if heater type is selected
            if (HeaterTypeComboBox.SelectedIndex <= 0 || HeaterTypeComboBox.SelectedItem == null)
            {
                await ShowMessageAsync("Error", "Please select a heater type.");
                HeaterTypeComboBox.Focus(FocusState.Programmatic);
                return;
            }

            // Determine which form is visible
            string formType = "Unknown";
            if (CartridgeHeaterForm.Visibility == Visibility.Visible)
            {
                formType = "High Density Cartridge Heater";
                await CollectAndSaveCartridgeHeaterData();
            }
            else
            {
                await ShowMessageAsync("Error", "Please select a valid heater type.");
            }
        }


        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement calculation logic here
        }

        private async Task CollectAndSaveCartridgeHeaterData()
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(CustomerNameTextBox.Text))
                {
                    await ShowMessageAsync("Error", "Please enter a customer name.");
                    CustomerNameTextBox.Focus(FocusState.Programmatic);
                    return;
                }

                if (string.IsNullOrWhiteSpace(OrderNoTextBox.Text))
                {
                    await ShowMessageAsync("Error", "Please enter an order number.");
                    OrderNoTextBox.Focus(FocusState.Programmatic);
                    return;
                }

                // Only validate the main input fields that don't have edit buttons
                if (string.IsNullOrWhiteSpace(DiameterTextBox.Text) || 
                    string.IsNullOrWhiteSpace(LengthTextBox.Text) ||
                    string.IsNullOrWhiteSpace(WattsTextBox.Text) ||
                    string.IsNullOrWhiteSpace(VoltsTextBox.Text))
                {
                    await ShowMessageAsync("Error", "Please enter all dimensions and electrical specifications.");
                    if (string.IsNullOrWhiteSpace(DiameterTextBox.Text))
                        DiameterTextBox.Focus(FocusState.Programmatic);
                    else if (string.IsNullOrWhiteSpace(LengthTextBox.Text))
                        LengthTextBox.Focus(FocusState.Programmatic);
                    else if (string.IsNullOrWhiteSpace(WattsTextBox.Text))
                        WattsTextBox.Focus(FocusState.Programmatic);
                    else
                        VoltsTextBox.Focus(FocusState.Programmatic);
                    return;
                }

                // Create a dictionary to store all the form data
                var formData = new Dictionary<string, string>();
                
                // Directly collect data from named controls
                formData["CUSTOMER_NAME"] = CustomerNameTextBox.Text;
                formData["ORDER_NO"] = OrderNoTextBox.Text;
                formData["DATE"] = DatePicker.Date.ToString("yyyy-MM-dd");
                
                formData["DIAMETER"] = DiameterTextBox.Text;
                formData["LENGTH"] = LengthTextBox.Text;
                formData["WATTS"] = WattsTextBox.Text;
                formData["VOLTS"] = VoltsTextBox.Text;
                
                formData["DIAMETER_AFTER_PROCESS"] = DiameterAfterProcessTextBox.Text;
                formData["TUBE_DIAMETER"] = TubeDiameterTextBox.Text;
                formData["DIAMETER_AFTER_MACHINING"] = DiameterAfterMachiningTextBox.Text;
                formData["TUBE_THICKNESS"] = TubeThicknessTextBox.Text;
                
                formData["TYPE_OF_TERMINAL"] = TerminalTypeTextBox.Text;
                formData["BATCH_NO"] = BatchNoTextBox.Text;
                formData["TERMINAL_STYLE"] = TerminalStyleTextBox.Text;
                formData["QUANTITY"] = QuantityTextBox.Text;

                formData["NEW_FIELD_1"] = NewField1TextBox.Text;
                formData["NEW_FIELD_2"] = NewField2TextBox.Text;
                formData["NEW_FIELD_3"] = NewField3TextBox.Text;
                formData["NEW_FIELD_4"] = NewField4TextBox.Text;

                formData["MGO_POWDER_LOT_NO"] = MgoPowderLotNoTextBox.Text;
                formData["MGO_TUBE"] = MgoTubeTextBox.Text;
                formData["MGO_INSULATOR_LOT_NO"] = MgoInsulatorLotNoTextBox.Text;
                formData["PITCH"] = PitchTextBox.Text;
                
                formData["SS_TUBE_GRADE"] = SsTubeGradeTextBox.Text;
                formData["NO_OF_TURNS"] = NoOfTurnsTextBox.Text;
                formData["SS_TUBE_BATCH_NO"] = SsTubeBatchNoTextBox.Text;
                formData["MGO_END_INSULATORS_LOT_NO"] = MgoEndInsulatorsLotNoTextBox.Text;
                formData["RESISTANCE_WIRE_PART_NO"] = ResistanceWirePartNoTextBox.Text;
                formData["TERMINAL_WIRE_LOT_NO"] = TerminalWireLotNoTextBox.Text;
                formData["RESISTANCE_COIL_WIRE"] = ResistanceCoilWireTextBox.Text;
                formData["JOB_INCHARGE"] = JobInchargeTextBox.Text;
                
                string heaterType = "unknown";
                if (HeaterTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    heaterType = selectedItem.Content.ToString();
                    formData["HEATER_TYPE"] = heaterType;
                }

                // Print the collected data to console
                string jsonString = JsonSerializer.Serialize(formData, new JsonSerializerOptions { WriteIndented = true });
                System.Diagnostics.Debug.WriteLine("Form Data:");
                System.Diagnostics.Debug.WriteLine(jsonString);

                // Save to JSON file
                string orderNo = OrderNoTextBox.Text.Replace("/", "_").Replace("\\", "_").Replace(":", "_");
                string fileName = $"Jobsheet_{heaterType.Replace(" ", "_")}_{orderNo}.json";
                string filePath = Path.Combine(ExportDirectory, fileName);

                // Write the JSON to file
                File.WriteAllText(filePath, jsonString);
                
                // Show success message
                await ShowMessageAsync("Success", $"Jobsheet Generated Successfully");
                

                DocumentWriter.WriteDocumnet(formData, heaterType);

                // Clear the form
                ClearForm();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error collecting form data: {ex.Message}");
                await ShowMessageAsync("Error", $"Failed to collect form data: {ex.Message}");
            }
        }

        //private async Task CollectAndSaveTubularHeaterData()
        //{
        //    try
        //    {
        //        // Validate required fields
        //        if (string.IsNullOrWhiteSpace(CustomerNameTube.Text))
        //        {
        //            await ShowMessageAsync("Error", "Please enter a customer name.");
        //            CustomerNameTube.Focus(FocusState.Programmatic);
        //            return;
        //        }

        //        if (string.IsNullOrWhiteSpace(OrderNoTube.Text))
        //        {
        //            await ShowMessageAsync("Error", "Please enter an order number.");
        //            OrderNoTube.Focus(FocusState.Programmatic);
        //            return;
        //        }

        //        // Validate main input fields
        //        if (string.IsNullOrWhiteSpace(HeaterTypeTube.Text) || 
        //            string.IsNullOrWhiteSpace(TubeDiaTube.Text) ||
        //            string.IsNullOrWhiteSpace(WattsTube.Text) ||
        //            string.IsNullOrWhiteSpace(VoltsTube.Text))
        //        {
        //            await ShowMessageAsync("Error", "Please enter all basic specifications (Type, Tube Dia, Watts, Volts).");
        //            if (string.IsNullOrWhiteSpace(HeaterTypeTube.Text))
        //                HeaterTypeTube.Focus(FocusState.Programmatic);
        //            else if (string.IsNullOrWhiteSpace(TubeDiaTube.Text))
        //                TubeDiaTube.Focus(FocusState.Programmatic);
        //            else if (string.IsNullOrWhiteSpace(WattsTube.Text))
        //                WattsTube.Focus(FocusState.Programmatic);
        //            else
        //                VoltsTube.Focus(FocusState.Programmatic);
        //            return;
        //        }

        //        // Create a dictionary to store all the form data
        //        var formData = new Dictionary<string, string>();
                
        //        // Collect basic information
        //        formData["CUSTOMER_NAME"] = CustomerNameTube.Text;
        //        formData["ORDER_NO"] = OrderNoTube.Text;
        //        formData["DATE"] = DatePickerTube.Date.ToString("yyyy-MM-dd");
                
        //        // Collect heater specifications
        //        formData["HEATER_TYPE"] = HeaterTypeTube.Text;
        //        formData["TUBE_DIA"] = TubeDiaTube.Text;
        //        formData["BATCH_1"] = Batch1Tube.Text;
        //        formData["WATTS"] = WattsTube.Text;
        //        formData["GRADE"] = GradeTube.Text;
        //        formData["VOLTS"] = VoltsTube.Text;
        //        formData["TERMINAL_PIN"] = TerminalPinTube.Text;
        //        formData["BATCH_2"] = Batch2Tube.Text;
                
        //        // Collect manufacturing details
        //        formData["MGO_POWDER"] = MgoPowderTube.Text;
        //        formData["LOT_NO"] = LotNoTube.Text;
        //        formData["COLD_ZONE_TOP"] = ColdZoneTopTube.Text;
        //        formData["COLD_ZONE_BOTTOM"] = ColdZoneBottomTube.Text;
        //        formData["BATCH_NO"] = BatchNoTube.Text;
        //        formData["DIA_AFTER_SWAGGING"] = DiaAfterSwaggingTube.Text;
                
        //        // Collect delivery and quantity information
        //        formData["DELIVERY_DATE"] = DeliveryDateTube.Date.ToString("yyyy-MM-dd");
        //        formData["FILL_QTY"] = FillQtyTube.Text;
        //        formData["ASS_QTY"] = AssQtyTube.Text;
        //        formData["WIRE_SWG"] = WireSWGTube.Text;
        //        formData["PART_NO"] = PartNoTube.Text;
        //        formData["JOB_INCHARGE"] = JobInchargeTube.Text;
                
        //        // Add heater type identifier
        //        string heaterType = "unknown";
        //        if (HeaterTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
        //        {
        //            heaterType = selectedItem.Content.ToString();
        //        }

        //        // Print the collected data to console
        //        string jsonString = JsonSerializer.Serialize(formData, new JsonSerializerOptions { WriteIndented = true });
        //        System.Diagnostics.Debug.WriteLine("Tubular Heater Form Data:");
        //        System.Diagnostics.Debug.WriteLine(jsonString);

        //        // Save to JSON file
        //        string orderNo = OrderNoTube.Text.Replace("/", "_").Replace("\\", "_").Replace(":", "_");
        //        string fileName = $"Jobsheet_{heaterType.Replace(" ", "_")}_{orderNo}.json";
        //        string filePath = Path.Combine(ExportDirectory, fileName);

        //        // Write the JSON to file
        //        File.WriteAllText(filePath, jsonString);
                
        //        // Show success message
        //        await ShowMessageAsync("Success", $"Tubular Heater Jobsheet Generated Successfully");

        //        DocumentWriter.WriteDocumnet(formData, heaterType);

        //        // Clear the form
        //        ClearForm();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error collecting tubular heater form data: {ex.Message}");
        //        await ShowMessageAsync("Error", $"Failed to collect tubular heater form data: {ex.Message}");
        //    }
        //}

        //private async Task CollectAndSaveSwagData()
        //{
        //    try
        //    {
        //        // Validate required fields
        //        if (string.IsNullOrWhiteSpace(CustomerNameSwag.Text))
        //        {
        //            await ShowMessageAsync("Error", "Please enter a customer name.");
        //            CustomerNameSwag.Focus(FocusState.Programmatic);
        //            return;
        //        }

        //        if (string.IsNullOrWhiteSpace(OrderNoSwag.Text))
        //        {
        //            await ShowMessageAsync("Error", "Please enter an order number.");
        //            OrderNoSwag.Focus(FocusState.Programmatic);
        //            return;
        //        }

        //        // Validate main input fields
        //        if (string.IsNullOrWhiteSpace(WattsSwag.Text) || 
        //            string.IsNullOrWhiteSpace(VoltsSwag.Text))
        //        {
        //            await ShowMessageAsync("Error", "Please enter Watts and Volts specifications.");
        //            if (string.IsNullOrWhiteSpace(WattsSwag.Text))
        //                WattsSwag.Focus(FocusState.Programmatic);
        //            else
        //                VoltsSwag.Focus(FocusState.Programmatic);
        //            return;
        //        }

        //        // Create a dictionary to store all the form data
        //        var formData = new Dictionary<string, string>();
                
        //        // Collect basic information
        //        formData["CUSTOMER_NAME"] = CustomerNameSwag.Text;
        //        formData["ORDER_NO"] = OrderNoSwag.Text;
        //        formData["DATE"] = DatePickerSwag.Date.ToString("yyyy-MM-dd");
                
        //        // Collect specifications
        //        formData["WATTS"] = WattsSwag.Text;
        //        formData["VOLTS"] = VoltsSwag.Text;
        //        formData["BATCH_NO"] = BatchNoSwag.Text;
        //        formData["HEATER_CROSS_SECTION"] = HeaterCrossSectionSwag.Text;
        //        formData["STRETCHED_LENGTH"] = StretchedLengthSwag.Text;
        //        formData["OHMS"] = OhmsSwag.Text;
        //        formData["COLD_ZONE"] = ColdZoneSwag.Text;
        //        formData["TERMINAL_WIRE_LENGTH"] = TerminalWireLengthSwag.Text;
        //        formData["TYPE_OF_TERMINAL"] = TypeOfTerminalSwag.Text;
        //        formData["QUANTITY"] = QuantitySwag.Text;
        //        formData["WIRE_COLOR_1"] = WireColor1Swag.Text;
        //        formData["PROPERTY_1"] = Property1Swag.Text;
        //        formData["WIRE_COLOR_2"] = WireColor2Swag.Text;
        //        formData["PROPERTY_2"] = Property2Swag.Text;
        //        formData["JOB_INCHARGE"] = JobInchargeSwag.Text;
                
        //        // Add heater type identifier
        //        string heaterType = "unknown";
        //        if (HeaterTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
        //        {
        //            heaterType = selectedItem.Content.ToString();
        //        }

        //        // Print the collected data to console
        //        string jsonString = JsonSerializer.Serialize(formData, new JsonSerializerOptions { WriteIndented = true });
        //        System.Diagnostics.Debug.WriteLine("Swag Form Data:");
        //        System.Diagnostics.Debug.WriteLine(jsonString);

        //        // Save to JSON file
        //        string orderNo = OrderNoSwag.Text.Replace("/", "_").Replace("\\", "_").Replace(":", "_");
        //        string fileName = $"Jobsheet_{heaterType.Replace(" ", "_")}_{orderNo}.json";
        //        string filePath = Path.Combine(ExportDirectory, fileName);

        //        // Write the JSON to file
        //        File.WriteAllText(filePath, jsonString);
                
        //        // Show success message
        //        await ShowMessageAsync("Success", $"Swagging To Finishing Jobsheet Generated Successfully");

        //        DocumentWriter.WriteDocumnet(formData, heaterType);

        //        // Clear the form
        //        ClearForm();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error collecting Swaging form data: {ex.Message}");
        //        await ShowMessageAsync("Error", $"Failed to collect Swagging form data: {ex.Message}");
        //    }
        //}

        private void ClearForm()
        {
            // Reset editable fields' readonly state and background
            void ResetEditableField(TextBox textBox)
            {
                if (textBox != null)
                {
                    textBox.IsReadOnly = true;

                    // --- Theme-Aware Background ---
                    // Use TextControlBackgroundReadOnly for the read-only background
                    if (Application.Current.Resources.TryGetValue("TextControlBackgroundReadOnly", out object bgBrushResource) && bgBrushResource is Brush themeBgBrush)
                    {
                        textBox.Background = themeBgBrush;
                    }
                    else
                    {
                        // Fallback background
                        textBox.Background = new SolidColorBrush(Microsoft.UI.Colors.LightGray);
                    }

                    // --- Theme-Aware Foreground (Text) ---
                    // Use TextControlForegroundReadOnly for the text color
                    if (Application.Current.Resources.TryGetValue("TextControlForegroundReadOnly", out object fgBrushResource) && fgBrushResource is Brush themeFgBrush)
                    {
                        textBox.Foreground = themeFgBrush;
                    }
                    else
                    {
                        // Fallback foreground
                        textBox.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Gray);
                    }
                }
            }

            // Clear Cartridge Heater form fields
            CustomerNameTextBox.Text = string.Empty;
            OrderNoTextBox.Text = string.Empty;
            DatePicker.Date = DateTime.Today;

            DiameterTextBox.Text = string.Empty;
            LengthTextBox.Text = string.Empty;
            WattsTextBox.Text = string.Empty;
            VoltsTextBox.Text = string.Empty;

            DiameterAfterProcessTextBox.Text = string.Empty;
            TubeDiameterTextBox.Text = string.Empty;
            DiameterAfterMachiningTextBox.Text = string.Empty;
            TubeThicknessTextBox.Text = string.Empty;

            TerminalTypeTextBox.Text = string.Empty;
            BatchNoTextBox.Text = string.Empty;
            TerminalStyleTextBox.Text = string.Empty;
            QuantityTextBox.Text = string.Empty;

            NewField1TextBox.Text = string.Empty;
            NewField2TextBox.Text = string.Empty;
            NewField3TextBox.Text = string.Empty;
            NewField4TextBox.Text = string.Empty;

            MgoPowderLotNoTextBox.Text = string.Empty;
            MgoTubeTextBox.Text = string.Empty;
            MgoInsulatorLotNoTextBox.Text = string.Empty;
            PitchTextBox.Text = string.Empty;

            SsTubeGradeTextBox.Text = string.Empty;
            NoOfTurnsTextBox.Text = string.Empty;
            SsTubeBatchNoTextBox.Text = string.Empty;
            MgoEndInsulatorsLotNoTextBox.Text = string.Empty;
            ResistanceWirePartNoTextBox.Text = string.Empty;
            TerminalWireLotNoTextBox.Text = string.Empty;
            ResistanceCoilWireTextBox.Text = string.Empty;
            JobInchargeTextBox.Text = string.Empty;

            // Clear Tubular Heater form fields
            //CustomerNameTube.Text = string.Empty;
            //OrderNoTube.Text = string.Empty;
            //DatePickerTube.Date = DateTime.Today;

            //HeaterTypeTube.Text = string.Empty;
            //TubeDiaTube.Text = string.Empty;
            //Batch1Tube.Text = string.Empty;
            //WattsTube.Text = string.Empty;
            //GradeTube.Text = string.Empty;
            //VoltsTube.Text = string.Empty;
            //TerminalPinTube.Text = string.Empty;
            //Batch2Tube.Text = string.Empty;

            //MgoPowderTube.Text = string.Empty;
            //LotNoTube.Text = string.Empty;
            //ColdZoneTopTube.Text = string.Empty;
            //ColdZoneBottomTube.Text = string.Empty;
            //BatchNoTube.Text = string.Empty;
            //DiaAfterSwaggingTube.Text = string.Empty;

            //DeliveryDateTube.Date = DateTime.Today;
            //FillQtyTube.Text = string.Empty;
            //AssQtyTube.Text = string.Empty;
            //WireSWGTube.Text = string.Empty;
            //PartNoTube.Text = string.Empty;
            //JobInchargeTube.Text = string.Empty;

            //// Clear Swagging form fields
            //CustomerNameSwag.Text = string.Empty;
            //OrderNoSwag.Text = string.Empty;
            //DatePickerSwag.Date = DateTime.Today;

            //WattsSwag.Text = string.Empty;
            //VoltsSwag.Text = string.Empty;
            //BatchNoSwag.Text = string.Empty;
            //HeaterCrossSectionSwag.Text = string.Empty;
            //StretchedLengthSwag.Text = string.Empty;
            //OhmsSwag.Text = string.Empty;
            //ColdZoneSwag.Text = string.Empty;
            //TerminalWireLengthSwag.Text = string.Empty;
            //TypeOfTerminalSwag.Text = string.Empty;
            //QuantitySwag.Text = string.Empty;
            //WireColor1Swag.Text = string.Empty;
            //Property1Swag.Text = string.Empty;
            //WireColor2Swag.Text = string.Empty;
            //Property2Swag.Text = string.Empty;
            //JobInchargeSwag.Text = string.Empty;

            // Apply reset to the editable fields
            ResetEditableField(DiameterAfterProcessTextBox);
            ResetEditableField(TubeDiameterTextBox);
            ResetEditableField(DiameterAfterMachiningTextBox);
            ResetEditableField(TubeThicknessTextBox);
            ResetEditableField(PitchTextBox);
            ResetEditableField(ResistanceCoilWireTextBox);

            // Reset heater type selection
            HeaterTypeComboBox.SelectedIndex = 0;

            // Hide all forms
            //CartridgeHeaterForm.Visibility = Visibility.Collapsed;
            //TubularHeaterForm.Visibility = Visibility.Collapsed;
            //SwaggingForm.Visibility = Visibility.Collapsed;
        }
        private async Task ShowMessageAsync(string title, string message)
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
