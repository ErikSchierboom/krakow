module Krakow.Core.Tests.TokenizerTests

open Xunit
open FsUnit.Xunit

open Krakow.Core.Tokenizer

[<Theory>]
[<InlineData("")>]
[<InlineData("  ")>]
[<InlineData("\t\n")>]
[<InlineData("\r\n")>]
let ``Tokenize empty equation`` equation =
    tokenize equation = Ok (Tokens []) |> should be True

[<Theory>]
[<InlineData("^", "^")>]
[<InlineData("1 .", ".")>]
[<InlineData("1 2 + Z", "Z")>]
let ``Tokenize invalid token`` equation invalidToken =
    tokenize equation = (Error (InvalidToken invalidToken)) |> should be True

[<Fact>]
let ``Tokenize minimal equation`` () =
    tokenize "5" = (Ok (Tokens [Number 5])) |> should be True
    
[<Fact>]
let ``Tokenize simple equation`` () =
    tokenize "5 3 +" = (Ok (Tokens [Number 5; Number 3; Plus])) |> should be True

[<Fact>]
let ``Tokenize complex equation`` () =
    let expected = Ok(Tokens [Number 1; Number 3; Plus; Number 54; Slash; Number 8; Asterisk; Number 4; Minus])
    tokenize "1 3 + 54 / 8 * 4 -" = expected |> should be True
