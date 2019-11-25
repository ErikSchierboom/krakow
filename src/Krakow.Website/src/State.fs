module Krakow.Website.State

open Elmish

open Fable.Core.JsInterop

open Krakow.Core.Domain
open Krakow.Core.WebAssembly

open Krakow.Website.Interop
open Krakow.Website.Types

let private equationEvaluatedSuccessfully model webAssembly =
    let (WebAssemblyText(wat)) = webAssembly.Text
    let (WebAssemblyBinary(wasm)) = webAssembly.Binary

    let onSuccess webAssemblyInBrowser =
        EquationEvaluatedSuccessfully
            ({ Result = webAssemblyInBrowser.instance.exports?evaluate ()
               Wasm = wasm
               Wat = wat })

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
    match convert model.Equation with
    | Ok webAssembly -> equationEvaluatedSuccessfully model webAssembly
    | Error error -> equationEvaluatedWithError model error

let update msg model =
    match msg with
    | UpdateEquation newEquation -> { model with Equation = newEquation }, Cmd.none
    | EvaluateEquation -> evaluate model
    | EquationEvaluatedSuccessfully evaluation -> { model with Evaluation = Some(Ok evaluation) }, Cmd.none
    | EquationEvaluatedWithError error -> { model with Evaluation = Some(Error error) }, Cmd.none

let init() =
    { Evaluation = None
      Equation = "" }, Cmd.none
