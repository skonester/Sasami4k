namespace WinView2

open System
open System.IO
open System.Text.Json
open System.Windows
open System.Windows.Markup
open System.Windows.Shell
open System.Windows.Media.Imaging
open Microsoft.Web.WebView2.Core
open Microsoft.Web.WebView2.Wpf
open System.Collections.Generic
open System.Threading.Tasks
open System.Linq

type FaviconInfo() =
    member val href = "" with get, set
    member val sizes = "" with get, set

type MainWindow() as this =
    inherit Window()
    
    let mutable webView: WebView2 = null
    
    do
        // Set Window Properties
        this.Title <- "Sasami4K"
        this.Height <- 450.0
        this.Width <- 800.0
        this.Background <- System.Windows.Media.Brushes.Black
        this.WindowStyle <- WindowStyle.None
        this.AllowsTransparency <- false
        this.ResizeMode <- ResizeMode.CanResize
        this.BorderThickness <- Thickness(0.0)
        
        // Load Application Icon
        try
            let iconUri = Uri("resources/winview2.ico", UriKind.Relative)
            this.Icon <- BitmapFrame.Create(iconUri)
        with _ -> ()

        // WindowChrome
        let chrome = WindowChrome()
        chrome.CaptionHeight <- 0.0
        chrome.GlassFrameThickness <- Thickness(0.0)
        chrome.CornerRadius <- CornerRadius(0.0)
        chrome.ResizeBorderThickness <- Thickness(0.0)
        WindowChrome.SetWindowChrome(this, chrome)

        // Load XAML content (Grid)
        let uri = Uri("MainWindow.xaml", UriKind.Relative)
        let stream = Application.GetResourceStream(uri).Stream
        let content = XamlReader.Load(stream) :?> FrameworkElement
        this.Content <- content
        
        webView <- content.FindName("webView") :?> WebView2
        this.Loaded.Add(fun _ -> this.MainWindow_Loaded())

    member private this.MainWindow_Loaded() =
        async {
            do! webView.EnsureCoreWebView2Async() |> Async.AwaitTask
            
            webView.CoreWebView2.AddHostObjectToScript("windowApi", WindowApi(this))

            webView.CoreWebView2.DocumentTitleChanged.Add(fun _ ->
                this.Title <- webView.CoreWebView2.DocumentTitle)

            webView.CoreWebView2.ContainsFullScreenElementChanged.Add(fun _ ->
                if webView.CoreWebView2.ContainsFullScreenElement then
                    this.WindowStyle <- WindowStyle.None
                    this.WindowState <- WindowState.Maximized
                    this.Topmost <- true
                else
                    this.WindowStyle <- WindowStyle.None
                    this.WindowState <- WindowState.Normal
                    this.Topmost <- false)

            webView.CoreWebView2.NavigationCompleted.Add(fun _ -> this.OnNavigationCompleted())

            this.StateChanged.Add(fun _ ->
                if this.WindowState = WindowState.Maximized then
                    let area = SystemParameters.WorkArea
                    this.MaxHeight <- area.Height + 8.0
                    this.MaxWidth <- area.Width + 8.0
                this.DispatchWindowEventAsync("windowStateChanged") |> ignore)

            this.LocationChanged.Add(fun _ ->
                this.DispatchWindowEventAsync("windowMoved") |> ignore)

            webView.CoreWebView2.WebMessageReceived.Add(fun args ->
                let msg = args.TryGetWebMessageAsString()
                this.Dispatcher.Invoke(fun () ->
                    match msg with
                    | "dragMove" -> if this.WindowState = WindowState.Normal then this.DragMove()
                    | "minimize" -> this.WindowState <- WindowState.Minimized
                    | "maximize" -> 
                        if this.WindowState = WindowState.Maximized then this.WindowState <- WindowState.Normal
                        else this.WindowState <- WindowState.Maximized
                    | "close" -> this.Close()
                    | _ -> ()
                )
            )

            let sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "source", "index.html")
            webView.Source <- Uri(sourcePath)
        } |> Async.StartImmediate

    member private this.OnNavigationCompleted() =
        async {
            try
                let! faviconUri = this.GetBestFaviconUriAsync() |> Async.AwaitTask
                if not (isNull faviconUri) then
                    let bitmap = TitlebarIconHelper.LoadBitmapFromUri(faviconUri)
                    if not (isNull bitmap) then
                        TitlebarIconHelper.SetTitlebarIcon this bitmap
                else
                    let! stream = webView.CoreWebView2.GetFaviconAsync(CoreWebView2FaviconImageFormat.Png) |> Async.AwaitTask
                    if not (isNull stream) then
                        let! bitmap = TitlebarIconHelper.LoadBitmapFromStreamAsync(stream) |> Async.AwaitTask
                        if not (isNull bitmap) then
                            TitlebarIconHelper.SetTitlebarIcon this bitmap
            with _ -> ()
        } |> Async.StartImmediate

    member private _.GetBestFaviconUriAsync() : Task<Uri> =
        async {
            let script = """
                (() => {
                    const links = Array.from(document.querySelectorAll('link[rel~="icon"]'));
                    return links.map(l => ({ href: l.href, sizes: l.sizes?.value ?? '' }));
                })();
                """
            try
                let! (result: string) = webView.CoreWebView2.ExecuteScriptAsync(script) |> Async.AwaitTask
                let icons = JsonSerializer.Deserialize<List<FaviconInfo>>(result)
                if isNull icons || icons.Count = 0 then return null
                else
                    let best = 
                        icons 
                        |> Seq.sortByDescending(fun i -> 
                            let parts = i.sizes.Split('x')
                            if parts.Length = 2 then 
                                match Int32.TryParse(parts.[0]) with
                                | (true, n) -> n
                                | _ -> 0
                            else 0)
                        |> Seq.head
                    return Uri(best.href)
            with _ -> return null
        } |> Async.StartAsTask

    member private this.DispatchWindowEventAsync(eventName: string) : Task =
        (async {
            if not (isNull webView) && not (isNull webView.CoreWebView2) then
                let info = WindowApi.GetWindowInfo(this)
                let script = sprintf "window.dispatchEvent(new CustomEvent('%s', { detail: %s }))" eventName info
                let! _ = webView.CoreWebView2.ExecuteScriptAsync(script) |> Async.AwaitTask
                ()
        } |> Async.StartAsTask) :> Task
