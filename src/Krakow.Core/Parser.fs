module Krakow.Core.Parser
//
//open Tokenizer
//
//let private toEquation expressions =
//    Add
////    let rec helper expressions stack =
////        match expressions, stack with
////        | Operand _::xs, _                  -> helper xs (Operand 0::stack)
////        | Add::xs, Operand _::Operand _::ys -> helper xs (Operand 0::ys)
////        | Sub::xs, Operand _::Operand _::ys -> helper xs (Operand 0::ys)
////        | Mul::xs, Operand _::Operand _::ys -> helper xs (Operand 0::ys)
////        | Div::xs, Operand _::Operand _::ys -> helper xs (Operand 0::ys)
////        | [], [_] -> true
////        | _, _-> false
////        
////    helper equation []
//
//let private add = pstring "+" >>% Add
//let private sub = pstring "-" >>% Sub
//let private mul = pstring "*" >>% Mul
//let private div = pstring "/" >>% Div
//
//let private operator = choice [add; sub; mul; div]
//let private operand = pint32 |>> Operand
//
//let private expression = operator <|> operand
//
//let private equation = sepBy1 expression spaces1
//
//let parse str =
//    match tokenize str with
//    | Success(result, _, _) -> Result.Ok result
//    | _ -> Result.Error "Invalid equation"