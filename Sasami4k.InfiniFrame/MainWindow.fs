namespace Sasami4k.InfiniFrame

open System
open System.IO
open System.Text.Json
open System.Drawing
open InfiniFrame

module MainWindow =
    let create () =
        let baseDir = AppDomain.CurrentDomain.BaseDirectory
        let iconPath = Path.Combine(baseDir, "resources", "winview2.ico")
        let indexPath = Path.Combine(baseDir, "source", "index.html")
        let mutable isDarkMode = false

        let getWindowInfo (window: IInfiniFrameWindow) =
            try
                dict [
                    "x", box window.Left
                    "y", box window.Top
                    "width", box window.Width
                    "height", box window.Height
                    "isMaximized", box window.Maximized
                    "isMinimized", box window.Minimized
                    "isFocused", box window.Focused
                    "isDarkMode", box isDarkMode
                ] |> JsonSerializer.Serialize
            with _ -> "{}"

        let dispatchWindowEvent (window: IInfiniFrameWindow) eventName =
            try
                let info = getWindowInfo window
                let payload = dict [ "type", box eventName; "detail", box info ]
                window.SendWebMessage(JsonSerializer.Serialize(payload)) |> ignore
            with _ -> ()

        let builder = 
            InfiniFrameWindowBuilder.Create()
                .SetTitle("Sasami4K")
                .SetUseOsDefaultSize(false)
                .SetSize(1280, 720)
                .Center()
                .SetResizable(true)
                .SetDevToolsEnabled(true)
                .SetFileSystemAccessEnabled(true)
                .SetWebSecurityEnabled(false)

        if File.Exists(iconPath) then
            builder.SetIconFile(iconPath) |> ignore

        // Use a proper file URI for the local index.html
        let indexUri = 
            if File.Exists(indexPath) then 
                "file:///" + indexPath.Replace("\\", "/").Replace(" ", "%20")
            else 
                "about:blank"
        
        builder.SetStartUrl(indexUri) |> ignore

        // Build the window
        let window = builder.Build()

        // Register handlers AFTER building to ensure the native handle is valid
        window.RegisterWebMessageReceivedHandler(fun window message ->
            try
                let data = JsonDocument.Parse(message)
                let action = data.RootElement.GetProperty("action").GetString()
                
                match action with
                | "close" -> window.Close()
                | "minimize" -> 
                    window.SetMinimized(true) |> ignore
                    dispatchWindowEvent window "windowInfoChanged"
                | "maximize" -> 
                    window.SetMaximized(not window.Maximized) |> ignore
                    dispatchWindowEvent window "windowInfoChanged"
                | "restore" -> 
                    window.SetMaximized(false) |> ignore
                    window.SetMinimized(false) |> ignore
                    dispatchWindowEvent window "windowInfoChanged"
                | "dragMove" -> () 
                | "setSize" ->
                    let w = data.RootElement.GetProperty("width").GetInt32()
                    let h = data.RootElement.GetProperty("height").GetInt32()
                    window.SetSize(w, h) |> ignore
                    dispatchWindowEvent window "windowInfoChanged"
                | "move" ->
                    let x = data.RootElement.GetProperty("x").GetInt32()
                    let y = data.RootElement.GetProperty("y").GetInt32()
                    window.SetLocation(Point(x, y)) |> ignore
                    dispatchWindowEvent window "windowInfoChanged"
                | "setAlwaysOnTop" ->
                    window.SetTopMost(data.RootElement.GetProperty("top").GetBoolean()) |> ignore
                    dispatchWindowEvent window "windowInfoChanged"
                | "setDarkMode" ->
                    isDarkMode <- data.RootElement.GetProperty("dark").GetBoolean()
                    dispatchWindowEvent window "windowInfoChanged"
                | "getWindowInfo" ->
                    let info = getWindowInfo window
                    let payload = dict [ "type", "windowInfoResult"; "detail", info ]
                    window.SendWebMessage(JsonSerializer.Serialize(payload)) |> ignore
                | _ -> ()
            with _ -> ()
        ) |> ignore

        window.RegisterSizeChangedHandler(fun window _ -> dispatchWindowEvent window "windowInfoChanged") |> ignore
        window.RegisterLocationChangedHandler(fun window _ -> dispatchWindowEvent window "windowInfoChanged") |> ignore
        
        window
