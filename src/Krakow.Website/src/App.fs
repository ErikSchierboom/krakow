module Krakow.Website.App

open Elmish

open Browser.Dom

open Fable.Import
open Fable.Core.JsInterop

open Krakow.Core.Parser
open Krakow.Core.WebAssembly.Text
open Krakow.Core.WebAssembly.Binary
open Krakow.Website.Interop

let form   = document.getElementById("form")   :?> Browser.Types.HTMLFormElement
let input  = document.getElementById("input")  :?> Browser.Types.HTMLInputElement
let output = document.getElementById("output") :?> Browser.Types.HTMLDivElement
let text   = document.getElementById("text")   :?> Browser.Types.HTMLDivElement
let binary = document.getElementById("binary") :?> Browser.Types.HTMLDivElement

let byteToHex (b: int) = b.ToString("X2")

type EvaluateResult =
    { equation: string
      result: string
      wasm: int list
      wat: string }

type Model =
    { current: EvaluateResult
      history: EvaluateResult list }

type Msg = 
    | Parse of string
    | Error of string
    | Evaluate of Equation
    | Evaluated of EvaluateResult

let update msg model = 
    match msg with
    | Parse input ->
        match parse input with
        | Some equation ->
            model, Cmd.ofMsg (Evaluate equation)
        | None ->
            model, Cmd.ofMsg (Error "Parse error")
    | Error message ->
        { model with current = { model.current with result = message } }, Cmd.none
    | Evaluate equation ->
        let wat = equationToWebAssemblyText equation
        let wasm = equationToWebAssemblyBinary equation
        let wasmBytes = Uint8Array.from wasm

        model, Cmd.OfPromise.either WebAssembly.instantiate wasmBytes 
                (fun wa -> Evaluated ({ equation = string equation; result = string (wa.instance.exports?evaluate()); wasm = wasm; wat = wat })) (fun _ -> Error "run error")
    | Evaluated evaluateResult ->
        printfn "evaluated %A" evaluateResult
        { model with current = evaluateResult; history = model.current :: model.history }, Cmd.none

let init () = 
    let initialResult = { equation = ""; result = ""; wasm = []; wat = ""}
    let initialModel = { current = initialResult; history = [] }
    initialModel, Cmd.none

let view model dispatch =
    form.onsubmit <- fun event ->
        event.preventDefault()
        dispatch (Parse input.value)

    binary.innerText <- model.current.wasm |> List.map byteToHex |> String.concat " "
    text.innerText <- model.current.wat
    output.innerText <- string model.current.result

Program.mkProgram init update view
|> Program.withConsoleTrace
|> Program.run

// TODO: make styling prettier
// TODO: try to use prepack