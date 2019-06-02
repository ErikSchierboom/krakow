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

// let byteToHex (b: int) = b.ToString("X2")

// let showWebAssemblyOutput webAssemblyBinary =
//     let result = WebAssembly.instantiate(Uint8Array.from(webAssemblyBinary)) 
//     result.``then``(fun x -> output.innerText <- x.instance.exports?evaluate())

// let showWebAssemblyBinary webAssemblyBinary =
//     binary.innerText <- webAssemblyBinary |> List.map byteToHex |> String.concat " " 

// let showWebAssemblyText webAssemblyText =
//     text.innerText <- webAssemblyText

// let showError =
//     output.innerText <- "Error evaluating expression"

type EvaluateResult =
    { equation: string
      result: int
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
        printfn "parsing %s" input

        match parse input with
        | Some equation ->
            model, Cmd.ofMsg (Evaluate equation)
        | None ->
            model, Cmd.ofMsg (Error "Parse error")
    | Error message ->
        printfn "parse eror %s" message
        model, Cmd.none
    | Evaluate equation ->
        let wat = equationToWebAssemblyText equation
        let wasm = equationToWebAssemblyBinary equation

        let wasmBytes = Uint8Array.from wasm
        printfn "evaluate %A" equation
        printfn "wat %A" wat
        printfn "wasm %A" wasm
        model, Cmd.OfPromise.either WebAssembly.instantiate wasmBytes (fun wa -> Evaluated (
            { equation = string equation; result = wa.instance.exports?evaluate(); wasm = wasm; wat = wat })) (fun _ -> Error "run error")
    | Evaluated evaluateResult ->
        printfn "evaluated %A" evaluateResult
        model, Cmd.none

        // model, Cmd.OfPromise.either parse equation
        // let result = { equation = equation; result = 1; wasm = [1; 2]; wat = "(module)" }
        // { model with current = result; history = model.current :: model.history }, Cmd.none

let init () = 
    let initialResult = { equation = ""; result = 0; wasm = []; wat = ""}
    let initialModel = { current = initialResult; history = [] }
    initialModel, Cmd.none

let view _ dispatch = 
    form.onsubmit <- fun event ->
      event.preventDefault()
      dispatch (Parse input.value)

Program.mkProgram init update view
|> Program.withConsoleTrace
|> Program.run

// TODO: make styling prettier
// TODO: try to use prepack