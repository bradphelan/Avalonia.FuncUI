namespace TodoExample

open Avalonia.FuncUI.Hosts
open Avalonia
open Avalonia.FuncUI.Elmish
open Elmish

type MainWindow() =
    inherit HostWindow()

    do
        base.Title <- "TODO Example"
        base.Width <- 300.0
        base.Height <- 400.0
        ()

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Load "resm:Avalonia.Themes.Default.DefaultTheme.xaml?assembly=Avalonia.Themes.Default"
        this.Styles.Load "resm:Avalonia.Themes.Default.Accents.BaseDark.xaml?assembly=Avalonia.Themes.Default"
        this.Styles.Load "avares://TodoExample/Assets/Styles.xaml"

module Program =
    open Avalonia
    open Avalonia.Logging.Serilog

    [<CompiledName "BuildAvaloniaApp">]
    let buildAvaloniaApp() =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .LogToDebug()

    [<CompiledName "AppMain">]
    let appMain (app: Application) (args: string[]) =
        let mainWindow = MainWindow()

        let init arg = 
            AppView.State.emptyState, Cmd.Empty

        Elmish.Program.mkProgram init AppView.update AppView.view
        |> Program.withHost mainWindow
        |> Program.withConsoleTrace
        |> Program.run

        app.Run(mainWindow)
        |> ignore

    [<EntryPoint>]
    [<CompiledName "Main">]
    let main(args: string[]) =
        buildAvaloniaApp().Start(appMain, args)
        0
