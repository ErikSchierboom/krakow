module Krakow.Core.Parsing

open FParsec

let private add = pstring "+" >>% Add
let private sub = pstring "-" >>% Sub
let private mul = pstring "*" >>% Mul
let private div = pstring "/" >>% Div

let private operator = choice [add; sub; mul; div]
let private operand = pint32 |>> Operand

let private expression = operator <|> operand

let private equation = sepBy1 expression spaces1 |>> Equation

let parse str =
    match run equation str with
    | Success(result, _, _) -> Result.Ok result
    | Failure(_, _, _)      -> Result.Error "Invalid equation"