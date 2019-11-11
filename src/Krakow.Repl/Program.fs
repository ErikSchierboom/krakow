open System

open Krakow.Core.Evaluator
open Krakow.Core.Parser

let private printInstructions() =
    printfn "Please enter an equation and evaluate it by pressing enter..."

let private read() =
    Console.ReadLine()

let private eval input =
    evaluate input

let private printOutcome outcome =
    printfn "Outcome: %A" outcome

let private parserErrorMessage error = 
    match error with 
    | InvalidToken token -> sprintf "Invalid token: %s" token
    | EmptyEquation -> "Empty equation"
    | DoesNotReduceToSingleNumber -> "Equation does not evaluate to single number"
    | OperatorMissingOperands operator -> sprintf "Operator %A requires more numbers" operator

let private printError error =
    let errorMessage = parserErrorMessage error
    printfn "Error: %s" errorMessage

let private print result =
    match result with
    | Ok outcome  -> printOutcome outcome
    | Error error -> printError error

let private loop() = 
    while true do
        read()
        |> eval
        |> print

[<EntryPoint>]
let main _ =
    printInstructions()
    loop()
    0
