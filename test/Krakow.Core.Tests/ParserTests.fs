module Krakow.Core.Tests.ParserTests

open Krakow.Core.Parser
open Krakow.Core.Tests.Helpers

open Expecto
            
[<Tests>]
let tests =
    testList "Parse equation" [
        testPropertyWithConfig config "Empty" <| fun (emptyEquation: EmptyEquation) ->
            Expect.equal (parse (string emptyEquation)) (Error Empty) "Should be Empty"
        
        testPropertyWithConfig config "Unbalanced" <| fun (unbalancedEquation: UnbalancedEquation) ->
            Expect.equal (parse (string unbalancedEquation)) (Error Unbalanced) "Should be Unbalanced"
            
//        testCase "Invalid" <| fun (invalidEquation: Inv) ->
//            Expect.equal (parse "1 +") (Error Invalid _) "Should be Invalid"
  ]

//open Xunit
//open FsUnit.Xunit
//
//open Krakow.Core.Parser
//

//[<Theory>]
//[<InlineData("^", "^")>]
//[<InlineData("1 .", ".")>]
//[<InlineData("1 2 + Z", "Z")>]
//let ``Parse equation with invalid token`` equation invalidToken =
//    parse equation = (Error (Invalid invalidToken)) |> should be True
//
//[<Fact>]
//let ``Parse equation that does not reduce to single number invalid token`` =
//    parse "1 2" = (Error Unbalanced) |> should be True
//
//[<Fact>]
//let ``Parse equation with operator missing operand`` =
//    parse "1 +" = (Error Unbalanced) |> should be True
//
//[<Theory>]
//[<InlineData("0", 0)>]
//[<InlineData("1", 1)>]
//[<InlineData("10", 10)>]
//[<InlineData("1337", 1337)>]
//let ``Parse minimal equation`` equation expected =
//    parse equation = (Ok (Equation [Operand expected])) |> should be True
//
//[<Fact>]
//let ``Parse simple equation`` () =
//    parse "5 3 +" = (Ok (Equation [Operand 5; Operand 3; Operator Add])) |> should be True
//
//[<Fact>]
//let ``Parse complex equation`` () =
//    let expected = Equation [
//        Operand 1
//        Operand 3
//        Operator Add
//        Operand 54
//        Operator Div
//        Operand 8
//        Operator Mul
//        Operand 4
//        Operator Sub
//    ]
//    parse "1 3 + 54 / 8 * 4 -" = (Ok expected) |> should be True