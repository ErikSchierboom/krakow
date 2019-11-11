module Krakow.Core.Tests.ParserTests

open Xunit
open FsUnit.Xunit

open Krakow.Core.Parser

[<Theory>]
[<InlineData("")>]
[<InlineData("  ")>]
[<InlineData("\t\n")>]
[<InlineData("\r\n")>]
let ``Parse empty equation`` equation =
    parse equation = (Error EmptyEquation) |> should be True

[<Fact>]
let ``Parse equation with invalid token`` =
    parse "1 a" = (Error (InvalidToken "a")) |> should be True

[<Fact>]
let ``Parse equation that does not reduce to single number invalid token`` =
    parse "1 2" = (Error DoesNotReduceToSingleNumber) |> should be True

[<Fact>]
let ``Parse equation with operator missing operand`` =
    parse "1 +" = (Error (OperatorMissingOperands (Operator.Add))) |> should be True

[<Theory>]
[<InlineData("0", 0)>]
[<InlineData("1", 1)>]
[<InlineData("10", 10)>]
[<InlineData("1337", 1337)>]
let ``Parse minimal equation`` equation expected =
    parse equation = (Ok (Equation [Operand expected])) |> should be True

[<Fact>]
let ``Parse simple equation`` () =
    parse "5 3 +" = (Ok (Equation [Operand 5; Operand 3; Operator Add])) |> should be True

[<Fact>]
let ``Parse complex equation`` () =
    let expected = Equation [
        Operand 1
        Operand 3
        Operator Add
        Operand 54
        Operator Div
        Operand 8
        Operator Mul
        Operand 4
        Operator Sub
    ]
    parse "1 3 + 54 / 8 * 4 -" = (Ok expected) |> should be True