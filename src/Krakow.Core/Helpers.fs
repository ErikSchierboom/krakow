module Krakow.Core.Helpers

open System

let (|Int|_|) (str: string) =
    match Int32.TryParse(str) with
    | true, i -> Some i
    | _ -> None

module Result =
    let ofList results =
        let folder resultState resultElement =
            match resultState, resultElement with
            | Ok results, Ok element -> Ok(element :: results)
            | Error err, _ -> Error err
            | _, Error err -> Error err

        results
        |> List.fold folder (Ok [])
        |> Result.map List.rev

module String =
    let words (str: string) = str.Trim().Split([| ' ' |], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray
