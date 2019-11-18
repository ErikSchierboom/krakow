module Krakow.Website.View

open Fable.React
open Fable.React.Props

open Krakow.Website.Types

let private byteToHex (b: int) = b.ToString("X2")

let private evaluationErrorText error =
    match error with
    | WebAssemblyException ex -> sprintf "WebAssembly error: %s" ex.Message
    | EmptyEquation -> "Empty equation"
    | InvalidEquation token -> sprintf "Invalid token: %s" token
    | UnbalancedEquation -> "Equation is unbalanced"

let private evaluationError model =
    let displayOption =
        match model.evaluation with
        | Some(Error _) -> DisplayOptions.Block
        | _ -> DisplayOptions.None

    let text =
        match model.evaluation with
        | Some(Error error) -> evaluationErrorText error
        | _ -> ""

    mark
        [ Class "secondary"
          Style [ Display displayOption ] ] [ str text ]

let private equationForm model dispatch =
    div [ Class "col-sm-4" ]
        [ form [ OnSubmit(fun ev -> ev.preventDefault()) ]
              [ fieldset []
                    [ legend [] [ str "Reverse Polish Notation" ]
                      label [ HtmlFor "equation" ] [ str "Equation" ]
                      input
                          [ Placeholder "Enter equation..."
                            Value model.equation
                            OnChange(fun ev -> dispatch (UpdateEquation ev.Value)) ]
                      button [ OnClick(fun _ -> dispatch EvaluateEquation) ] [ str "Evaluate" ]
                      evaluationError model ] ] ]

let private outputSection header text =
    [ h3 [ Class "doc" ] [ str header ]
      p [ Class "doc" ] [ str text ] ]

let private resultSection evaluation =
    let text = string evaluation.result
    outputSection "Result" text

let private watSection evaluation = outputSection "WAT" evaluation.wat

let private wasmSection evaluation =
    let text =
        evaluation.wasm
        |> List.map byteToHex
        |> String.concat " "
    outputSection "WASM" text

let private resultSections evaluation = resultSection evaluation @ watSection evaluation @ wasmSection evaluation

let private results model =
    match model.evaluation with
    | Some(Ok evaluation) -> div [ Class "col-sm-8" ] (resultSections evaluation)
    | _ -> div [] []

let view model dispatch =
    div [ Class "container" ]
        [ div [ Class "row" ]
              [ equationForm model dispatch
                results model ] ]
