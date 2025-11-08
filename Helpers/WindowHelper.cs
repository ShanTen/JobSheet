using Microsoft.UI.Xaml;
using System;

namespace Work_winui_
{
    /// <summary>
    /// Helper class for Window related operations.
    /// </summary>
    public static class WindowHelper
    {
        /// <summary>
        /// Gets the main window, assuming it's available through App.MainWindow.
        /// </summary>
        /// <returns>The main window or null.</returns>
        public static Window? GetMainWindow()
        {
            if (Application.Current is App app)
            {
                return app.MainWindow;
            }
            
            return null;
        }
    }
}
