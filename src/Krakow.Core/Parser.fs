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
    override self.ToString() =
        match self with
        | Add -> "+"
        | Sub -> "-"
        | Mul -> "*"
        | Div -> "/"
        
type Operand = Operand of int

type Expression =
    | OperatorExpression of Operator
    | OperandExpression of Operand

type Equation =
    | Equation of Expression list
    override self.ToString() =
        let (Equation expressions) = self
        expressions
        |> List.map string
        |> String.concat " "

let private parseExpression word =
    match word with
    | "+" -> Ok (OperatorExpression Add)
    | "-" -> Ok (OperatorExpression Sub)
    | "*" -> Ok (OperatorExpression Mul)
    | "/" -> Ok (OperatorExpression Div)
    | Int i -> Ok (OperandExpression (Operand i))
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
        | [], [ OperandExpression _ ] -> Ok(Equation expressions)
        | OperandExpression i :: xs, _ -> helper xs (OperandExpression i :: stack)
        | OperatorExpression i :: xs, OperandExpression _ :: OperandExpression _ :: ys -> helper xs (OperandExpression (Operand 0) :: ys)
        | _ -> Error Unbalanced

    helper expressions []
    
let parse str =
    str
    |> parseEquation
    |> Result.bind validateEquation
