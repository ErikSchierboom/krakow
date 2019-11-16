module Krakow.Core.Tests.Helpers

open System
open FsCheck
open FsCheck.Arb
open Expecto
open Krakow.Core.Parser

type EmptyString = EmptyString of string with
    member x.Get = match x with EmptyString r -> r
    override x.ToString() = x.Get

type ValidToken = ValidToken of string with
    member x.Get = match x with ValidToken r -> r
    override x.ToString() = x.Get
    
type InvalidToken = InvalidToken of string with
    member x.Get = match x with InvalidToken r -> r
    override x.ToString() = x.Get
    
type EmptyEquation = EmptyEquation of string with
    member x.Get = match x with EmptyEquation r -> r
    override x.ToString() = x.Get
    
type UnbalancedEquation = UnbalancedEquation of string with
    member x.Get = match x with UnbalancedEquation r -> r
    override x.ToString() = x.Get

let private validToken =
    let validOperators = set ["+"; "-"; "*"; "/"]
    let validOperator str = Set.contains str validOperators
    let validOperand str = Seq.forall Char.IsDigit str
    
    fun str -> validOperator str || validOperand str
    
type CustomArbitraries =
    static member EmptyString() = 
        Default.String()
        |> filter (fun s -> s <> null && String.IsNullOrWhiteSpace(s))
        |> convert EmptyString string

    static member Operator() = 
        Gen.oneof ["+"; "-"; "*"; "/"]
    
    static member Expression() = 
        Default.String()
        |> filter validToken
        |> convert ValidToken string

    static member InvalidToken() = 
        Default.String()
        |> filter validToken
        |> convert Expression string
        
    static member Equation() = 
        let genEquation =
            
            gen {
                let startingOperands = Gen.listOfLength 2 CustomArbitraries.Expression
                let operator = 
                return DateTimeOffset(DateTime.SpecifyKind(t, DateTimeKind.Unspecified), tz)
            }
        let shrinkEquation (equation: Equation) =
            seq {
                for ts in shrinkTimeZone d.Offset ->
                    DateTimeOffset(d.DateTime, ts)
                if d.Offset <> TimeSpan.Zero then
                    yield DateTimeOffset(d.DateTime, TimeSpan.Zero)
                for dt in shrink d.DateTime ->
                    DateTimeOffset(dt, TimeSpan.Zero) }
        fromGenShrink (genEquation, shrinkEquation)

    static member EmptyEquation() = 
        CustomArbitraries.EmptyString()
        |> convert (fun emptyString -> EmptyEquation emptyString.Get) (fun emptyEquation -> EmptyString emptyEquation.Get)
        
        

    static member UnbalancedEquation() = 
        let genUnbalanced = gen {
            let! 
        }

let config = { FsCheckConfig.defaultConfig with arbitrary = [typedefof<CustomArbitraries>] }