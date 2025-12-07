using iText.Forms;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Work_winui_
{
    static class DocumentWriter
    {
        public static void WriteDocumnet(Dictionary<string, string> formData, string comboBoxSelection)
        {

            string templateName;
            string heaterTypeFolder;    

            if (comboBoxSelection == "TUBULAR HEATER")
            {
                // This is for Tubular Heater
                templateName = "Tubular_Heater_Template.pdf";
                heaterTypeFolder = "TubularHeater";
                WriteTubularHeaterDocument(formData, templateName, heaterTypeFolder);
            }
            else if (comboBoxSelection == "HIGH DENSITY CARTRIDGE HEATER")
            {
                // This is for High Density Cartridge Heater
                templateName = "JobSheetTemplate.pdf";
                heaterTypeFolder = "CartridgeHeater";
                WriteCartridgeHeaterDocument(formData, templateName, heaterTypeFolder);
            }
            else if (comboBoxSelection == "SWAGGING TO FINISHING")
            {
                // This is for Swagging
                templateName = "Swagging.pdf";
                heaterTypeFolder = "Swagging";
                WriteSwaggingDocument(formData, templateName, heaterTypeFolder);
            }
        }

        private static void WriteCartridgeHeaterDocument(Dictionary<string, string> formData, string templateName, string heaterTypeFolder)
        {
            string src = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Documents\\{templateName}";

            // Create main JobSheet folder and heater type subfolder if they don't exist
            string mainJobSheetDir = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Documents\\JobSheet";
            string outputDir = $"{mainJobSheetDir}\\{heaterTypeFolder}";

            if (!Directory.Exists(mainJobSheetDir))
            {
                Directory.CreateDirectory(mainJobSheetDir);
                Console.WriteLine($"Created main directory: {mainJobSheetDir}");
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
                Console.WriteLine($"Created heater type directory: {outputDir}");
            }

            // Get order number and create filename
            string orderNo = formData.GetValueOrDefault("ORDER_NO", "NoOrderNo")
                .Replace("/", "_").Replace("\\", "_").Replace(":", "_").Replace(" ", "_");
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string dest = $"{outputDir}\\{orderNo}_{timestamp}.pdf";

            var fieldValues = new Dictionary<string, string>
            {
                ["CUSTOMER NAME 1"] = formData.GetValueOrDefault("CUSTOMER_NAME", ""),
                ["ORDER NO"] = formData.GetValueOrDefault("ORDER_NO", ""),
                ["DATE"] = formData.GetValueOrDefault("DATE", ""),
                ["fill_6"] = formData.GetValueOrDefault("DIAMETER", ""),
                ["x"] = formData.GetValueOrDefault("LENGTH", ""),
                ["L x"] = formData.GetValueOrDefault("WATTS", ""),
                ["WATTS"] = formData.GetValueOrDefault("VOLTS", ""),
                ["DIA AFTER PROCESS"] = formData.GetValueOrDefault("DIAMETER_AFTER_PROCESS", ""),
                ["undefined"] = formData.GetValueOrDefault("TUBE_DIAMETER", ""),
                ["DIA AFTER MACHINING"] = formData.GetValueOrDefault("DIAMETER_AFTER_MACHINING", ""),
                ["TYPE OF TERMINAL"] = formData.GetValueOrDefault("TYPE_OF_TERMINAL", ""),
                ["TERMINAL STYLE"] = formData.GetValueOrDefault("TERMINAL_STYLE", ""),
                ["TUBE THICKNESS 1"] = formData.GetValueOrDefault("TUBE_THICKNESS", ""),
                ["TUBE THICKNESS 2"] = formData.GetValueOrDefault("BATCH_NO", ""),
                ["TUBE THICKNESS 3"] = formData.GetValueOrDefault("QUANTITY", ""),
                ["MGO POWDER LOT NO"] = formData.GetValueOrDefault("MGO_POWDER_LOT_NO", ""),
                ["MGO INSULATOR LOT NO 1"] = formData.GetValueOrDefault("MGO_INSULATOR_LOT_NO", ""),
                ["MGO INSULATOR LOT NO 2"] = formData.GetValueOrDefault("SS_TUBE_GRADE", ""),
                ["MGO INSULATOR LOT NO 3"] = formData.GetValueOrDefault("SS_TUBE_BATCH_NO", ""),
                ["1"] = formData.GetValueOrDefault("MGO_TUBE", ""),
                ["2"] = formData.GetValueOrDefault("PITCH", ""),
                ["3"] = formData.GetValueOrDefault("NO_OF_TURNS", ""),
                ["MGO END INSULATORS LOT NO"] = formData.GetValueOrDefault("MGO_END_INSULATORS_LOT_NO", ""),
                ["RESISTANCE WIRE PART NO"] = formData.GetValueOrDefault("RESISTANCE_WIRE_PART_NO", ""),
                ["TERMINAL WIRE LOT NO"] = formData.GetValueOrDefault("TERMINAL_WIRE_LOT_NO", ""),
                ["RESISTANCE COIL WIRE SWG"] = formData.GetValueOrDefault("RESISTANCE_COIL_WIRE", ""),
                ["Text3"] = formData.GetValueOrDefault("NEW_FIELD_1", ""), //mgo powder
                ["Text4"] = formData.GetValueOrDefault("NEW_FIELD_2", ""), //top insulator
                ["Text5"] = formData.GetValueOrDefault("NEW_FIELD_3", ""), // bottom insulator
                ["Text6"] = formData.GetValueOrDefault("NEW_FIELD_4", ""), // sillicon spacer
                ["Text7"] = formData.GetValueOrDefault("JOB_INCHARGE", ""),
                ["Text8"] = ""  // iHoD: not required
            };

            WritePdfDocument(src, dest, fieldValues, "Cartridge Heater");
        }

        private static void WriteTubularHeaterDocument(Dictionary<string, string> formData, string templateName, string heaterTypeFolder)
        {
            string src = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Documents\\{templateName}";

            // Create main JobSheet folder and heater type subfolder if they don't exist
            string mainJobSheetDir = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Documents\\JobSheet";
            string outputDir = $"{mainJobSheetDir}\\{heaterTypeFolder}";

            if (!Directory.Exists(mainJobSheetDir))
            {
                Directory.CreateDirectory(mainJobSheetDir);
                Console.WriteLine($"Created main directory: {mainJobSheetDir}");
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
                Console.WriteLine($"Created heater type directory: {outputDir}");
            }

            // Get order number and create filename
            string orderNo = formData.GetValueOrDefault("ORDER_NO", "NoOrderNo")
                .Replace("/", "_").Replace("\\", "_").Replace(":", "_").Replace(" ", "_");
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string dest = $"{outputDir}\\{orderNo}_{timestamp}.pdf";

            // Map Tubular Heater form data to PDF fields
            var fieldValues = new Dictionary<string, string>
            {
                // Basic Information
                ["CUSTOMER_NAME"] = formData.GetValueOrDefault("CUSTOMER_NAME", ""),
                ["ORDER_NO"] = formData.GetValueOrDefault("ORDER_NO", ""),
                ["DATE"] = formData.GetValueOrDefault("DATE", ""),

                // Heater Specifications
                ["TYPE_OF_HEATER"] = formData.GetValueOrDefault("HEATER_TYPE", ""),
                ["DIA"] = formData.GetValueOrDefault("TUBE_DIA", ""),
                ["BATCH"] = formData.GetValueOrDefault("BATCH_1", ""),
                ["WATT"] = formData.GetValueOrDefault("WATTS", ""),
                ["GRADE"] = formData.GetValueOrDefault("GRADE", ""),
                ["VOLT"] = formData.GetValueOrDefault("VOLTS", ""),
                ["TERMINAL_PIN_SIZE"] = formData.GetValueOrDefault("TERMINAL_PIN", ""),
                ["BATCH_2"] = formData.GetValueOrDefault("BATCH_2", ""),

                // Manufacturing Details
                ["MGO_POWDER"] = formData.GetValueOrDefault("MGO_POWDER", ""),
                ["LOT_NO"] = formData.GetValueOrDefault("LOT_NO", ""),
                ["COLD_ZONE_TOP"] = formData.GetValueOrDefault("COLD_ZONE_TOP", ""),
                ["COLD_ZONE_BOTTOM"] = formData.GetValueOrDefault("COLD_ZONE_BOTTOM", ""),
                ["BATCH_NO"] = formData.GetValueOrDefault("BATCH_NO", ""),
                ["DIA_AFTER_SWAGGING"] = formData.GetValueOrDefault("DIA_AFTER_SWAGGING", ""),

                // Delivery and Quantity Information
                ["DELIVERY_DATE"] = formData.GetValueOrDefault("DELIVERY_DATE", ""),
                ["FILL_QTY"] = formData.GetValueOrDefault("FILL_QTY", ""),
                ["ASS_QTY"] = formData.GetValueOrDefault("ASS_QTY", ""),
                ["WIRE_SWG"] = formData.GetValueOrDefault("WIRE_SWG", ""),
                ["PART_NO"] = formData.GetValueOrDefault("PART_NO", ""),
                ["JOB_INCHARGE"] = formData.GetValueOrDefault("JOB_INCHARGE", "")
            };

            WritePdfDocument(src, dest, fieldValues, "Tubular Heater");
        }

        private static void WriteSwaggingDocument(Dictionary<string, string> formData, string templateName, string heaterTypeFolder)
        {
            string src = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Documents\\{templateName}";

            // Create main JobSheet folder and heater type subfolder if they don't exist
            string mainJobSheetDir = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\Documents\\JobSheet";
            string outputDir = $"{mainJobSheetDir}\\{heaterTypeFolder}";

            if (!Directory.Exists(mainJobSheetDir))
            {
                Directory.CreateDirectory(mainJobSheetDir);
                Console.WriteLine($"Created main directory: {mainJobSheetDir}");
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
                Console.WriteLine($"Created heater type directory: {outputDir}");
            }

            // Get order number and create filename
            string orderNo = formData.GetValueOrDefault("ORDER_NO", "NoOrderNo")
                .Replace("/", "_").Replace("\\", "_").Replace(":", "_").Replace(" ", "_");
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string dest = $"{outputDir}\\{orderNo}_{timestamp}.pdf";

            // Map Swag Band Heater form data to PDF fields
            var fieldValues = new Dictionary<string, string>
            {
                // Basic Information
                ["CUSTOMER_NAME"] = formData.GetValueOrDefault("CUSTOMER_NAME", ""),
                ["ORDER_NO"] = formData.GetValueOrDefault("ORDER_NO", ""),
                ["DATE"] = formData.GetValueOrDefault("DATE", ""),

                // Electrical Specifications
                ["WATTS"] = formData.GetValueOrDefault("WATTS", ""),
                ["VOLTS"] = formData.GetValueOrDefault("VOLTS", ""),
                ["OHMS"] = formData.GetValueOrDefault("OHMS", ""),

                // Physical Specifications
                ["BATCH_NO"] = formData.GetValueOrDefault("BATCH_NO", ""),
                ["HEATER_CROSS_SECTION"] = formData.GetValueOrDefault("HEATER_CROSS_SECTION", ""),
                ["STRETCHED_LENGTH"] = formData.GetValueOrDefault("STRETCHED_LENGTH", ""),

                // Manufacturing Details
                ["COLD_ZONE"] = formData.GetValueOrDefault("COLD_ZONE", ""),
                ["TERMINAL_WIRE_LENGTH"] = formData.GetValueOrDefault("TERMINAL_WIRE_LENGTH", ""),
                ["TYPE_OF_TERMINAL"] = formData.GetValueOrDefault("TYPE_OF_TERMINAL", ""),

                // Quantity and Wire Information
                ["QUANTITY"] = formData.GetValueOrDefault("QUANTITY", ""),
                ["WIRE_COLOR_1"] = formData.GetValueOrDefault("WIRE_COLOR_1", ""),
                ["PROPERTY_1"] = formData.GetValueOrDefault("PROPERTY_1", ""),
                ["WIRE_COLOR_2"] = formData.GetValueOrDefault("WIRE_COLOR_2", ""),
                ["PROPERTY_2"] = formData.GetValueOrDefault("PROPERTY_2", ""),

                // Job Information
                ["JOB_INCHARGE"] = formData.GetValueOrDefault("JOB_INCHARGE", "")
            };

            WritePdfDocument(src, dest, fieldValues, "Swagging");
        }

        private static void WritePdfDocument(string src, string dest, Dictionary<string, string> fieldValues, string heaterType)
        {
            try
            {
                if (!File.Exists(src))
                {
                    Console.WriteLine($"Error: Template file not found: {src}");
                    Console.WriteLine($"Please ensure the {heaterType} template is placed in the Documents folder.");
                    return;
                }   

                using var pdfReader = new PdfReader(src);
                using var pdfWriter = new PdfWriter(dest);
                using var pdfDoc = new PdfDocument(pdfReader, pdfWriter);
                var form = PdfAcroForm.GetAcroForm(pdfDoc, true);

                var fields = form.GetAllFormFields();
                Console.WriteLine($"\n=== {heaterType} PDF Processing ===");

                foreach (var kvp in fieldValues)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        Console.WriteLine($"[{kvp.Key}] = {kvp.Value}");
                    }

                    if (fields.ContainsKey(kvp.Key))
                        fields[kvp.Key].SetValue(kvp.Value);
                    else if (!string.IsNullOrEmpty(kvp.Value))
                        Console.WriteLine($"Warning: Field '{kvp.Key}' not found in {heaterType} PDF template.");
                }

                Console.WriteLine($"\n{heaterType} PDF form filled successfully.");
                Console.WriteLine($"Output file: {dest}");
                Console.WriteLine($"File directory: {Path.GetDirectoryName(dest)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating {heaterType} PDF document.");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
