module Krakow.Core.Evaluator

open Krakow.Core.Parser

let private evaluateExpression stack expression =
    match expression, stack with
    | Operand operand, _ -> operand :: stack
    | Add, x::y::xs -> y + x :: xs
    | Sub, x::y::xs -> y - x :: xs
    | Mul, x::y::xs -> y * x :: xs
    | Div, x::y::xs -> y / x :: xs
    | _, _-> failwith "Invalid expression"

let rec private evaluateEquation equation =
    match List.fold evaluateExpression [] equation with
    | [i] -> Some i
    | _   -> None

let evaluate str =
    parse str
    |> Option.bind evaluateEquation