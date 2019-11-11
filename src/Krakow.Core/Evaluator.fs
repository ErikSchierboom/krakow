module Krakow.Core.Evaluator

open Krakow.Core.Parser

let private evaluateExpression stack expression =
    match expression, stack with
    | Operand operand, _ -> operand :: stack
    | Operator Add, x :: y :: xs -> y + x :: xs
    | Operator Sub, x :: y :: xs -> y - x :: xs
    | Operator Mul, x :: y :: xs -> y * x :: xs
    | Operator Div, x :: y :: xs -> y / x :: xs
    | _, _ -> failwith "Invalid expression"

let private evaluateEquation (Equation expressions) =
    expressions
    |> List.fold evaluateExpression []
    |> List.head

let evaluate str = parse str |> Result.map evaluateEquation
