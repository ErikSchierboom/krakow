module Krakow.Core.Tests.ParserTests

open Xunit
open FsUnit.Xunit

open Krakow.Core.Parser

[<Theory>]
[<InlineData("")>]
[<InlineData("+")>]
[<InlineData("-1")>]
[<InlineData("1 2")>]
[<InlineData("1 +")>]
[<InlineData("1 2 + -")>]
[<InlineData("1 2 - 3 * 4 5 /")>]
let ``Parse invalid equation`` equation =
    parse equation |> should equal None

[<Theory>]
[<InlineData("0", 0)>]
[<InlineData("1", 1)>]
[<InlineData("10", 10)>]
[<InlineData("1337", 1337)>]
let ``Parse minimal equation`` equation expected =
    parse equation |> should equal (Some (Equation [Expression.Operand expected]))

[<Fact>]
let ``Parse simple equation`` () =
    parse "5 3 +" |> should equal (Some (Equation [Expression.Operand 5; Expression.Operand 3; Expression.Add]))

[<Fact>]
let ``Parse complex equation`` () =
    let expected = (Equation [
        Expression.Operand 1
        Expression.Operand 3
        Expression.Add
        Expression.Operand 54
        Expression.Div
        Expression.Operand 8
        Expression.Mul
        Expression.Operand 4
        Expression.Sub
    ])
    parse "1 3 + 54 / 8 * 4 -" |> should equal (Some expected)