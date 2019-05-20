open System

open Krakow.Core.Evaluation

let private read() =
    Console.ReadLine()

let private eval input =
    evaluate input

let private print output =
    match output with
    | Result.Ok outcome  -> printfn "%i" outcome
    | Result.Error error -> printfn "%s" error

[<EntryPoint>]
let main _ =
    while true do
        read()
        |> eval
        |> print
    
    0
