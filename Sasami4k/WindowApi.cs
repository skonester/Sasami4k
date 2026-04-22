using System.Text.Json;
using System.Windows;

namespace WinView2
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public class WindowApi
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(System.IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        private readonly MainWindow _window;
        private bool _isDark = false;

        public WindowApi(MainWindow window) => _window = window;

        // actions
        public void Close() => _window.Dispatcher.Invoke(() => _window.Close());
        public void Minimize() => _window.Dispatcher.Invoke(() => _window.WindowState = WindowState.Minimized);
        public void Maximize() => _window.Dispatcher.Invoke(() => _window.WindowState = WindowState.Maximized);
        public void Restore() => _window.Dispatcher.Invoke(() => _window.WindowState = WindowState.Normal);
        public void ToggleMaximize() => _window.Dispatcher.Invoke(() => {
            if (_window.WindowState == WindowState.Maximized)
                _window.WindowState = WindowState.Normal;
            else
                _window.WindowState = WindowState.Maximized;
        });
        public void SetSize(int width, int height) => _window.Dispatcher.Invoke(() =>
        {
            _window.Width = width;
            _window.Height = height;
        });
        public void Move(int x, int y) => _window.Dispatcher.Invoke(() =>
        {
            _window.Left = x;
            _window.Top = y;
        });

        public void DragMove() => _window.Dispatcher.Invoke(() =>
        {
            if (_window.WindowState == WindowState.Normal)
            {
                ReleaseCapture();
                SendMessage(new System.Windows.Interop.WindowInteropHelper(_window).Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        });

        public void SetAlwaysOnTop(bool top) => _window.Dispatcher.Invoke(() => _window.Topmost = top);
        public void SetOpacity(double opacity) => _window.Dispatcher.Invoke(() => _window.Opacity = opacity);
        public void Fullscreen() => _window.Dispatcher.Invoke(() =>
        {
            if (_window.WindowState == WindowState.Maximized)
            {
                _window.WindowState = WindowState.Normal;
                _window.Topmost = false;
            }
            else
            {
                _window.WindowState = WindowState.Maximized;
                _window.Topmost = true;
            }
        });

        public void SetDarkMode(bool dark) => _window.Dispatcher.Invoke(() =>
        {
            _isDark = dark;
            MicaHelper.SetDarkMode(_window, dark);
        });

        public void SetBackdrop(string type) => _window.Dispatcher.Invoke(() =>
            MicaHelper.ApplyBackdrop(_window, type));

        // getters
        public bool IsMaximized() => _window.Dispatcher.Invoke(() => _window.WindowState == WindowState.Maximized);
        public bool IsMinimized() => _window.Dispatcher.Invoke(() => _window.WindowState == WindowState.Minimized);
        public bool IsFocused() => _window.Dispatcher.Invoke(() => _window.IsActive);
        public bool IsDarkMode() => _isDark;
        public bool IsAlwaysOnTop() => _window.Dispatcher.Invoke(() => _window.Topmost);
        public double GetOpacity() => _window.Dispatcher.Invoke(() => _window.Opacity);
        public bool IsWindows11() => MicaHelper.IsWindows11;
        public string GetWindowInfo() => GetWindowInfo(_window, _isDark);

        public static string GetWindowInfo(MainWindow w, bool dark = false) => w.Dispatcher.Invoke(() =>
            JsonSerializer.Serialize(new
            {
                x = w.Left,
                y = w.Top,
                width = w.Width,
                height = w.Height,
                isMaximized = w.WindowState == WindowState.Maximized,
                isMinimized = w.WindowState == WindowState.Minimized,
                isFocused = w.IsActive,
                isDarkMode = dark
            })
        );
    }
}