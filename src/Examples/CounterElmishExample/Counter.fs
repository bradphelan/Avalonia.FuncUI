namespace CounterElmishSample

open Avalonia.Controls
open Avalonia.Media
open Avalonia.FuncUI.Types
open Avalonia.FuncUI
open Avalonia.Layout

type CustomControl() =
    inherit Control()

    member val Text: string = "" with get, set

[<AutoOpen>]
module ViewExt =
    type Views with
        static member customControl (attrs: TypedAttr<CustomControl> list): View =
            Views.create<CustomControl>(attrs)

// see also 
// http://stackoverflow.com/questions/4949941/convert-string-to-system-datetime-in-f

module TryParser =
    // convenient, functional TryParse wrappers returning option<'a>
    let tryParseWith (tryParseFunc: string -> bool * _) = tryParseFunc >> function
        | true, v    -> Some v
        | false, _   -> None

    let parseDate   = tryParseWith System.DateTime.TryParse
    let parseInt    = tryParseWith System.Int32.TryParse
    let parseIntWithDefault d = parseInt >> (Option.defaultValue d) 
    let parseSingle = tryParseWith System.Single.TryParse
    let parseDouble = tryParseWith System.Double.TryParse
    // etc.

    // active patterns for try-parsing strings
    let (|Date|_|)   = parseDate
    let (|Int|_|)    = parseInt
    let (|Single|_|) = parseSingle
    let (|Double|_|) = parseDouble

open TryParser

module Counter =

    type CounterState = {
        count : int
    }

    let init = {
        count = 0
    }

    type Msg =
    | Increment
    | Decrement

    let update msg (state: CounterState) : CounterState =
        msg state

    let increment  (state: CounterState) : CounterState = { state with count =  state.count + 1 }
    let decrement  (state: CounterState) : CounterState = { state with count =  state.count + 1 }
    let set_count  (state:CounterState) count : CounterState = { state with count = count;}


    // Neat method of finding the TryParse method for any type that supports it.
    // See https://stackoverflow.com/a/33161245/158285
    let inline tryParseWithDefault (defaultVal:'a) text : ^a when ^a : (static member TryParse : string * ^a byref -> bool) = 
        let r = ref defaultVal
        if (^a : (static member TryParse: string * ^a byref -> bool) (text, &r.contents)) 
        then !r 
        else defaultVal

    module Binder =
        let inline oneWay defaultValue update dispatch (sender:obj) _ =
            let value = 
                match sender with 
                | :? Avalonia.Controls.TextBox as textBox -> tryParseWithDefault defaultValue (textBox.Text) 
                | _ -> defaultValue
            dispatch (fun state -> update state value ) 

        let inline command update dispatch (sender:obj) _ =
            dispatch (fun state -> update state)

    let view (state: CounterState) (dispatch): View =
        Views.dockpanel [
            Attrs.children [
                Views.button [
                    Attrs.dockPanel_dock Dock.Bottom
                    Attrs.onClick (Binder.command increment dispatch)
                    Attrs.content "-"
                ]
                Views.button [
                    Attrs.dockPanel_dock Dock.Bottom
                    Attrs.onClick (Binder.command decrement dispatch)
                    Attrs.content "+"
                ]
                Views.textBox[
                    Attrs.dockPanel_dock Dock.Top
                    Attrs.fontSize 48.0
                    Attrs.verticalAlignment VerticalAlignment.Center
                    Attrs.horizontalAlignment HorizontalAlignment.Stretch
                    Attrs.text (string state.count)
                    Attrs.onKeyUp (Binder.oneWay 0 set_count dispatch )
                ]
            ]
        ]       
