module Krakow.Website.App

open Elmish
open Elmish.React

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

open Krakow.Core.Parser
open Krakow.Core.WebAssembly.Text
open Krakow.Core.WebAssembly.Binary
open Krakow.Website.Interop

let byteToHex (b: int) = b.ToString("X2")

type Evaluation =
    { result: int
      wasm: int list
      wat: string }

type EvaluationError =
    | InvalidWebAssembly
    | InvalidEquation

type Model =
    { evaluation: Result<Evaluation, EvaluationError> option
      equation: string }

type Msg =
    | UpdateEquation of string
    | EvaluateEquation
    | EquationEvaluatedSuccessfully of Evaluation
    | EquationEvaluatedWithError of EvaluationError

let init() =
    { evaluation = None
      equation = "" }, Cmd.none

let parseSuccess (model: Model) equation =
    let wat = equationToWebAssemblyText equation
    let wasm = equationToWebAssemblyBinary equation
    let wasmByteArray = Uint8Array.from wasm

    let onSuccess wa =
        EquationEvaluatedSuccessfully
            ({ result = wa.instance.exports?evaluate ()
               wasm = wasm
               wat = wat })

    let onError _ = EquationEvaluatedWithError InvalidWebAssembly
    let cmd = Cmd.OfPromise.either WebAssembly.instantiate wasmByteArray onSuccess onError

    model, cmd

let parseError (model: Model) = model, Cmd.ofMsg (EquationEvaluatedWithError InvalidEquation)

let update msg model =
    match msg with
    | UpdateEquation equation -> { model with equation = equation }, Cmd.none
    | EvaluateEquation ->
        parse model.equation
        |> Option.map (parseSuccess model)
        |> Option.defaultValue (parseError model)
    | EquationEvaluatedSuccessfully evaluation -> { model with evaluation = Some(Ok evaluation) }, Cmd.none
    | EquationEvaluatedWithError error -> { model with evaluation = Some(Error error) }, Cmd.none

let view (model: Model) dispatch =
    let viewInputForm =
        form [ OnSubmit(fun ev -> ev.preventDefault()) ]
            [ fieldset []
                  [ legend [] [ str "Reverse Polish Notation" ]
                    label [ HtmlFor "equation" ] [ str "Equation" ]
                    input
                        [ Placeholder "Enter equation..."
                          Value model.equation
                          OnChange(fun ev -> dispatch (UpdateEquation ev.Value)) ]
                    button [ OnClick(fun ev -> dispatch EvaluateEquation) ] [ str "Evaluate" ] ] ]

    let viewInput = div [ Class "col-sm-4" ] [ viewInputForm ]

    let viewEvaluationResult map: string =
        match model.evaluation with
        | Some(Ok evaluation) -> map evaluation
        | Some(Error error) ->
            match error with
            | InvalidEquation -> "Invalid equation"
            | InvalidWebAssembly -> "Invalid WebAssembly binary"
        | None -> ""

    let viewOutputHeader name = h3 [ Class "doc" ] [ str name ]
    let viewOutputDetails map = p [ Class "doc" ] [ str (viewEvaluationResult map) ]

    let viewOutput =
        div [ Class "col-sm-8" ]
            [ viewOutputHeader "Result"
              viewOutputDetails (fun evaluation -> string evaluation.result)
              viewOutputHeader "WAT"
              viewOutputDetails (fun evaluation -> evaluation.wat)
              viewOutputHeader "WASM"
              viewOutputDetails (fun evaluation ->
                  evaluation.wasm
                  |> List.map byteToHex
                  |> String.concat " ") ]

    div [ Class "container" ] [ div [ Class "row" ] [ viewInput; viewOutput ] ]

Program.mkProgram init update view
|> Program.withReactSynchronous "app"
|> Program.withConsoleTrace
|> Program.run
