using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Work_winui_.User;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Composition.SystemBackdrops;
using WinRT;
using Microsoft.UI.Windowing;
using WinRT.Interop;

namespace Work_winui_
{
    [StructLayout(LayoutKind.Sequential)]
    struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct POINT
    {
        public int x;
        public int y;
    }

    static class Win32
    {
        public const int WM_GETMINMAXINFO = 0x0024;

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern bool RemoveWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass);

        [DllImport("Comctl32.dll")]
        public static extern IntPtr DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
    }

    public delegate int SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData);

    public sealed partial class MainWindow : Window
    {
        WindowsSystemDispatcherQueueHelper m_wsdqHelper;
        MicaController m_micaController;
        SystemBackdropConfiguration m_configurationSource;

        private SUBCLASSPROC SubClassDelegate;
        private const int MinWidth = 1000;
        private const int MinHeight = 600;

        private int WindowSubClass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData)
        {
            switch (uMsg)
            {
                case Win32.WM_GETMINMAXINFO:
                    {
                        // Sent to a window when the size or position of the window is about to change.
                        MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

                        // Prevents app window from being resized below these dimensions:
                        mmi.ptMinTrackSize.x = MinWidth;
                        mmi.ptMinTrackSize.y = MinHeight;

                        Marshal.StructureToPtr(mmi, lParam, false);
                        return 0;
                    }
            }
            return (int)Win32.DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }

        public MainWindow()
        {
            this.InitializeComponent();

            TrySetMicaBackdrop();

            IntPtr _hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            SubClassDelegate = new SUBCLASSPROC(WindowSubClass);
            Win32.SetWindowSubclass(_hWnd, SubClassDelegate, 0, 0);

            // Initialize titlebar colors
            var appWindow = GetAppWindowForCurrentWindow();
            if (appWindow != null)

            this.Closed += MainWindow_Closed;

            if (ContentFrame != null)
            {
                ContentFrame.Content = new Homepage();
            }

            sideBar.NavigationItemInvoked += SideBar_NavigationItemInvoked;
        }

        //private void UpdateTitleBarColors(AppWindowTitleBar titleBar)
        //{
        //    if (titleBar == null) return;

        //    // Determine current theme from root content
        //    var theme = ((FrameworkElement)this.Content).ActualTheme;

        //    if (theme == ElementTheme.Dark)
        //    {
        //        // Dark theme colors
        //        titleBar.BackgroundColor = Microsoft.UI.Colors.Transparent;
        //        titleBar.ForegroundColor = Microsoft.UI.Colors.White;
        //        titleBar.InactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
        //        titleBar.InactiveForegroundColor = Microsoft.UI.Colors.White;
                
        //        titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
        //        titleBar.ButtonForegroundColor = Microsoft.UI.Colors.White;
        //        titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
        //        titleBar.ButtonInactiveForegroundColor = Microsoft.UI.Colors.White;
                
        //        titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(0x10, 0xFF, 0xFF, 0xFF);
        //        titleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.White;
                
        //        titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(0x20, 0xFF, 0xFF, 0xFF);
        //        titleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.White;
        //    }
        //    else
        //    {
        //        // Light theme colors
        //        titleBar.BackgroundColor = Microsoft.UI.Colors.Transparent;
        //        titleBar.ForegroundColor = Microsoft.UI.Colors.Black;
        //        titleBar.InactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
        //        titleBar.InactiveForegroundColor = Microsoft.UI.Colors.Black;
                
        //        titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
        //        titleBar.ButtonForegroundColor = Microsoft.UI.Colors.Black;
        //        titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
        //        titleBar.ButtonInactiveForegroundColor = Microsoft.UI.Colors.Black;
                
        //        titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(0x10, 0x00, 0x00, 0x00);
        //        titleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.Black;
                
        //        titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(0x20, 0x00, 0x00, 0x00);
        //        titleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.Black;
        //    }
        //}

        bool TrySetMicaBackdrop()
        {
            if (MicaController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                m_configurationSource = new SystemBackdropConfiguration();
                this.Activated += Window_Activated;
                this.Closed += Window_Closed_Mica;
                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                m_micaController = new MicaController();

                m_micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
                return true;
            }

            return false;
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (m_configurationSource != null)
            {
                m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
            }
        }

        private void Window_Closed_Mica(object sender, WindowEventArgs args)
        {
            if (m_micaController != null)
            {
                m_micaController.Dispose();
                m_micaController = null;
            }
            this.Activated -= Window_Activated;
            m_configurationSource = null;
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (m_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
            // Update titlebar colors when theme changes
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)this.Content).ActualTheme)
            {
                case ElementTheme.Dark:
                    m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark;
                    break;
                case ElementTheme.Light:
                    m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light;
                    break;
                case ElementTheme.Default:
                    m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default;
                    break;
            }
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            if (hWnd != IntPtr.Zero && SubClassDelegate != null)
            {
                Win32.RemoveWindowSubclass(hWnd, SubClassDelegate, 0);
            }
        }

        private void SideBar_NavigationItemInvoked(object sender, string tag)
        {
            UserControl? page = null;

            switch (tag)
            {
                case "Homepage":
                    page = new Homepage();
                    break;
                case "Generate":
                    page = new Generatepage();
                    break;
                case "Uploadpage":
                    page = new Uploadpage();
                    break;
                case "Searchpage":
                    page = new Searchpage();
                    break;
                case "Settings":
                    page = new Settings();
                    break;

            }

            if (page != null && ContentFrame != null)
            {
                ContentFrame.Content = page;
            }
        }

        // Add this method to MainWindow class to fix CS0103
        private AppWindow? GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            if (hWnd == IntPtr.Zero)
            {
                return null;
            }
            return AppWindow.GetFromWindowId(
                Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd)
            );
        }

        class WindowsSystemDispatcherQueueHelper
        {
            [StructLayout(LayoutKind.Sequential)]
            struct DispatcherQueueOptions
            {
                internal int dwSize;
                internal int threadType;
                internal int apartmentType;
            }

            [DllImport("CoreMessaging.dll")]
            private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

            object m_dispatcherQueueController = null;
            public void EnsureWindowsSystemDispatcherQueueController()
            {
                if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
                {
                    return;
                }

                if (m_dispatcherQueueController == null)
                {
                    DispatcherQueueOptions options;
                    options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
                    options.threadType = 2;    // DQTYPE_THREAD_CURRENT
                    options.apartmentType = 2; // DQTAT_COM_STA

                    CreateDispatcherQueueController(options, ref m_dispatcherQueueController);
                }
            }
        }
    }
}