module Krakow.Website.View

open Fable.React
open Fable.React.Props

open Krakow.Website.Model

let private byteToHex (b: int) = b.ToString("X2")

let private inputForm model dispatch =
    div [ Class "col-sm-4" ]
        [ form [ OnSubmit(fun ev -> ev.preventDefault()) ]
              [ fieldset []
                    [ legend [] [ str "Reverse Polish Notation" ]
                      label [ HtmlFor "equation" ] [ str "Equation" ]
                      input
                          [ Placeholder "Enter equation..."
                            Value model.equation
                            OnChange(fun ev -> dispatch (UpdateEquation ev.Value)) ]
                      button [ OnClick(fun _ -> dispatch EvaluateEquation) ] [ str "Evaluate" ] ] ] ]

let evaluationResult model map =
    match model.evaluation with
    | Some(Ok evaluation) -> map evaluation
    | Some(Error error) ->
        match error with
        | WebAssemblyException ex -> sprintf "WebAssembly error: %s" ex.Message
        | EmptyEquation -> "Empty equation"
        | InvalidEquation token -> sprintf "Invalid token: %s" token
        | UnbalancedEquation -> "Equation is unbalanced"
    | None -> ""

let viewOutputHeader name = h3 [ Class "doc" ] [ str name ]
let viewOutputDetails model map = p [ Class "doc" ] [ str (evaluationResult model map) ]

let output model =
    div [ Class "col-sm-8" ]
        [ viewOutputHeader "Result"
          viewOutputDetails model (fun evaluation -> string evaluation.result)
          viewOutputHeader "WAT"
          viewOutputDetails model (fun evaluation -> evaluation.wat)
          viewOutputHeader "WASM"
          viewOutputDetails model (fun evaluation ->
              evaluation.wasm
              |> List.map byteToHex
              |> String.concat " ") ]

let view model dispatch =
    div [ Class "container" ]
        [ div [ Class "row" ]
              [ inputForm model dispatch
                output model ] ]
