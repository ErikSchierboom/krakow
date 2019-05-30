module Krakow.Website.App

open Fable.Import
open Fable.Core.JsInterop
open Browser.Dom

open Krakow.Core.WebAssembly.Binary
open Krakow.Website.Interop

let form = document.getElementById("form") :?> Browser.Types.HTMLFormElement
let input = document.getElementById("input") :?> Browser.Types.HTMLInputElement
let output = document.getElementById("output") :?> Browser.Types.HTMLDivElement

let createWebAssemblyResult bytes =
    WebAssembly.instantiate(Uint8Array.from(bytes))

let evaluate equation =
    match equationToWebAssemblyBinary equation with
    | Some bytes -> 
        let result = createWebAssemblyResult bytes
        result.``then``(fun x -> output.innerText <- x.instance.exports?evaluate())
    | None ->
        output.innerText <- "Error evaluating expression"
        Promise.lift ()

form.onsubmit <- fun event ->
    event.preventDefault()
    evaluate input.value

// TODO: show WebAssembly text output
// TODO: show WebAssembly binary output