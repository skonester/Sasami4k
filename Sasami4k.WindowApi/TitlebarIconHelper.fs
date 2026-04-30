namespace WinView2

open System
open System.Drawing
open System.IO
open System.Runtime.InteropServices
open System.Windows
open System.Windows.Interop
open System.Windows.Media.Imaging
open System.Threading.Tasks

module TitlebarIconHelper =
    [<DllImport("user32.dll", SetLastError = true)>]
    extern bool private DestroyIcon(IntPtr hIcon)

    [<DllImport("user32.dll", CharSet = CharSet.Auto)>]
    extern IntPtr private SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam)

    let private WM_SETICON = 0x0080
    let private ICON_SMALL = 0

    let SetTitlebarIcon (window: Window) (bitmap: BitmapSource) =
        let hwnd = WindowInteropHelper(window).Handle

        let encoder = PngBitmapEncoder()
        encoder.Frames.Add(BitmapFrame.Create(bitmap))

        use ms = new MemoryStream()
        encoder.Save(ms)
        ms.Position <- 0L

        use gdiBitmap = new Bitmap(ms)
        let hIcon = gdiBitmap.GetHicon()
        try
            SendMessage(hwnd, WM_SETICON, ICON_SMALL, hIcon) |> ignore
        finally
            DestroyIcon(hIcon) |> ignore

    let LoadBitmapFromUri (uri: Uri) : BitmapSource =
        try
            let bitmap = BitmapImage()
            bitmap.BeginInit()
            bitmap.UriSource <- uri
            bitmap.CacheOption <- BitmapCacheOption.OnLoad
            bitmap.EndInit()
            bitmap.Freeze()
            bitmap :> BitmapSource
        with _ ->
            null

    let LoadBitmapFromStreamAsync (stream: Stream) : Task<BitmapSource> =
        async {
            try
                use ms = new MemoryStream()
                do! stream.CopyToAsync(ms) |> Async.AwaitTask
                ms.Position <- 0L

                let bitmap = BitmapImage()
                bitmap.BeginInit()
                bitmap.StreamSource <- ms
                bitmap.CacheOption <- BitmapCacheOption.OnLoad
                bitmap.EndInit()
                bitmap.Freeze()
                return (bitmap :> BitmapSource)
            with _ ->
                return null
        } |> Async.StartAsTask
    
    let LoadBitmapFromStream (stream: Stream) : BitmapSource =
        LoadBitmapFromStreamAsync(stream).Result
