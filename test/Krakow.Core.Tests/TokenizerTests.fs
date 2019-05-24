module Krakow.Core.Tests.TokenizerTests

open Xunit

open Krakow.Core.Tokenizer

[<Theory>]
[<InlineData("")>]
[<InlineData("^")>]
[<InlineData("Z")>]
let ``Tokenize invalid equation`` equation =
    Assert.Equal(None, tokenize equation)

[<Fact>]
let ``Tokenize minimal equation`` () =
    Assert.Equal(Some [Token.Number 5], tokenize "5")
    
    
[<Fact>]
let ``Tokenize simple equation`` () =
    Assert.Equal(Some [Token.Number 5; Token.Number 3; Token.Add], tokenize "5 3 +")

[<Fact>]
let ``Tokenize complex equation`` () =
    let equation = [Token.Number 1; Token.Number 3; Token.Add; Token.Number 54; Token.Div; Token.Number 8; Token.Mul; Token.Number 4; Token.Sub]
    Assert.Equal(Some equation, tokenize "1 3 + 54 / 8 * 4 -")
