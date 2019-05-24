open System

open Krakow.Core.Evaluator

let private read() =
    Console.ReadLine()

let private eval input =
    evaluate input

let private print output =
    match output with
    | Some outcome  -> printfn "%i" outcome
    | None -> printfn "Error evaluating expression"

[<EntryPoint>]
let main _ =
    while true do
        read()
        |> eval
        |> print
    
    0
