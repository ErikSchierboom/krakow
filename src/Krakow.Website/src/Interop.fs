module Krakow.Website.Interop

open Fable.Core
open Fable.Core.JS

[<AutoOpen>]
module WebAssembly =
    type Instance  =
        { exports: obj }

    type ResultObject = 
        { instance: Instance }

    [<Emit("WebAssembly.instantiate($0)")>]
    let instantiate (bufferSource: ArrayBuffer) : JS.Promise<ResultObject> = jsNative

[<AutoOpen>]
module Uint8Array =
    [<Emit("Uint8Array.from($0)")>]
    let from (bytes: int list): ArrayBuffer = jsNative