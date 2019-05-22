module Krakow.App

open Fable.Import
open Browser.Dom

open Krakow.Core.Evaluation

// TODO: use rollup instead of Webpack to minimize bundle size

let form = document.getElementById("form") :?> Browser.Types.HTMLFormElement
let input = document.getElementById("input") :?> Browser.Types.HTMLInputElement
let output = document.getElementById("output") :?> Browser.Types.HTMLDivElement

form.onsubmit <- fun event ->
    event.preventDefault()

    match evaluate input.value with
    | Result.Ok result ->
        output.innerText <- string result
    | Result.Error error ->
        output.innerText <- error