module Krakow.Website.View

open Fable.React
open Fable.React.Props

open Krakow.Website.Types
open Krakow.Website.Helpers

let private row children = div [ Class "row" ] [ div [ Class "col-sm-offset-3 col-sm-6" ] children ]

let private rowWithHeader text content =
    let header = h2 [] [ str text ]
    row [ header; content ]

let private noContent = row []

let private equationInput equation dispatch =
    let content =
        form [ OnSubmit(fun ev -> ev.preventDefault()) ]
            [ label [ HtmlFor "equation" ] [ str "Equation" ]
              input
                  [ Placeholder "Enter equation..."
                    Name "equation"
                    Value equation
                    OnChange(fun ev -> dispatch (UpdateEquation ev.Value)) ]
              button [ OnClick(fun _ -> dispatch EvaluateEquation) ] [ str "Evaluate" ] ]

    rowWithHeader "Equation input" content

let private evaluationErrorText error =
    match error with
    | WebAssemblyException ex -> sprintf "WebAssembly error: %s" ex.Message
    | EmptyEquation -> "Empty equation"
    | InvalidEquation token -> sprintf "Invalid token: %s" token
    | UnbalancedEquation -> "Equation is unbalanced"

let private equationOutputContent resultClass text =
    p []
        [ mark
            [ Name "result"
              Class resultClass ] [ str text ] ]

let private equationOutput evaluation =
    let content =
        match evaluation with
        | Ok evaluation -> equationOutputContent "tertiary" (string evaluation.result)
        | Error error -> equationOutputContent "secondary" (evaluationErrorText error)

    rowWithHeader "Equation output" content

let private webAssemblyText evaluation =
    match evaluation with
    | Ok evaluation ->
        let content = pre [] [ str evaluation.wat ]

        rowWithHeader "WebAssembly Text" content
    | _ -> noContent

let private webAssemblyBinary evaluation =
    match evaluation with
    | Ok evaluation ->
        let bytes =
            evaluation.wasm
            |> List.map (fun b -> b.ToString("X2"))
            |> String.concat " "

        let content = pre [] [ str bytes ]

        rowWithHeader "WebAssembly Binary" content
    | _ -> noContent

let rows model dispatch =
    match model.evaluation with
    | Some evaluation ->
        [ equationInput model.equation dispatch
          equationOutput evaluation
          webAssemblyText evaluation
          webAssemblyBinary evaluation ]
    | None -> [ equationInput model.equation dispatch ]

let view model dispatch = div [ Class "container" ] (rows model dispatch)
