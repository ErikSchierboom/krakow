module Krakow.Core.Domain

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

type Operand =
    | Integer of int
    override self.ToString() =
        match self with
        | Integer operand -> string operand

type Expression =
    | OperatorExpression of Operator
    | OperandExpression of Operand
    override self.ToString() =
        match self with
        | OperatorExpression operatorExpression -> string operatorExpression
        | OperandExpression operandExpression -> string operandExpression

type Equation =
    | Equation of Expression list
    override self.ToString() =
        let (Equation expressions) = self
        expressions
        |> List.map string
        |> String.concat " "
