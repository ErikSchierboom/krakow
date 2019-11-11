module Krakow.Core.Parser

open Tokenizer

type Operator =
    | Add
    | Sub
    | Mul
    | Div

type Expression =
    | Operator of Operator
    | Operand of int

type Equation = Equation of Expression list

type ParserError =
    | InvalidToken of string
    | EmptyEquation
    | DoesNotReduceToSingleNumber
    | OperatorMissingOperands of Operator

let private parseExpression token =
    match token with
    | Plus -> Operator Add
    | Minus -> Operator Sub
    | Asterisk -> Operator Mul
    | Slash -> Operator Div
    | Number i -> Operand i

let private parseEquation (Tokens tokens) =
    tokens
    |> List.map parseExpression
    |> Equation

let private validateEquation (Equation expressions) =
    let rec helper unprocessedExpressions stack =
        match unprocessedExpressions, stack with
        | [], [] -> Error(EmptyEquation)
        | [], [ Operand _ ] -> Ok(Equation expressions)
        | Operand i :: xs, _ -> helper xs (Operand i :: stack)
        | Operator _ :: xs, Operand _ :: Operand _ :: ys -> helper xs (Operand 0 :: ys)
        | Operator operator :: _, _ -> Error(OperatorMissingOperands operator)
        | _ -> Error DoesNotReduceToSingleNumber

    helper expressions []

let private tokenizerErrorToParserError error =
    match error with
    | TokenizerError.InvalidToken token -> InvalidToken token

let parse str =
    str
    |> tokenize
    |> Result.map parseEquation
    |> Result.mapError tokenizerErrorToParserError
    |> Result.bind validateEquation
