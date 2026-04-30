namespace WinView2

open System
open System.Windows
open System.Windows.Markup

type App() as this =
    inherit Application()
    
    do
        // Load App.xaml resources using XamlReader
        let uri = Uri("App.xaml", UriKind.Relative)
        let stream = Application.GetResourceStream(uri).Stream
        let resources = XamlReader.Load(stream) :?> ResourceDictionary
        
        // Merge resources
        this.Resources.MergedDictionaries.Add(resources)

module Program =
    [<STAThread>]
    [<EntryPoint>]
    let main _ =
        let app = App()
        let window = MainWindow()
        app.Run(window)
