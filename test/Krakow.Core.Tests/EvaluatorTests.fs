module Krakow.Core.Tests.EvaluatorTests

open Krakow.Core.Evaluator
open Krakow.Core.Domain
open Krakow.Core.Tests.PropertyBasedTesting

open Expecto
open FsCheck

[<Tests>]
let tests =
    testList "Evaluate equation"
        [ testPropertyWithConfig config "Empty"
          <| fun (EmptyEquation emptyEquation) ->
              Expect.equal (evaluate emptyEquation) (Error Empty) "Should be empty"

          testPropertyWithConfig config "Unbalanced"
          <| fun (UnbalancedEquation unbalancedEquation) ->
              Expect.equal (evaluate unbalancedEquation) (Error Unbalanced) "Should be unbalanced"

          testPropertyWithConfig config "Invalid"
          <| fun (InvalidEquation(invalidEquation, invalidToken)) ->
              Expect.equal (evaluate invalidEquation) (Error(Invalid invalidToken)) "Should be invalid"

          testPropertyWithConfig config "Valid"
          <| fun (ValidEquation validEquation) -> Expect.isOk (evaluate validEquation) "Should be valid"

          testPropertyWithConfig config "Single operand"
          <| fun (number: PositiveInt) ->
              Expect.equal (evaluate (string number.Get)) (Ok number.Get) "Should evaluate to input operand"

          testCase "All expression types"
          <| fun () -> Expect.equal (evaluate "1 3 + 36 + 4 / 8 * 5 -") (Ok 75) "Should evaluate successfully" ]
