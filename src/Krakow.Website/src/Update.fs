module Krakow.Website.Update

open Elmish

open Fable.Core.JsInterop

open Krakow.Core.Parser
open Krakow.Core.WebAssembly.Text
open Krakow.Core.WebAssembly.Binary
open Krakow.Website.Model
open Krakow.Website.Interop

let private equationEvaluatedSuccessfully model equation =
    let wat = equationToWebAssemblyText equation
    let wasm = equationToWebAssemblyBinary equation
    let wasmByteArray = Uint8Array.from wasm

    let onSuccess wa =
        EquationEvaluatedSuccessfully
            ({ result = wa.instance.exports?evaluate ()
               wasm = wasm
               wat = wat })

    let onError ex = EquationEvaluatedWithError(WebAssemblyException ex)
    let cmd = Cmd.OfPromise.either WebAssembly.instantiate wasmByteArray onSuccess onError

    model, cmd

let private equationEvaluatedWithError model error =
    let evaluationError =
        match error with
        | Empty -> EmptyEquation
        | Invalid token -> InvalidEquation token
        | Unbalanced -> UnbalancedEquation

    model, Cmd.ofMsg (EquationEvaluatedWithError evaluationError)

let private evaluate model =
    match parse model.equation with
    | Ok evaluation -> equationEvaluatedSuccessfully model evaluation
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
