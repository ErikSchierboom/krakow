module Krakow.App

open Fable.Import
open Browser.Dom

open Krakow.Core.WebAssembly.Binary

// TODO: use rollup instead of Webpack to minimize bundle size
// TODO: show WebAssembly text output
// TODO: show WebAssembly binary output
// TODO: execute using WebAssembly

// const bytes = [
//         0x00,
//         0x61,
//         0x73,
//         0x6d,
//         0x01,
//         0x00,
//         0x00,
//         0x00,
//         0x01,
//         0x05,
//         0x01,
//         0x60,
//         0x00,
//         0x01,
//         0x7f,
//         0x03,
//         0x02,
//         0x01,
//         0x00,
//         0x07,
//         0x0c,
//         0x01,
//         0x08,
//         0x65,
//         0x76,
//         0x61,
//         0x6c,
//         0x75,
//         0x61,
//         0x74,
//         0x65,
//         0x00,
//         0x00,
//         0x0a,
//         0x09,
//         0x01,
//         0x07,
//         0x00,
//         0x41,
//         0x09,
//         0x41,
//         0x08,
//         0x6b,
//         0x0b
//       ];

//       WebAssembly.instantiate(Uint8Array.from(bytes)).then(results =>
//         alert(results.instance.exports.evaluate())
//       );

let form = document.getElementById("form") :?> Browser.Types.HTMLFormElement
let input = document.getElementById("input") :?> Browser.Types.HTMLInputElement
let output = document.getElementById("output") :?> Browser.Types.HTMLDivElement

let evaluate equation =
    match equationToWebAssemblyBinary equation with
    | Some bytes -> "Success"
    | None -> "Error evaluating expression"

form.onsubmit <- fun event ->
    event.preventDefault()
    output.innerText <- evaluate input.value