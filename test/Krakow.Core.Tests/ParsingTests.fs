module Krakow.Core.Tests.ParsingTests

open Xunit

open Krakow.Core.Domain
open Krakow.Core.Parsing

[<Fact>]
let ``Parse empty equation`` () =
    Assert.Equal(Result.Error "Error parsing equation", parseEquation "")

[<Fact>]
let ``Parse single operand equation`` () =
    Assert.Equal(Equation [Expression.Operand 5] |> Result.Ok, parseEquation "5")

[<Fact>]
let ``Parse single add operator equation`` () =
    Assert.Equal(Equation [Expression.Add] |> Result.Ok, parseEquation "+")

[<Fact>]
let ``Parse single sub operator equation`` () =
    Assert.Equal(Equation [Expression.Sub] |> Result.Ok, parseEquation "-")

[<Fact>]
let ``Parse single mul operator equation`` () =
    Assert.Equal(Equation [Expression.Mul] |> Result.Ok, parseEquation "*")

[<Fact>]
let ``Parse single div operator equation`` () =
    Assert.Equal(Equation [Expression.Div] |> Result.Ok, parseEquation "/")

[<Fact>]
let ``Parse complex equation`` () =
    let equation = Equation [
        Expression.Operand 1
        Expression.Operand 3
        Expression.Add
        Expression.Operand 54
        Expression.Div
        Expression.Mul
        Expression.Operand 8
        Expression.Sub
    ]
    Assert.Equal(equation |> Result.Ok, parseEquation "1 3 + 54 / * 8 -")
