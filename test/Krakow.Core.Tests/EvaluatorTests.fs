module Krakow.Core.Tests.EvaluatorTests

open Xunit
open FsUnit.Xunit

open Krakow.Core.Evaluator

[<Theory>]
[<InlineData("+")>]
[<InlineData("1 2")>]
[<InlineData("1 +")>]
[<InlineData("1 2 + -")>]
[<InlineData("1 2 - 3 * 4 5 /")>]
let ``Evaluate invalid equation`` equation =
    evaluate equation |> should equal None

[<Theory>]
[<InlineData(0, "0")>]
[<InlineData(1, "1")>]
[<InlineData(10, "10")>]
[<InlineData(123, "123")>]
let ``Evaluate single operand equation`` (expected, equation) =
    evaluate equation |> should equal (Some expected)

[<Theory>]
[<InlineData(8, "6 2 +")>]
[<InlineData(1, "9 8 -")>]
[<InlineData(3, "15 5 /")>]
[<InlineData(9, "3 3 *")>]
let ``Evaluate single operator equation`` (expected, equation) =
    evaluate equation |> should equal (Some expected)

[<Theory>]
[<InlineData(20, "10 2 / 4 + 5 6 + +")>]
[<InlineData(75, "1 3 + 36 + 4 / 8 * 5 -")>]
[<InlineData(5, "15 7 1 1 + - / 3 * 2 1 1 + + -")>]
let ``Evaluate complex equation`` (expected, equation) =
    evaluate equation |> should equal (Some expected)
