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

let update msg model =
    match msg with
    | UpdateEquation newEquation -> { model with equation = newEquation }, Cmd.none
    | EvaluateEquation -> evaluate model
    | EquationEvaluatedSuccessfully evaluation -> { model with evaluation = Some(Ok evaluation) }, Cmd.none
    | EquationEvaluatedWithError error -> { model with evaluation = Some(Error error) }, Cmd.none

let init() =
    { evaluation = None
      equation = "" }, Cmd.none
