module Krakow.Core.Tests.WebAssemblyTests

open Expecto
open FsCheck
open Krakow.Core.Domain
open Krakow.Core.WebAssembly
open Krakow.Core.Tests.PropertyBasedTesting

module Text =

    let convertToWebAssemblyText equation = convert equation |> Result.map (fun webAssembly -> webAssembly.Text)





    [<Tests>]
    let tests =
        testList "Convert to WebAssembly text"
            [ testPropertyWithConfig config "Empty"
              <| fun (EmptyEquation emptyEquation) ->
                  Expect.equal (convertToWebAssemblyText emptyEquation) (Error Empty) "Should be empty"

              testPropertyWithConfig config "Unbalanced"
              <| fun (UnbalancedEquation unbalancedEquation) ->
                  Expect.equal (convertToWebAssemblyText unbalancedEquation) (Error Unbalanced) "Should be unbalanced"

              testPropertyWithConfig config "Invalid"
              <| fun (InvalidEquation(invalidEquation, invalidToken)) ->
                  Expect.equal (convertToWebAssemblyText invalidEquation) (Error(Invalid invalidToken))
                      "Should be invalid"

              testPropertyWithConfig config "Valid"
              <| fun (ValidEquation validEquation) ->
                  Expect.isOk (convertToWebAssemblyText validEquation) "Should be valid"

              testPropertyWithConfig config "Single operand"
              <| fun (number: PositiveInt) ->
                  let expected = sprintf "(module (func (export \"evaluate\") (result i32) i32.const %i))" number.Get
                  Expect.equal (convertToWebAssemblyText (string number.Get)) (Ok(WebAssemblyText expected))
                      "Should convert single operand to WAT"

              testCase "All expression types"
              <| fun () ->
                  let expected =
                      "(module " + "(func (export \"evaluate\") (result i32) " + "i32.const 15 " + "i32.const 7 "
                      + "i32.const 1 " + "i32.const 1 " + "i32.add " + "i32.sub " + "i32.div_32 " + "i32.const 3 "
                      + "i32.mul " + "i32.const 2 " + "i32.const 1 " + "i32.const 1 " + "i32.add " + "i32.add "
                      + "i32.sub))"
                  let equation = "15 7 1 1 + - / 3 * 2 1 1 + + -"
                  Expect.equal (convertToWebAssemblyText equation) (Ok(WebAssemblyText expected))
                      "Should convert successfully to WAT" ]

module Binary =
    open Krakow.Core.WebAssembly.Binary

    let convertToWebAssemblyBinary equation = convert equation |> Result.map (fun webAssembly -> webAssembly.Binary)





    [<Tests>]
    let tests =
        testList "Convert to WebAssembly binary"
            [ testPropertyWithConfig config "Empty"
              <| fun (EmptyEquation emptyEquation) ->
                  Expect.equal (convertToWebAssemblyBinary emptyEquation) (Error Empty) "Should be empty"

              testPropertyWithConfig config "Unbalanced"
              <| fun (UnbalancedEquation unbalancedEquation) ->
                  Expect.equal (convertToWebAssemblyBinary unbalancedEquation) (Error Unbalanced) "Should be unbalanced"

              testPropertyWithConfig config "Invalid"
              <| fun (InvalidEquation(invalidEquation, invalidToken)) ->
                  Expect.equal (convertToWebAssemblyBinary invalidEquation) (Error(Invalid invalidToken))
                      "Should be invalid"

              testPropertyWithConfig config "Valid"
              <| fun (ValidEquation validEquation) ->
                  Expect.isOk (convertToWebAssemblyBinary validEquation) "Should be valid"

              testPropertyWithConfig config "Single operand"
              <| fun () ->
                  let expected =
                      [ 0x00
                        0x61
                        0x73
                        0x6d
                        0x01
                        0x00
                        0x00
                        0x00
                        0x01
                        0x05
                        0x01
                        0x60
                        0x00
                        0x01
                        0x7f
                        0x03
                        0x02
                        0x01
                        0x00
                        0x07
                        0x0c
                        0x01
                        0x08
                        0x65
                        0x76
                        0x61
                        0x6c
                        0x75
                        0x61
                        0x74
                        0x65
                        0x00
                        0x00
                        0x0a
                        0x06
                        0x01
                        0x04
                        0x00
                        0x41
                        0x02
                        0x0b ]
                  Expect.equal (convertToWebAssemblyBinary "2") (Ok(WebAssemblyBinary expected))
                      "Should convert single operand to WASM"

              testCase "All expression types"
              <| fun () ->
                  let expected =
                      [ 0x00
                        0x61
                        0x73
                        0x6d
                        0x01
                        0x00
                        0x00
                        0x00
                        0x01
                        0x05
                        0x01
                        0x60
                        0x00
                        0x01
                        0x7f
                        0x03
                        0x02
                        0x01
                        0x00
                        0x07
                        0x0c
                        0x01
                        0x08
                        0x65
                        0x76
                        0x61
                        0x6c
                        0x75
                        0x61
                        0x74
                        0x65
                        0x00
                        0x00
                        0x0A
                        0x1B
                        0x01
                        0x19
                        0x00
                        0x41
                        0x0F
                        0x41
                        0x07
                        0x41
                        0x01
                        0x41
                        0x01
                        0x6A
                        0x6B
                        0x6E
                        0x41
                        0x03
                        0x6C
                        0x41
                        0x02
                        0x41
                        0x01
                        0x41
                        0x01
                        0x6A
                        0x6A
                        0x6B
                        0x0B ]
                  let equation = "15 7 1 1 + - / 3 * 2 1 1 + + -"
                  Expect.equal (convertToWebAssemblyBinary equation) (Ok(WebAssemblyBinary expected))
                      "Should convert successfully to WASM" ]
