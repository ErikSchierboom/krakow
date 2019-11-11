module Krakow.Core.Tokenizer

open Krakow.Core.Helpers

type TokenizerError =
    | InvalidToken of string

type Token =
    | Plus
    | Minus
    | Asterisk
    | Slash
    | Number of int

type Tokens = Tokens of Token list

let private tokenizeWord word =
    match word with
    | "+" -> Ok Plus
    | "-" -> Ok Minus
    | "*" -> Ok Asterisk
    | "/" -> Ok Slash
    | Int i -> Ok(Number i)
    | _ -> Error(InvalidToken word)

let tokenize (str: string) =
    str
    |> String.words
    |> List.map tokenizeWord
    |> Result.ofList
    |> Result.map Tokens
