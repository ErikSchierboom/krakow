module Krakow.Website.App

open Elmish
open Elmish.React

open Fable.React
open Fable.React.Props
open Fable.Core.JsInterop

open Krakow.Core.Domain
open Krakow.Core.WebAssembly

open Krakow.Website.Interop

type Evaluation =
    { result: int
      wasm: int list
      wat: string }

type EvaluationError =
    | WebAssemblyException of exn
    | EmptyEquation
    | InvalidEquation of string
    | UnbalancedEquation

type Model =
    { evaluation: Result<Evaluation, EvaluationError> option
      equation: string }

type Msg =
    | UpdateEquation of string
    | EvaluateEquation
    | EquationEvaluatedSuccessfully of Evaluation
    | EquationEvaluatedWithError of EvaluationError

let private equationEvaluatedSuccessfully model webAssembly =
    let (WebAssemblyText(wat)) = webAssembly.Text
    let (WebAssemblyBinary(wasm)) = webAssembly.Binary

    let onSuccess webAssemblyInBrowser =
        EquationEvaluatedSuccessfully
            ({ result = webAssemblyInBrowser.instance.exports?evaluate ()
               wasm = wasm
               wat = wat })

    let onError ex = EquationEvaluatedWithError(WebAssemblyException ex)

    let cmd = Cmd.OfPromise.either WebAssembly.instantiate (Uint8Array.from wasm) onSuccess onError
    model, cmd

let private equationEvaluatedWithError model error =
    let evaluationError =
        match error with
        | Empty -> EmptyEquation
        | Invalid token -> InvalidEquation token
        | Unbalanced -> UnbalancedEquation

    model, Cmd.ofMsg (EquationEvaluatedWithError evaluationError)

let private evaluate model =
    match convert model.equation with
    | Ok webAssembly -> equationEvaluatedSuccessfully model webAssembly
    | Error error -> equationEvaluatedWithError model error

let init() =
    { evaluation = None
      equation = "" }, Cmd.none

let update msg model =
    match msg with
    | UpdateEquation newEquation -> { model with equation = newEquation }, Cmd.none
    | EvaluateEquation -> evaluate model
    | EquationEvaluatedSuccessfully evaluation -> { model with evaluation = Some(Ok evaluation) }, Cmd.none
    | EquationEvaluatedWithError error -> { model with evaluation = Some(Error error) }, Cmd.none

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

Program.mkProgram init update view
|> Program.withReactSynchronous "app"
|> Program.withConsoleTrace
|> Program.run
