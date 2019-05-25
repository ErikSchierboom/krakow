module Krakow.Core.WebAssembly

open Krakow.Core.Parser

type Type =
    | I32

type Instruction =
    | I32_Const of int
    | I32_Add
    | I32_Sub
    | I32_Mul
    | I32_Div
    
type Export =
    | Export of string

type Parameter =
    | Parameter of Type

type Result =
    | Result of Type

type Function =
    { Parameters: Parameter list
      Results: Result list
      Body: Instruction list
      Export: Export}

type Module =
    | Module of Function list

let private expressionToWebAssemblyInstruction expression =
    match expression with
    | Operand i -> I32_Const i
    | Add -> I32_Add
    | Sub -> I32_Sub
    | Mul -> I32_Mul
    | Div -> I32_Div
    
let private equationToWebAssemblyInstructions equation =
    List.map expressionToWebAssemblyInstruction equation
    
let private equationToWebAssemblyModule equation =
    let function' = 
        { Parameters = []
          Results = [Result Type.I32]
          Body = equationToWebAssemblyInstructions equation
          Export = Export "evaluate"}
    
    Module [function']
    
module Text =
    let private outputType type' =
        match type' with
        | I32 -> "i32"
        
    let private outputParameter (Parameter type') =
        sprintf "(param %s)" (outputType type')
        
    let private outputParameters parameters =
        List.map outputParameter parameters
        
    let private outputResult (Result type') =
        sprintf "(result %s)" (outputType type')
        
    let private outputResults results =
        List.map outputResult results
        
    let private outputInstruction instruction =
        match instruction with
        | I32_Const i -> sprintf "i32.const %i" i
        | I32_Add -> "i32.add"
        | I32_Sub -> "i32.sub"
        | I32_Mul -> "i32.mul"
        | I32_Div -> "i32.div_32"
        
    let private outputBody instructions =
        List.map outputInstruction instructions
        
    let private outputExport (Export name) =
        sprintf "(export \"%s\")" name
        
    let private outputFunction function' =
        let export = outputExport function'.Export
        let parameters = outputParameters function'.Parameters
        let results = outputResults function'.Results
        let body = outputBody function'.Body
        
        export :: parameters @ results @ body
        |> String.concat " "
        |> sprintf "(func %s)"
        
    let private outputFunctions functions =
        List.map outputFunction functions

    let private outputModule (Module functions) =
        outputFunctions functions
        |> String.concat " "
        |> sprintf "(module %s)"
        
    let equationToWebAssemblyText equation =
        equation
        |> parse
        |> Option.map equationToWebAssemblyModule
        |> Option.map outputModule
