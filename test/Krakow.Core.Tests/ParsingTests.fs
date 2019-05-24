module Krakow.Core.Tests.ParserTests
//
//open Xunit
//
//open Krakow.Core.Domain
//open Krakow.Core.Parsing
//
//[<Theory>]
//[<InlineData("+")>]
//[<InlineData("1 2")>]
//[<InlineData("1 +")>]
//[<InlineData("1 2 + -")>]
//[<InlineData("1 2 - 3 * 4 5 /")>]
//let ``Parse invalid equation`` equation =
//    Assert.Equal(Result.Error "Invalid equation", parse equation)
//
//[<Fact>]
//let ``Parse minimal equation`` () =
//    Assert.Equal([Expression.Operand 5] |> Result.Ok, parse "5")
//    
//    
//[<Fact>]
//let ``Parse simple equation`` () =
//    Assert.Equal([Expression.Operand 5; Expression.Operand 3; Expression.Add] |> Result.Ok, parse "5 3 +")
//
//[<Fact>]
//let ``Parse complex equation`` () =
//    let equation = [
//        Expression.Operand 1
//        Expression.Operand 3
//        Expression.Add
//        Expression.Operand 54
//        Expression.Div
//        Expression.Operand 8
//        Expression.Mul
//        Expression.Operand 4
//        Expression.Sub
//    ]
//    Assert.Equal(equation |> Result.Ok, parse "1 3 + 54 / 8 * 4 -")
