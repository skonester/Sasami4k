namespace WinView2

open System
open System.Text.Json
open System.Windows
open System.Windows.Interop
open System.Runtime.InteropServices

module private NativeMethods =
    [<DllImport("user32.dll")>]
    extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam)
    
    [<DllImport("user32.dll")>]
    extern bool ReleaseCapture()

[<ComVisible(true)>]
type WindowApi(window: Window) =
    let WM_NCLBUTTONDOWN = 0xA1
    let HT_CAPTION = 0x2
    let mutable isDark = false

    member _.Close() = window.Dispatcher.Invoke(fun () -> window.Close())
    member _.Minimize() = window.Dispatcher.Invoke(fun () -> window.WindowState <- WindowState.Minimized)
    member _.Maximize() = window.Dispatcher.Invoke(fun () -> window.WindowState <- WindowState.Maximized)
    member _.Restore() = window.Dispatcher.Invoke(fun () -> window.WindowState <- WindowState.Normal)
    member _.ToggleMaximize() = 
        window.Dispatcher.Invoke(fun () -> 
            if window.WindowState = WindowState.Maximized then
                window.WindowState <- WindowState.Normal
            else
                window.WindowState <- WindowState.Maximized)
    
    member _.SetSize(width: int, height: int) = 
        window.Dispatcher.Invoke(fun () ->
            window.Width <- float width
            window.Height <- float height)
            
    member _.Move(x: int, y: int) = 
        window.Dispatcher.Invoke(fun () ->
            window.Left <- float x
            window.Top <- float y)

    member _.DragMove() = 
        window.Dispatcher.Invoke(fun () ->
            if window.WindowState = WindowState.Normal then
                NativeMethods.ReleaseCapture() |> ignore
                let helper = WindowInteropHelper(window)
                NativeMethods.SendMessage(helper.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0) |> ignore)

    member _.SetAlwaysOnTop(top: bool) = window.Dispatcher.Invoke(fun () -> window.Topmost <- top)
    member _.SetOpacity(opacity: float) = window.Dispatcher.Invoke(fun () -> window.Opacity <- opacity)
    
    member _.Fullscreen() = 
        window.Dispatcher.Invoke(fun () ->
            if window.WindowState = WindowState.Maximized then
                window.WindowState <- WindowState.Normal
                window.Topmost <- false
            else
                window.WindowState <- WindowState.Maximized
                window.Topmost <- true)

    member _.SetDarkMode(dark: bool) = 
        window.Dispatcher.Invoke(fun () ->
            isDark <- dark
            MicaHelper.SetDarkMode window dark)

    member _.SetBackdrop(backdropType: string) = 
        window.Dispatcher.Invoke(fun () ->
            MicaHelper.ApplyBackdrop window backdropType)

    member _.IsMaximized() = window.Dispatcher.Invoke(fun () -> window.WindowState = WindowState.Maximized)
    member _.IsMinimized() = window.Dispatcher.Invoke(fun () -> window.WindowState = WindowState.Minimized)
    member _.IsFocused() = window.Dispatcher.Invoke(fun () -> window.IsActive)
    member _.IsDarkMode() = isDark
    member _.IsAlwaysOnTop() = window.Dispatcher.Invoke(fun () -> window.Topmost)
    member _.GetOpacity() = window.Dispatcher.Invoke(fun () -> window.Opacity)
    member _.IsWindows11() = MicaHelper.IsWindows11
    member _.GetWindowInfo() = WindowApi.GetWindowInfo(window, isDark)

    static member GetWindowInfo(w: Window) = WindowApi.GetWindowInfo(w, false)
    static member GetWindowInfo(w: Window, dark: bool) = 
        w.Dispatcher.Invoke(fun () ->
            let info = 
                dict [
                    "x", box w.Left
                    "y", box w.Top
                    "width", box w.Width
                    "height", box w.Height
                    "isMaximized", box (w.WindowState = WindowState.Maximized)
                    "isMinimized", box (w.WindowState = WindowState.Minimized)
                    "isFocused", box w.IsActive
                    "isDarkMode", box dark
                ]
            JsonSerializer.Serialize(info)
        )
