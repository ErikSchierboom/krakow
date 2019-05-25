module Krakow.Core.Parser

open Tokenizer

type Expression =
    | Operand of int
    | Add
    | Sub
    | Mul
    | Div
    
type Equation =
    | Equation of Expression list

let private parseExpression token =
    match token with
    | Plus     -> Add
    | Minus    -> Sub
    | Asterisk -> Mul
    | Slash    -> Div
    | Number i -> Operand i
    
let private parseEquation tokens =
    tokens
    |> List.map parseExpression
    |> Equation

let private validateEquation (Equation expressions) =
    let rec helper unprocessedExpressions stack =
        match unprocessedExpressions, stack with
        | Add::xs, Operand _::Operand _::ys -> helper xs (Operand 0 :: ys)
        | Sub::xs, Operand _::Operand _::ys -> helper xs (Operand 0 :: ys)
        | Mul::xs, Operand _::Operand _::ys -> helper xs (Operand 0 :: ys)
        | Div::xs, Operand _::Operand _::ys -> helper xs (Operand 0 :: ys)
        | Operand i::xs, _ -> helper xs (Operand i :: stack)
        | [], [Operand _] -> Some (Equation expressions)
        | _ -> None
    
    helper expressions []

let parse str =
    str
    |> tokenize
    |> Option.map parseEquation
    |> Option.bind validateEquation