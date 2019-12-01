module Krakow.Core.Tests.PropertyBasedTesting

open System
open FsCheck
open Expecto
open Krakow.Core.Domain
open Krakow.Core.Tests.Helpers

type EmptyEquation =
    | EmptyEquation of string

    member self.Get =
        match self with
        | EmptyEquation equation -> equation

    override self.ToString() = self.Get

type UnbalancedEquation =
    | UnbalancedEquation of string

    member self.Get =
        match self with
        | UnbalancedEquation equation -> equation

    override self.ToString() = self.Get

type InvalidEquation =
    | InvalidEquation of string * string

    member self.Get =
        match self with
        | InvalidEquation(equation, invalidToken) -> (equation, invalidToken)

    member self.InvalidToken = snd self.Get
    override self.ToString() = fst self.Get

type ValidEquation =
    | ValidEquation of string

    member self.Get =
        match self with
        | ValidEquation equation -> equation

    override self.ToString() = self.Get

let private whiteSpaceChar =
    Arb.Default.Char()
    |> Arb.filter Char.IsWhiteSpace
    |> Arb.toGen

let private emptyEquation =
    whiteSpaceChar
    |> Gen.arrayOf
    |> Gen.map String
    |> Gen.map EmptyEquation
    |> Arb.fromGen

let private operandExpression =
    Arb.Default.PositiveInt()
    |> Arb.convert (fun positiveInt -> Integer positiveInt.Get) (fun (Integer operand) -> PositiveInt operand)
    |> Arb.toGen
    |> Gen.map OperandExpression

let private operatorExpression =
    Arb.from<Operator>
    |> Arb.toGen
    |> Gen.map OperatorExpression

let private expressionForEmptyStack =
    gen {
        let! operands = Gen.listOfLength 2 operandExpression
        let! operator = operatorExpression
        return operands @ [ operator ] }

let private expressionForNonEmptyStack =
    gen {
        let! operand = operandExpression
        let! operator = operatorExpression
        return [ operand; operator ] }

let private equation =
    gen {
        let! firstExpressions = expressionForEmptyStack
        let! laterExpressions = Gen.listOf expressionForNonEmptyStack |> Gen.map (List.collect id)
        return Equation(firstExpressions @ laterExpressions) } |> Arb.fromGen

let private unbalancedEquation =
    equation
    |> Arb.toGen
    |> Gen.map (fun (Equation equation) -> Equation(List.deleteAtRandom equation))
    |> Gen.map string
    |> Gen.map UnbalancedEquation
    |> Arb.fromGen

let private invalidToken =
    let validOperator c = List.contains c [ '+'; '-'; '*'; '/' ]
    let validOperand c = Char.IsDigit c
    let isWhiteSpace c = Char.IsWhiteSpace c

    Arb.Default.Char()
    |> Arb.filter (fun c -> not (validOperator c) && not (validOperand c) && not (isWhiteSpace c))
    |> Arb.toGen
    |> Gen.map string

let private invalidEquation =
    gen {
        let! Equation(expressions) = Arb.toGen equation
        let! token = invalidToken
        let invalidEquation' =
            expressions
            |> List.map string
            |> List.insertAtRandom token
            |> String.concat " "
        return InvalidEquation(invalidEquation', token)
    }
    |> Arb.fromGen

let private validEquation =
    equation
    |> Arb.toGen
    |> Gen.map string
    |> Gen.map ValidEquation
    |> Arb.fromGen

type EquationArbitraries =
    static member EmptyEquation() = emptyEquation
    static member UnbalancedEquation() = unbalancedEquation
    static member InvalidEquation() = invalidEquation
    static member ValidEquation() = validEquation

let config = { FsCheckConfig.defaultConfig with arbitrary = [ typedefof<EquationArbitraries> ] }
