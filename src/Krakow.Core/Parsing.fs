module Krakow.Core.Parsing

open System.Text.RegularExpressions

let private parseExpression (str: string) =
    match str with
    | "+" -> Add
    | "-" -> Sub
    | "*" -> Mul
    | "/" -> Div
    | _   -> Operand (int str)

let private parseExpressions (str: string) =
    Regex.Matches(str, "(\d+|\+|\-|\/|\*)")
    |> Seq.cast<Match>
    |> Seq.map (fun m -> m.Value)
    |> Seq.map parseExpression
    |> Seq.toList

let parse str =
    match parseExpressions str with
    | [] -> Result.Error "Error parsing equation"
    | xs -> Result.Ok (Equation xs) 