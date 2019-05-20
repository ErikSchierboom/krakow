[<AutoOpen>]
module Krakow.Core.Domain

type Expression =
    | Operand of int
    | Add
    | Sub
    | Mul
    | Div

    override this.ToString() =
        match this with
        | Operand i -> string i
        | Add -> "+"
        | Sub -> "-"
        | Mul -> "*"
        | Div -> "/"
    
type Equation =
    | Equation of Expression list

    override this.ToString() =
        match this with
        | Equation expressions ->
            expressions
            |> List.map string 
            |> String.concat " "