namespace Avalonia.FuncUI.DSL

[<AutoOpen>]
module Thumb =
    open Avalonia.Input
    open Avalonia.Controls.Primitives
    open Avalonia.FuncUI.Types
    open Avalonia.FuncUI.Builder
    
    let create (attrs: IAttr<Thumb> list): IView<Thumb> =
        ViewBuilder.Create<Thumb>(attrs)
     
    type Thumb with

        static member onDragStarted<'t when 't :> Thumb>(func: VectorEventArgs -> unit) : IAttr<'t> =
            AttrBuilder<'t>.CreateSubscription<VectorEventArgs>(Thumb.DragStartedEvent, func)
            
        static member onDragDelta<'t when 't :> Thumb>(func: VectorEventArgs -> unit) : IAttr<'t> =
            AttrBuilder<'t>.CreateSubscription<VectorEventArgs>(Thumb.DragDeltaEvent, func)
            
        static member onDragCompleted<'t when 't :> Thumb>(func: VectorEventArgs -> unit) : IAttr<'t> =
            AttrBuilder<'t>.CreateSubscription<VectorEventArgs>(Thumb.DragCompletedEvent, func)