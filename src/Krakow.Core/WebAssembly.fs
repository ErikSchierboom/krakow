module Krakow.Core.WebAssembly

open Krakow.Core.Parser

type ValueType =
    | I32

type NumericInstruction =
    | I32_Const of int
    | I32_Add
    | I32_Sub
    | I32_Mul
    | I32_Div

type Instruction =
    | Numeric of NumericInstruction

type FunctionType =
    { ParameterTypes: ValueType list
      ResultTypes: ValueType list }
    
type TypeSection =
    { Types: FunctionType list }
    
type Export =
    { Name: string
      FunctionIndex: int }
    
type ExportSection =
    { Exports: Export list }
    
type CodeSection =
    { Code: Instruction list }

type Section =
    | Type of TypeSection
    | Export of ExportSection
    | Code of CodeSection

type Module =
    { Sections: Section list }

let private expressionToWebAssemblyInstruction expression =
    match expression with
    | Operand i -> I32_Const i |> Numeric
    | Add -> I32_Add |> Numeric
    | Sub -> I32_Sub |> Numeric
    | Mul -> I32_Mul |> Numeric
    | Div -> I32_Div |> Numeric

let private equationToWebAssemblyInstructions (Equation expressions) =
    List.map expressionToWebAssemblyInstruction expressions

let private equationToWebAssemblyModule equation =
    { Sections =
        [ Type { Types = [ { ParameterTypes = [I32; I32]; ResultTypes = [I32] } ] }
          Export { Exports = [ { Name = "evaluate"; Description = Function } ] }
          Code { Code = equationToWebAssemblyInstructions equation } ] }

module Text =
    let private outputValueType valueType =
        match valueType with
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
        
    let private outputSection section =
        match section with
        | Type typeSection ->
        | Export exportSection ->
        | Code codeSection ->

    let private outputModule module' =
        module'.Sections
        |> outputSection
        |> String.concat " "
        |> sprintf "(module %s)"
        
    let equationToWebAssemblyText equation =
        equation
        |> parse
        |> Option.map equationToWebAssemblyModule
        |> Option.map outputModule

module Binary =
    type SectionIndex =
        | Type
        | Function
        | Export
        | Code

    let outputUnsignedLEB128 i =
        let rec helper bytes n =
            let byte = n &&& 0x7f
            let nextSevenBits = n >>> 7
            let correctedByte = if nextSevenBits <> 0 then byte ||| 0x80 else byte
            let bytesWithCorrectedByte = correctedByte :: bytes
            
            if nextSevenBits = 0 then
                List.rev bytesWithCorrectedByte
            else
                helper bytesWithCorrectedByte nextSevenBits
                
        helper [] i

    let outputVector bytes =
        let length = bytes |> List.length |> outputUnsignedLEB128
        length @ bytes

    let outputMagic = [0x00; 0x61; 0x73; 0x6D]

    let outputVersion = [0x01; 0x00; 0x00; 0x00]

    let outputString (str: string) =
        str
        |> Seq.map int
        |> Seq.toList
        |> outputVector

    let outputType type' =
        match type' with
        | I32 -> 0x7F

    let outputSectionIndex index =
        match index with
        | SectionIndex.Type     -> 0
        | SectionIndex.Function -> 3
        | SectionIndex.Export   -> 7
        | SectionIndex.Code     -> 10
    
    let outputSection sectionIndex bytes =
        outputSectionIndex sectionIndex :: bytes
        |> outputVector

    let outputParameter (Parameter type') =
        outputType type'

    let outputParameters parameters =
        parameters
        |> List.map outputParameter
        |> outputVector

    let outputResult (Result type') =
        outputType type'

    let outputResults results =
        results
        |> List.map outputResult
        |> outputVector

    let outputFunctionTypes function' =
        let parameters = outputParameters function'.Parameters
        let results = outputResults function'.Results
        0x60 :: parameters @ results

    let outputTypeSection functions =
         let functionTypes = List.collect outputFunctionTypes functions
         let functionCount = List.length functions |> outputUnsignedLEB128

         functionCount @ functionTypes
         |> outputSection SectionIndex.Type

    let outputInstruction instruction =
        match instruction with
        | I32_Const i -> [0x41; i]
        | I32_Add -> [0x6a]
        | I32_Sub -> [0x6b]
        | I32_Mul -> [0x6c]
        | I32_Div -> [0x6e]
        
    let outputBody instructions =
        List.map outputInstruction instructions
        
    let outputExport (Export.Export name) =
        []
        
    let outputFunction function' =
        []
//        let export = outputExport function'.Export
//        let parameters = outputParameters function'.Parameters
//        let results = outputResults function'.Results
//        let body = outputBody function'.Body
//        
//        export :: parameters @ results @ body
//        |> String.concat " "
//        |> sprintf "(func %s)"
        
    let outputFunctions functions =
        List.collect outputFunction functions

    let outputModule (Module functions) =
        outputMagic @ outputVersion
//        outputFunctions functions
//        |> String.concat " "
//        |> sprintf "(module %s)"
        
    let equationToWebAssemblyBinary equation =
        equation
        |> parse
        |> Option.map equationToWebAssemblyModule
        |> Option.map outputModule
