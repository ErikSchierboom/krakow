module Krakow.Core.Tests.WebAssemblyTests

open Xunit
open FsUnit.Xunit

module Text =
    
    open Krakow.Core.WebAssembly.Text
    
    [<Theory>]
    [<InlineData("")>]
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

module Binary =
    
    open Krakow.Core.WebAssembly.Binary

    [<Theory>]
    [<InlineData("+")>]
    [<InlineData("1 2")>]
    [<InlineData("1 +")>]
    [<InlineData("1 2 + -")>]
    [<InlineData("1 2 - 3 * 4 5 /")>]
    let ``Convert invalid equation to WebAssembly binary`` equation =
        equationToWebAssemblyBinary equation |> should equal None

    [<Fact>]
    let ``Convert single operand equation to WebAssembly binary`` () =
        let expected =
            [0x00; 0x61; 0x73; 0x6d; 0x01; 0x00; 0x00; 0x00; 0x01; 0x05;
             0x01; 0x60; 0x00; 0x01; 0x7f; 0x03; 0x02; 0x01; 0x00; 0x07;
             0x0c; 0x01; 0x08; 0x65; 0x76; 0x61; 0x6c; 0x75; 0x61; 0x74;
             0x65; 0x00; 0x00; 0x0a; 0x06; 0x01; 0x04; 0x00; 0x41; 0x02;
             0x0b]
        equationToWebAssemblyBinary "2" |> should equal (Some expected)

    [<Fact>]
    let ``Convert single operator equation to WebAssembly binary`` () =
       let expected =
             [0x00; 0x61; 0x73; 0x6d; 0x01; 0x00; 0x00; 0x00; 0x01; 0x05;
              0x01; 0x60; 0x00; 0x01; 0x7f; 0x03; 0x02; 0x01; 0x00; 0x07;
              0x0c; 0x01; 0x08; 0x65; 0x76; 0x61; 0x6c; 0x75; 0x61; 0x74;
              0x65; 0x00; 0x00; 0x0A; 0x09; 0x01; 0x07; 0x00; 0x41; 0x09;
              0x41; 0x08; 0x6B; 0x0B]
       equationToWebAssemblyBinary "9 8 -" |> should equal (Some expected)

    [<Fact>]
    let ``Convert complex equation to WebAssembly binary`` () =
        let expected =
              [0x00; 0x61; 0x73; 0x6d; 0x01; 0x00; 0x00; 0x00; 0x01; 0x05;
               0x01; 0x60; 0x00; 0x01; 0x7f; 0x03; 0x02; 0x01; 0x00; 0x07;
               0x0c; 0x01; 0x08; 0x65; 0x76; 0x61; 0x6c; 0x75; 0x61; 0x74;
               0x65; 0x00; 0x00; 0x0A; 0x1B; 0x01; 0x19; 0x00; 0x41; 0x0F;
               0x41; 0x07; 0x41; 0x01; 0x41; 0x01; 0x6A; 0x6B; 0x6E; 0x41;
               0x03; 0x6C; 0x41; 0x02; 0x41; 0x01; 0x41; 0x01; 0x6A; 0x6A;
               0x6B; 0x0B]
        equationToWebAssemblyBinary "15 7 1 1 + - / 3 * 2 1 1 + + -" |> should equal (Some expected)