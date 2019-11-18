module Krakow.Core.Evaluator

open Krakow.Core.Domain
open Krakow.Core.Parser

let private evaluateExpression stack expression =
    match expression, stack with
    | OperandExpression(Operand operand), _ -> operand :: stack
    | OperatorExpression Add, x :: y :: xs -> y + x :: xs
    | OperatorExpression Sub, x :: y :: xs -> y - x :: xs
    | OperatorExpression Mul, x :: y :: xs -> y * x :: xs
    | OperatorExpression Div, x :: y :: xs -> y / x :: xs
    | _, _ -> failwith "Invalid expression"

let private evaluateEquation (Equation expressions) =
    expressions
    |> List.fold evaluateExpression []
    |> List.head

let evaluate str = parse str |> Result.map evaluateEquation
