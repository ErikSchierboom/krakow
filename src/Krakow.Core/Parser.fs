module Krakow.Core.Parser

open Krakow.Core.Helpers
open Krakow.Core.Domain

let private parseExpression word =
    match word with
    | "+" -> Ok (OperatorExpression Add)
    | "-" -> Ok (OperatorExpression Sub)
    | "*" -> Ok (OperatorExpression Mul)
    | "/" -> Ok (OperatorExpression Div)
    | Int i -> Ok (OperandExpression (Operand i))
    | _ -> Error (Invalid word)
    
let private validate expressions =
    let rec helper remainder stack =
        match remainder, stack with
        | [], [] -> Error Empty
        | [], [ OperandExpression _ ] -> Ok(expressions)
        | OperandExpression i :: xs, _ -> helper xs (OperandExpression i :: stack)
        | OperatorExpression i :: xs, OperandExpression _ :: OperandExpression _ :: ys -> helper xs (OperandExpression (Operand 0) :: ys)
        | _ -> Error Unbalanced

    helper expressions []
    
let parse str =
    str
    |> String.words
    |> List.map parseExpression
    |> Result.ofList
    |> Result.bind validate
    |> Result.map Equation
