module Krakow.Core.Tests.EvaluationTests

open Xunit

open Krakow.Core.Evaluation

[<Theory>]
[<InlineData("+")>]
[<InlineData("1 2")>]
[<InlineData("1 +")>]
[<InlineData("1 2 + -")>]
[<InlineData("1 2 - 3 * 4 5 /")>]
let ``Evaluate invalid equation`` equation =
    Assert.Equal(Result.Error "Invalid equation", evaluate equation)

[<Theory>]
[<InlineData(0, "0")>]
[<InlineData(1, "1")>]
[<InlineData(10, "10")>]
[<InlineData(123, "123")>]
let ``Evaluate single operand equation`` (expected, equation) =
    Assert.Equal(Result.Ok expected, evaluate equation)

[<Theory>]
[<InlineData(8, "6 2 +")>]
[<InlineData(1, "9 8 -")>]
[<InlineData(3, "15 5 /")>]
[<InlineData(9, "3 3 *")>]
let ``Evaluate single operator equation`` (expected, equation) =
    Assert.Equal(Result.Ok expected, evaluate equation)

[<Theory>]
[<InlineData(20, "10 2 / 4 + 5 6 + +")>]
[<InlineData(75, "1 3 + 36 + 4 / 8 * 5 -")>]
[<InlineData(5, "15 7 1 1 + - / 3 * 2 1 1 + + -")>]
let ``Evaluate complex equation`` (expected, equation) =
    Assert.Equal(Result.Ok expected, evaluate equation)
