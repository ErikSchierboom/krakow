module Krakow.Core.Tests.WebAssemblyTests

open Xunit
open FsUnit.Xunit

open Krakow.Core.WebAssembly.Text

[<Theory>]
[<InlineData("+")>]
[<InlineData("1 2")>]
[<InlineData("1 +")>]
[<InlineData("1 2 + -")>]
[<InlineData("1 2 - 3 * 4 5 /")>]
let ``Convert invalid equation to WebAssembly text`` equation =
    equationToWebAssemblyText equation |> should equal None

[<Fact>]
let ``Convert single operand equation to WebAssembly text`` () =
    let expected = "(module " +
                     "(func (export \"evaluate\") (result i32) " +
                        "i32.const 1))"
    equationToWebAssemblyText "1" |> should equal (Some expected)

[<Fact>]
let ``Convert single operator equation to WebAssembly text`` () =
   let expected = "(module " +
                    "(func (export \"evaluate\") (result i32) " +
                        "i32.const 9 " +
                        "i32.const 8 " +
                        "i32.sub))"
   equationToWebAssemblyText "9 8 -" |> should equal (Some expected)

[<Fact>]
let ``Convert complex equation to WebAssembly text`` () =
    let expected = "(module " +
                    "(func (export \"evaluate\") (result i32) " +
                       "i32.const 15 " +
                       "i32.const 7 " +
                       "i32.const 1 " +
                       "i32.const 1 " +
                       "i32.add " +
                       "i32.sub " +
                       "i32.div_32 " +
                       "i32.const 3 " +
                       "i32.mul " +
                       "i32.const 2 " +
                       "i32.const 1 " +
                       "i32.const 1 " +
                       "i32.add " +
                       "i32.add " +
                       "i32.sub))"
    equationToWebAssemblyText "15 7 1 1 + - / 3 * 2 1 1 + + -" |> should equal (Some expected)