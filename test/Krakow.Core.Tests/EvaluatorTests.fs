module Krakow.Core.Tests.EvaluatorTests

open Xunit
open FsUnit.Xunit

open Krakow.Core.Evaluator
open Krakow.Core.Parser

[<Theory>]
[<InlineData("")>]
[<InlineData("  ")>]
[<InlineData("\t\n")>]
[<InlineData("\r\n")>]
let ``Evaluate empty equation`` equation =
    evaluate equation = (Error EmptyEquation) |> should be True

[<Fact>]
let ``Evaluate equation with invalid token`` =
    evaluate "1 a" = (Error (InvalidToken "a")) |> should be True

[<Fact>]
let ``Evaluate equation that does not reduce to single number invalid token`` =
    evaluate "1 2" = (Error DoesNotReduceToSingleNumber) |> should be True

[<Fact>]
let ``Evaluate equation with operator missing operand`` =
    evaluate "1 +" = (Error (OperatorMissingOperands (Operator.Add))) |> should be True

[<Theory>]
[<InlineData(0, "0")>]
[<InlineData(1, "1")>]
[<InlineData(10, "10")>]
[<InlineData(123, "123")>]
let ``Evaluate single operand equation`` (expected, equation) =
    evaluate equation = (Ok expected) |> should be True

[<Theory>]
[<InlineData(8, "6 2 +")>]
[<InlineData(1, "9 8 -")>]
[<InlineData(3, "15 5 /")>]
[<InlineData(9, "3 3 *")>]
let ``Evaluate single operator equation`` (expected, equation) =
    evaluate equation = (Ok expected) |> should be True

[<Theory>]
[<InlineData(20, "10 2 / 4 + 5 6 + +")>]
[<InlineData(75, "1 3 + 36 + 4 / 8 * 5 -")>]
[<InlineData(5, "15 7 1 1 + - / 3 * 2 1 1 + + -")>]
let ``Evaluate complex equation`` (expected, equation) =
    evaluate equation = (Ok expected) |> should be True
