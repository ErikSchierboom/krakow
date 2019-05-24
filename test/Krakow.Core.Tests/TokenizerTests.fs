module Krakow.Core.Tests.TokenizerTests

open Xunit
open FsUnit.Xunit

open Krakow.Core.Tokenizer

[<Fact>]
let ``Tokenize empty equation`` =
    tokenize "" |> should be Empty

[<Theory>]
[<InlineData("^")>]
[<InlineData("-1")>]
[<InlineData("1.")>]
[<InlineData("A")>]
[<InlineData("Z")>]
let ``Tokenize invalid equation`` equation =
    tokenize equation |> should equal [Token.Unknown]

[<Fact>]
let ``Tokenize minimal equation`` () =
    tokenize "5" |> should equal [Token.Number 5]
    
    
[<Fact>]
let ``Tokenize simple equation`` () =
    tokenize "5 3 +" |> should equal [Token.Number 5; Token.Number 3; Token.Plus]

[<Fact>]
let ``Tokenize complex equation`` () =
    let expected = [Token.Number 1; Token.Number 3; Token.Plus; Token.Number 54; Token.Slash; Token.Number 8; Token.Asterisk; Token.Number 4; Token.Minus]
    tokenize "1 3 + 54 / 8 * 4 -" |> should equal expected
