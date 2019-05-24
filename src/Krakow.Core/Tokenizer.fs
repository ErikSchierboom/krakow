module Krakow.Core.Tokenizer

open FParsec

type Token =
    | Number of int
    | Add
    | Sub
    | Mul
    | Div

let private add = pstring "+" >>% Add
let private sub = pstring "-" >>% Sub
let private mul = pstring "*" >>% Mul
let private div = pstring "/" >>% Div

let private operand = pint32 |>> Number
let private operator = choice [add; sub; mul; div]

let private token = operator <|> operand
let private tokens = sepBy1 token spaces1

let tokenize str =
    match run tokens str with
    | Success(result, _, _) -> Some result
    | _ -> None