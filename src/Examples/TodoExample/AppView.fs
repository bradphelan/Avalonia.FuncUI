namespace TodoExample

open Elmish
open Avalonia.Controls
open Avalonia.Media
open Avalonia.FuncUI.Types
open Avalonia.FuncUI
open Avalonia.Layout
open System
open Avalonia.Input
open Avalonia.Media.Imaging

module AppView =

    type Filter = All | Completed | Active

    type Entry = {
        id : Guid
        description : string
        complete : bool
        editing : bool
    }

    module Entry =
        let create (description: string) =
            {
                id = Guid.NewGuid()
                description = description
                complete = false
                editing = false
            }

    type State = {
        entries : Entry list
        field : string
        filter : Filter
    }

    module State = 
        let emptyState = {
            entries = []
            field = ""
            filter = All
        }

    type Msg =
    | UpdateField of string
    | EditingEntry of Guid
    | UpdateEntry of Guid * string
    | Add
    | Delete of Guid
    | DeleteComplete
    | Check of Guid * bool
    | CheckAll of bool
    | ChangeFilter of Filter

    let update (msg: Msg) (state: State) : State * Cmd<Msg> =
        match msg with
        | Add ->
            {
                state with
                    field = ""
                    entries = 
                        if String.IsNullOrWhiteSpace state.field then
                            state.entries
                        else
                            (Entry.create state.field) :: state.entries    
                            
            }, Cmd.Empty

        | UpdateField str ->
            { state with field = str }, Cmd.Empty

        | EditingEntry id ->
            let entries =
                state.entries
                |> List.map (fun entry ->
                    { entry with editing = (entry.id = id) }
                )

            { state with entries = entries }, Cmd.Empty
                
        | UpdateEntry (id, description) ->
            let entries =
                state.entries
                |> List.map (fun entry ->
                    match entry.id = id with
                    | true -> { entry with description = description }
                    | false -> entry
                )

            { state with entries = entries }, Cmd.Empty

        | Delete id ->
            {
                state with
                    entries = state.entries |> List.filter (fun entry -> entry.id <> id) 
            }, Cmd.Empty

        | DeleteComplete ->
            {
                state with
                    entries = state.entries |> List.filter (fun entry -> entry.complete <> true)
            }, Cmd.Empty

        | Check (id, isCompleted) ->
            let entries =
                state.entries
                |> List.map (fun entry ->
                    match entry.id = id with
                    | true -> { entry with complete = isCompleted }
                    | false -> entry
                )

            { state with entries = entries }, Cmd.Empty

        | CheckAll isCompleted ->
            let entries =
                state.entries
                |> List.map (fun entry -> { entry with complete = isCompleted })
                
            { state with entries = entries }, Cmd.Empty

        | ChangeFilter filter ->
            { state with filter = filter }, Cmd.Empty
            

    let iconButton (path: string) (attrs: TypedAttr<Button> list) =
        Views.button <| attrs @ [
            Attrs.content (
                Views.image [
                    Attrs.source (Bitmap.LoadFrom path)
                ]
            )
        ]

    let viewEntry (entry: Entry) (dispatch): View =
        Views.dockpanel [
            Attrs.children [
                yield Views.checkBox [
                    Attrs.isChecked entry.complete
                    Attrs.onClick (fun sender _ -> 
                        dispatch (Check (entry.id, (sender :?> CheckBox).IsChecked.GetValueOrDefault()))
                    )
                ]

                yield iconButton Icons.edit [
                    Attrs.dockPanel_dock Dock.Right
                    Attrs.width 32.0
                    Attrs.height 32.0
                    Attrs.onClick (fun _ __ -> 
                        dispatch (EditingEntry entry.id)
                    )
                    Attrs.isVisible (not entry.editing)
                ]

                yield iconButton Icons.delete [
                    Attrs.dockPanel_dock Dock.Right
                    Attrs.width 32.0
                    Attrs.height 32.0
                    Attrs.onClick (fun _ __ -> 
                        dispatch (Delete entry.id)
                    )
                ]

                match entry.editing with
                | true ->
                    yield Views.textBox [
                        Attrs.dockPanel_dock Dock.Left
                        Attrs.text entry.description
                        Attrs.onKeyDown (fun sender args ->
                            if args.Key = Key.Enter then
                                dispatch (EditingEntry Guid.Empty)
                            else
                                dispatch (UpdateEntry (entry.id, (sender :?> TextBox).Text))
                        )
                        Attrs.onKeyUp (fun sender __ ->
                            dispatch (UpdateEntry (entry.id, (sender :?> TextBox).Text))
                        )
                    ]
                | false ->
                    yield Views.textBlock [
                        Attrs.dockPanel_dock Dock.Left
                        Attrs.text entry.description
                        Attrs.verticalAlignment VerticalAlignment.Center
                    ]
            ]
        ]

    let viewFilter (activeFilter: Filter) (dispatch): View =
        let filterButton (filter: Filter) =
            Views.button [
                yield Attrs.content (
                    match filter with
                    | All -> "All"
                    | Active -> "Active"
                    | Completed -> "Complete"                
                )

                if activeFilter = filter then
                    yield Attrs.foreground "#2ecc71"

                yield Attrs.onClick (fun _ __ -> 
                    dispatch (ChangeFilter filter)
                )
            ]

        Views.stackPanel [
            Attrs.dockPanel_dock Dock.Bottom
            Attrs.orientation Orientation.Horizontal
            Attrs.children [
                filterButton Filter.All
                filterButton Filter.Active
                filterButton Filter.Completed
            ]
        ]

    let view (state: State) (dispatch): View =
        Views.dockpanel [
            Attrs.margin 10.0
            Attrs.children [
                Views.dockpanel [
                    Attrs.dockPanel_dock Dock.Top
                    Attrs.children [

                        Views.checkBox [
                            Attrs.isChecked (
                                state.entries
                                |> List.exists (fun entry -> not entry.complete)
                                |> not
                            ) 
                        ]                           

                        Views.textBox [
                            Attrs.text state.field
                            Attrs.watermark "What needs to be done ?"
                            Attrs.borderBrush Brushes.Transparent
                            Attrs.onKeyDown (fun sender args ->
                                if args.Key = Key.Enter then
                                    dispatch Add
                                else
                                    dispatch (UpdateField (sender :?> TextBox).Text)
                            )
                            Attrs.onKeyUp (fun sender __ ->
                                dispatch (UpdateField (sender :?> TextBox).Text)
                            )
                        ]
                    ]
                ]

                // filter
                viewFilter state.filter dispatch

                // entries
                Views.stackPanel [
                    Attrs.dockPanel_dock Dock.Top
                    Attrs.orientation Orientation.Vertical
                    Attrs.children [
                        for entry in state.entries do
                            yield viewEntry entry dispatch
                    ]
                ]
            ]
        ]       
