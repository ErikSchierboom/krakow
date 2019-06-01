module Krakow.Website.App

open Fable.Import
open Fable.Core.JsInterop
open Browser.Dom

open Krakow.Core.WebAssembly.Text
open Krakow.Core.WebAssembly.Binary
open Krakow.Website.Interop

let form = document.getElementById("form") :?> Browser.Types.HTMLFormElement
let input = document.getElementById("input") :?> Browser.Types.HTMLInputElement
let output = document.getElementById("output") :?> Browser.Types.HTMLDivElement
let text = document.getElementById("text") :?> Browser.Types.HTMLDivElement
let binary = document.getElementById("binary") :?> Browser.Types.HTMLDivElement

let byteToHex (b: int) = b.ToString("X2")

let showWebAssemblyOutput webAssemblyBinary =
    let result = WebAssembly.instantiate(Uint8Array.from(webAssemblyBinary)) 
    result.``then``(fun x -> output.innerText <- x.instance.exports?evaluate())

let showWebAssemblyBinary webAssemblyBinary =
    binary.innerText <- webAssemblyBinary |> List.map byteToHex |> String.concat " " 

let showWebAssemblyText webAssemblyText =
    text.innerText <- webAssemblyText

let showError =
    output.innerText <- "Error evaluating expression"

let evaluate equation =
    match equationToWebAssemblyText equation, equationToWebAssemblyBinary equation with
    | Some text, Some bytes -> 
        showWebAssemblyOutput bytes |> ignore
        showWebAssemblyBinary bytes |> ignore
        showWebAssemblyText text
    | _ ->
        showError

form.onsubmit <- fun event ->
    event.preventDefault()
    evaluate input.value

// TODO: make styling prettier
// TODO: try to use prepack