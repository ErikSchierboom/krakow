module Krakow.App

open Fable.Import
open Browser.Dom

open Krakow.Core.Evaluator

// TODO: use rollup instead of Webpack to minimize bundle size
// TODO: show WebAssembly text output
// TODO: show WebAssembly binary output
// TODO: execute using WebAssembly

let form = document.getElementById("form") :?> Browser.Types.HTMLFormElement
let input = document.getElementById("input") :?> Browser.Types.HTMLInputElement
let output = document.getElementById("output") :?> Browser.Types.HTMLDivElement

form.onsubmit <- fun event ->
    event.preventDefault()

    match evaluate input.value with
    | Some result ->
        output.innerText <- string result
    | None ->
        output.innerText <- "Error evaluating expression"