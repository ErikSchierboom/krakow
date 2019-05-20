module Krakow.Core.Tests.DomainTests

open Xunit

open Krakow.Core.Domain

[<Fact>]
let ``Empty equation to string`` () =
    Assert.Equal("", Equation [] |> string)

[<Fact>]
let ``Single operand equation to string`` () =
    Assert.Equal("5", Equation [Expression.Operand 5] |> string)

[<Fact>]
let ``Single add operator equation to string`` () =
    Assert.Equal("+", Equation [Expression.Add] |> string)

[<Fact>]
let ``Single sub operator equation to string`` () =
    Assert.Equal("-", Equation [Expression.Sub] |> string)

[<Fact>]
let ``Single mul operator equation to string`` () =
    Assert.Equal("*", Equation [Expression.Mul] |> string)

[<Fact>]
let ``Single div operator equation to string`` () =
    Assert.Equal("/", Equation [Expression.Div] |> string)

[<Fact>]
let ``Complex equation to string`` () =
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
    Assert.Equal("1 3 + 54 / * 8 -", equation |> string)
