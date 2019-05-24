module Krakow.Core.Tokenizer

open System

type Token =
    | Unknown
    | Plus
    | Minus
    | Asterisk
    | Slash
    | Number of int
    
let private (|Int|_|) (str: string) =
   if Seq.forall Char.IsDigit str then Some (int str) else None
    
let private parseToken token =
    match token with
    | "+"   -> Plus
    | "-"   -> Minus
    | "*"   -> Asterisk
    | "/"   -> Slash
    | Int i -> Number i
    | _     -> Unknown

let tokenize (str: string) =
    str.Split([|' '|], StringSplitOptions.RemoveEmptyEntries)
    |> Array.map parseToken
    |> Array.toList