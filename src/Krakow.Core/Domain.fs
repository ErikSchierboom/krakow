[<AutoOpen>]
module Krakow.Core.Domain

type Expression =
    | Operand of int
    | Add
    | Sub
    | Mul
    | Div
    
type Equation =
    | Equation of Expression list