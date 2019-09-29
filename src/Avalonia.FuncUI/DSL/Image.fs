namespace Avalonia.FuncUI.DSL
open Avalonia
open Avalonia.Controls.Templates

[<AutoOpen>]
module Image =
    open System
    open System.Threading
    open System.Collections
    
    open FSharp.Data.UnitSystems.SI.UnitNames
    
    open Avalonia.Animation
    open Avalonia.Controls
    open Avalonia.FuncUI.Core.Domain
    open Avalonia.Interactivity
    open Avalonia.Styling
    open Avalonia.Media
    open Avalonia.Media.Imaging
    
    let create (attrs: IAttr<Image> list): IView<Image> =
        View.create<Image>(attrs)
    
    type Image with
        static member source<'t when 't :> Image>(value: #IBitmap) : IAttr<'t> =
            let accessor = Accessor.AvaloniaProperty Image.SourceProperty
            let property = Property.createDirect(accessor, value)
            let attr = Attr.createProperty<'t> property
            attr :> IAttr<'t>
            
        static member stretch<'t when 't :> Image>(value: Stretch) : IAttr<'t> =
            let accessor = Accessor.AvaloniaProperty Image.StretchProperty
            let property = Property.createDirect(accessor, value)
            let attr = Attr.createProperty<'t> property
            attr :> IAttr<'t>