module Krakow.Core.Tokenizer

open System

type Token =
    | Plus
    | Minus
    | Asterisk
    | Slash
    | Number of int
    
let private (|Int|_|) (str: string) = if Seq.forall Char.IsDigit str then Some (int str) else None

let private words (str: string) = str.Split([|' '|], StringSplitOptions.RemoveEmptyEntries)

let private parseToken token =
    match token with
    | "+"   -> Some Plus
    | "-"   -> Some Minus
    | "*"   -> Some Asterisk
    | "/"   -> Some Slash
    | Int i -> Some (Number i)
    | _     -> None
    
let private parseTokens str =
    str
    |> words
    |> Array.map parseToken

let tokenize str =
    let folder parsedToken parsedTokens =
        match parsedToken, parsedTokens with
        | Some token, Some tokens -> Some (token :: tokens)
        | _ -> None

    let tokens = parseTokens str
    Array.foldBack folder tokens (Some [])