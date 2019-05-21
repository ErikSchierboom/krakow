module App

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Krakow.Core.Evaluation

type Model = 
    { Equation: string
      Result: Result<int, string> option }

type Message =
    | ChangeValue of string
    | Evaluate

let init() = 
    { Equation = ""
      Result = None }

let update message model =
    match message with
    | ChangeValue newValue ->
        { model with Equation = newValue }
    | Evaluate ->
        { model with Result = Some (evaluate model.Equation) }

let viewResult result =
    match result with
    | Some (Result.Ok solution) -> [ str (string solution) ] 
    | Some (Result.Error error) -> [ str error ] 
    | None -> [] 

let view model dispatch =

  div []
      [ form []
            [
                input [ Class "input"
                        Value model.Equation
                        OnChange (fun ev -> 
                            let target = ev.target :?> Browser.Types.HTMLInputElement
                            target.value |> string |> ChangeValue |> dispatch)
                      ]
                button [ OnClick (fun ev ->
                    dispatch Evaluate
                    ev.preventDefault()) ] [ str "Evaluate" ]
                div [] (viewResult model.Result)
            ]
      ]

// TODO: replace react with plain JS code

Program.mkSimple init update view
|> Program.withReactBatched "elmish-app"
|> Program.withConsoleTrace
|> Program.run