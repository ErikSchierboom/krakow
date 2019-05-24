module Krakow.Core.Evaluator
//
//open Krakow.Core.Parsing
//
//let private evaluateExpression stack expression =
//    match expression, stack with
//    | Operand operand, _ -> Result.Ok (operand :: stack)
//    | Add, x::y::xs -> Result.Ok (y + x :: xs)
//    | Sub, x::y::xs -> Result.Ok (y - x :: xs)
//    | Mul, x::y::xs -> Result.Ok (y * x :: xs)
//    | Div, x::y::xs -> Result.Ok (y / x :: xs)
//    | _, _-> Result.Error "Operator requires two operands on the stack"
//
//let rec private evaluateEquation stack equation =
//    match equation, stack with
//    | Equation [], [result] ->
//        Result.Ok result
//    | Equation [], _ ->
//        Result.Error "Equation does not resolve to single result"
//    | Equation (x::xs), _ ->
//        evaluateExpression stack x
//        |> Result.bind (fun newStack -> evaluateEquation newStack (Equation xs))
//
//let evaluate str =
//    parse str
//    |> Result.bind (evaluateEquation [])