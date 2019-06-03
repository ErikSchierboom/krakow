module Krakow.Website.App

open Elmish

open Browser.Dom

open Fable.Import
open Fable.Core.JsInterop

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

type Context =
    { formElement:     Browser.Types.HTMLFormElement
      equationElement: Browser.Types.HTMLInputElement
      resultElement:   Browser.Types.HTMLDivElement
      watElement:      Browser.Types.HTMLDivElement
      wasmElement:     Browser.Types.HTMLDivElement }

type Model = 
    { evaluation: Result<Evaluation, EvaluationError> option
      context: Context }

type Msg =
    | Evaluate of string
    | Evaluated of Evaluation
    | EvaluationError of EvaluationError

let parseSuccess model equation =
    let wat = equationToWebAssemblyText equation
    let wasm = equationToWebAssemblyBinary equation
    let wasmByteArray = Uint8Array.from wasm

    let onSuccess wa = Evaluated ({ result = wa.instance.exports?evaluate(); wasm = wasm; wat = wat })
    let onError _ = EvaluationError InvalidWebAssembly
    let cmd = Cmd.OfPromise.either WebAssembly.instantiate wasmByteArray onSuccess onError

    model, cmd

let parseError model =
    model, Cmd.ofMsg (EvaluationError InvalidEquation)

let update msg model = 
    match msg with
    | Evaluate input ->
        parse input
        |> Option.map (parseSuccess model)
        |> Option.defaultValue (parseError model)
    | Evaluated evaluation ->
        { model with evaluation = Some (Ok evaluation) }, Cmd.none
    | EvaluationError error ->
        { model with evaluation = Some (Error error) }, Cmd.none

let init () = 
    { evaluation = None
      context = 
          { formElement     = document.getElementById("form")   :?> Browser.Types.HTMLFormElement
            equationElement = document.getElementById("input")  :?> Browser.Types.HTMLInputElement
            resultElement   = document.getElementById("output") :?> Browser.Types.HTMLDivElement
            watElement      = document.getElementById("text")   :?> Browser.Types.HTMLDivElement
            wasmElement     = document.getElementById("binary") :?> Browser.Types.HTMLDivElement } }, Cmd.none

let viewSuccess model (evaluation: Evaluation) =
    let byte2hex (byte: int) = byte.ToString("X2")

    model.context.resultElement.innerText <- string evaluation.result
    model.context.watElement.innerText <- evaluation.wat
    model.context.wasmElement.innerText <- evaluation.wasm |> List.map byte2hex |> String.concat " "

let viewError model error =
    let errorText =
        match error with
        | InvalidEquation -> "Invalid equation"
        | InvalidWebAssembly -> "Invalid WebAssembly binary"

    model.context.resultElement.innerText <- errorText
    model.context.watElement.innerText <- ""
    model.context.wasmElement.innerText <- ""

let viewResult model result =
    result
    |> Result.map (viewSuccess model)
    |> Result.mapError (viewError model)

let view model dispatch =
    model.context.formElement.onsubmit <- fun event ->
        event.preventDefault()
        dispatch (Evaluate model.context.equationElement.value)

    Option.map (viewResult model) model.evaluation

Program.mkProgram init update view
|> Program.run

// TODO: make styling prettier