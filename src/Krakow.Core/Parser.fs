module Krakow.Core.Parser

open Tokenizer

type Expression =
    | Operand of int
    | Add
    | Sub
    | Mul
    | Div

let private parseExpression token =
    match token with
    | Plus     -> Add
    | Minus    -> Sub
    | Asterisk -> Mul
    | Slash    -> Div
    | Number i -> Operand i
    
let private parseEquation tokens = List.map parseExpression tokens

let private validateEquation equation =
    let rec helper expressions stack =
        match expressions, stack with
        | Add::xs, Operand _::Operand _::ys -> helper xs (Operand 0 :: ys)
        | Sub::xs, Operand _::Operand _::ys -> helper xs (Operand 0 :: ys)
        | Mul::xs, Operand _::Operand _::ys -> helper xs (Operand 0 :: ys)
        | Div::xs, Operand _::Operand _::ys -> helper xs (Operand 0 :: ys)
        | Operand i::xs, _ -> helper xs (Operand i :: stack)
        | [], [Operand _] -> Some equation
        | _ -> None
    
    helper equation []

let parse str =
    str
    |> tokenize
    |> Option.map parseEquation
    |> Option.bind validateEquation