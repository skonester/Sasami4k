namespace WinView2

open System
open System.Runtime.InteropServices
open System.Windows
open System.Windows.Interop
open System.Windows.Media

[<StructLayout(LayoutKind.Sequential)>]
type internal MARGINS = 
    struct
        val mutable Left: int
        val mutable Right: int
        val mutable Top: int
        val mutable Bottom: int
    end

module MicaHelper =
    [<DllImport("dwmapi.dll")>]
    extern int private DwmSetWindowAttribute(IntPtr hwnd, int attr, int& attrValue, int attrSize)

    [<DllImport("dwmapi.dll")>]
    extern int private DwmExtendFrameIntoClientArea(IntPtr hwnd, MARGINS& margins)

    let private DWMWA_USE_IMMERSIVE_DARK_MODE = 20
    let private DWMWA_SYSTEMBACKDROP_TYPE = 38
    let private DWMSBT_NONE = 1
    let private DWMSBT_MAINWINDOW = 2
    let private DWMSBT_TRANSIENTWINDOW = 3
    let private DWMSBT_TABBEDWINDOW = 4

    let IsWindows11 = Environment.OSVersion.Version.Build >= 22000

    let SetDarkMode (window: Window) (dark: bool) =
        if IsWindows11 then
            let hwnd = WindowInteropHelper(window).Handle
            let mutable value = if dark then 1 else 0
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, &value, sizeof<int>) |> ignore

    let ApplyBackdrop (window: Window) (backdropType: string) =
        if IsWindows11 then
            let hwnd = WindowInteropHelper(window).Handle
            let mutable value = 
                match backdropType.ToLower() with
                | "mica" -> DWMSBT_MAINWINDOW
                | "acrylic" -> DWMSBT_TRANSIENTWINDOW
                | "tabbed" -> DWMSBT_TABBEDWINDOW
                | "none" -> DWMSBT_NONE
                | _ -> DWMSBT_MAINWINDOW

            let hwndSource = HwndSource.FromHwnd(hwnd)
            if not (isNull hwndSource) then
                hwndSource.CompositionTarget.BackgroundColor <- 
                    if value <> DWMSBT_NONE then Color.FromArgb(0uy, 0uy, 0uy, 0uy)
                    else Color.FromArgb(255uy, 255uy, 255uy, 255uy)

            DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, &value, sizeof<int>) |> ignore

            let mutable margins = 
                if value <> DWMSBT_NONE then
                    let mutable m = MARGINS()
                    m.Left <- -1; m.Right <- -1; m.Top <- -1; m.Bottom <- -1
                    m
                else
                    let mutable m = MARGINS()
                    m.Left <- 0; m.Right <- 0; m.Top <- 0; m.Bottom <- 0
                    m
            DwmExtendFrameIntoClientArea(hwnd, &margins) |> ignore
