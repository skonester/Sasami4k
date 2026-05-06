namespace Sasami4k.InfiniFrame

open System
open InfiniFrame

module Program =
    [<EntryPoint>]
    [<STAThread>]
    let main _ =
        try
            // Bootstrap is for single-file published apps; we skip it if it fails in dev mode
            try InfiniFrameSingleFileBootstrap.Initialize() with _ -> ()
            
            let window = MainWindow.create()
            window.WaitForClose()
            Environment.Exit(0)
            0
        with ex ->
            printfn "FATAL ERROR: %s" ex.Message
            1
