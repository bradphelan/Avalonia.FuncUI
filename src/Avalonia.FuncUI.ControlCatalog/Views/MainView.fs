﻿namespace Avalonia.FuncUI.ControlCatalog.Views

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout

module MainView =
    
    type CounterState = {
        count : int
    }

    let init = {
        count = 0
    }

    type Msg =
    | Increment
    | Decrement

    let update (msg: Msg) (state: CounterState) : CounterState =
        match msg with
        | Increment -> { state with count = state.count + 1 }
        | Decrement -> { state with count = state.count - 1 }
    
    let view (state: CounterState) (dispatch) =
        DockPanel.create [
            DockPanel.children [
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun args -> dispatch Decrement)
                    Button.content "-"
                ]
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClickRouted (fun args -> dispatch Increment)
                    Button.content "+"
                ]
                TextBox.create [
                    TextBox.dock Dock.Bottom
                    TextBox.text (sprintf "%i" state.count)
                    TextBox.onTextChanged (fun text ->
                        printfn "new Text: %s" text
                     )
                ]                
                TextBlock.create [
                    TextBlock.dock Dock.Top
                    TextBlock.fontSize 48.0
                    TextBlock.verticalAlignment VerticalAlignment.Center
                    TextBlock.horizontalAlignment HorizontalAlignment.Center
                    TextBlock.text (string state.count)
                ]
            ]
        ]       