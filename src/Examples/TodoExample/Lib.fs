namespace TodoExample

[<AutoOpen>]
module AvaloniaExtensions =
    open Avalonia.Platform
    open System.Reflection
    open Avalonia
    open Avalonia.Media.Imaging
    open Avalonia.Markup.Xaml.Styling
    open System
    open Avalonia.Styling

    type Styles with
        member this.Load (source: string) = 
            let style = new StyleInclude(baseUri = null)
            style.Source <- new Uri(source)
            this.Add(style)

    type Bitmap with
        static member LoadFrom (path: string) =
            let avares (assembly: string) (source: string) =
                sprintf "avares://%s/%s" (assembly) source
                
            let assets = AvaloniaLocator.Current.GetService<IAssetLoader>()

            let assembly = Assembly.GetCallingAssembly()
            let source = avares (assembly.GetName().Name) path

            new Bitmap(assets.Open(Uri(source, UriKind.RelativeOrAbsolute)))

module Icons =
    let edit = "Assets/Icons/icon-edit.png"
    let delete = "Assets/Icons/icon-delete.png"