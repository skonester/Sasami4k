namespace WinView2

open System

module Program =
    [<EntryPoint>]
    [<STAThread>]
    let main _ =
        let mainWindow = MainWindow()
        mainWindow.Show()
        0
