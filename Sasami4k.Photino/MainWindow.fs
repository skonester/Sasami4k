namespace WinView2

open System
open System.IO
open System.Text.Json
open System.Drawing
open PhotinoNET

type MainWindow() as this =
    let window = new PhotinoWindow()
    let mutable isDarkMode = false

    let getWindowInfo () =
        let info = 
            dict [
                "x", box window.Left
                "y", box window.Top
                "width", box window.Width
                "height", box window.Height
                "isMaximized", box window.Maximized
                "isMinimized", box window.Minimized
                "isFocused", box true
                "isDarkMode", box isDarkMode
            ]
        JsonSerializer.Serialize(info)

    let dispatchWindowEvent eventName =
        let info = getWindowInfo()
        let payload = dict [ "type", box eventName; "detail", box info ]
        try
            window.SendWebMessage(JsonSerializer.Serialize(payload)) |> ignore
        with _ -> ()

    do
        let baseDir = AppDomain.CurrentDomain.BaseDirectory
        let iconPath = Path.Combine(baseDir, "resources", "winview2.ico")
        let indexPath = Path.Combine(baseDir, "source", "index.html")

        window
            .SetTitle("Sasami4K")
            .SetUseOsDefaultSize(false)
            .SetSize(800, 600)
            .SetResizable(true)
            .SetDevToolsEnabled(true) // ENABLE INSPECTING TO DEBUG
            |> ignore

        if File.Exists(iconPath) then
            window.SetIconFile(iconPath) |> ignore

        // Register Message Handlers
        window.RegisterWebMessageReceivedHandler(fun _ message ->
            try
                let data = JsonDocument.Parse(message)
                let action = data.RootElement.GetProperty("action").GetString()
                
                match action with
                | "close" -> window.Close()
                | "minimize" -> window.SetMinimized(true) |> ignore
                | "maximize" -> window.SetMaximized(not window.Maximized) |> ignore
                | "restore" -> 
                    window.SetMaximized(false) |> ignore
                    window.SetMinimized(false) |> ignore
                | "setSize" ->
                    let w = data.RootElement.GetProperty("width").GetInt32()
                    let h = data.RootElement.GetProperty("height").GetInt32()
                    window.SetSize(w, h) |> ignore
                | "move" ->
                    let x = data.RootElement.GetProperty("x").GetInt32()
                    let y = data.RootElement.GetProperty("y").GetInt32()
                    window.SetLocation(Point(x, y)) |> ignore
                | "setAlwaysOnTop" ->
                    let top = data.RootElement.GetProperty("top").GetBoolean()
                    window.SetTopMost(top) |> ignore
                | "setDarkMode" ->
                    isDarkMode <- data.RootElement.GetProperty("dark").GetBoolean()
                    dispatchWindowEvent "windowInfoChanged"
                | "getWindowInfo" ->
                    let info = getWindowInfo()
                    let payload = dict [ "type", "windowInfoResult"; "detail", info ]
                    window.SendWebMessage(JsonSerializer.Serialize(payload)) |> ignore
                | _ -> ()
            with _ -> ()
        ) |> ignore

        // Load content
        if File.Exists(indexPath) then
            // Use file:// URI for better local resource resolution
            let uri = Uri(indexPath)
            window.Load(uri) |> ignore
        else
            printfn "Error: index.html not found at %s" indexPath

    member _.Show() = 
        window.WaitForClose()
