module Krakow.Core.Parser

open Krakow.Core.Helpers

type EquationError =
    | Empty
    | Invalid of string
    | Unbalanced
    
type Operator =
    | Add
    | Sub
    | Mul
    | Div

type Expression =
    | Operator of Operator
    | Operand of int

type Equation = Equation of Expression list

let private parseExpression word =
    match word with
    | "+" -> Ok (Operator Add)
    | "-" -> Ok (Operator Sub)
    | "*" -> Ok (Operator Mul)
    | "/" -> Ok (Operator Div)
    | Int i -> Ok (Operand i)
    | _ -> Error (Invalid word)
    
let private parseEquation str =
    str
    |> String.words
    |> List.map parseExpression
    |> Result.ofList
    |> Result.map Equation
    
let private validateEquation (Equation expressions) =
    let rec helper remainder stack =
        match remainder, stack with
        | [], [] -> Error Empty
        | [], [ Operand _ ] -> Ok(Equation expressions)
        | Operand i :: xs, _ -> helper xs (Operand i :: stack)
        | Operator _ :: xs, Operand _ :: Operand _ :: ys -> helper xs (Operand 0 :: ys)
        | _ -> Error Unbalanced

    helper expressions []
    
let parse str =
    str
    |> parseEquation
    |> Result.bind validateEquation
