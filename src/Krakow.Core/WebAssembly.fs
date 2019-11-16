module Krakow.Core.WebAssembly

open Krakow.Core.Parser

type ValueType = I32

type Instruction =
    | I32Const of int
    | I32Add
    | I32Sub
    | I32Mul
    | I32Div

type Function =
    { Result: ValueType
      Body: Instruction list
      Name: string }

type Module =
    { Function: Function }

let private expressionToWebAssemblyInstruction expression =
    match expression with
    | Operand i -> I32Const i
    | Operator Add -> I32Add
    | Operator Sub -> I32Sub
    | Operator Mul -> I32Mul
    | Operator Div -> I32Div

let private equationToWebAssemblyCode (Equation expressions) = List.map expressionToWebAssemblyInstruction expressions

let private equationToWebAssemblyModule equation =
    { Function =
          { Result = I32
            Body = equationToWebAssemblyCode equation
            Name = "evaluate" } }

module Text =
    type WebAssemblyText = WebAssemblyText of string

    let private outputValueType valueType =
        match valueType with
        | I32 -> "i32"

    let private outputResult resultType = sprintf "(result %s)" (outputValueType resultType)

    let private outputInstruction instruction =
        match instruction with
        | I32Const i -> sprintf "i32.const %i" i
        | I32Add -> "i32.add"
        | I32Sub -> "i32.sub"
        | I32Mul -> "i32.mul"
        | I32Div -> "i32.div_32"

    let private outputBody instructions =
        instructions
        |> List.map outputInstruction
        |> String.concat " "

    let private outputExport name = sprintf "(export \"%s\")" name

    let private outputFunction function' =
        let export = outputExport function'.Name
        let result = outputResult function'.Result
        let body = outputBody function'.Body
        sprintf "(func %s %s %s)" export result body

    let private outputModule module' = sprintf "(module %s)" (outputFunction module'.Function)

    let equationToWebAssemblyText equation =
        equation
        |> equationToWebAssemblyModule
        |> outputModule
        |> WebAssemblyText

module Binary =
    type WebAssemblyBinary = WebAssemblyBinary of int list

    type Section =
        | Type
        | Function
        | Export
        | Code

    type ExportType = FunctionExport

    let private outputUnsignedLEB128 i =
        let rec helper bytes n =
            let byte = n &&& 0x7f
            let nextSevenBits = n >>> 7

            let correctedByte =
                if nextSevenBits <> 0 then byte ||| 0x80
                else byte

            let bytesWithCorrectedByte = correctedByte :: bytes

            if nextSevenBits = 0 then List.rev bytesWithCorrectedByte
            else helper bytesWithCorrectedByte nextSevenBits

        helper [] i

    let private outputInteger i = outputUnsignedLEB128 i

    let private outputVector bytes =
        let length =
            bytes
            |> List.length
            |> outputInteger
        length @ bytes

    let private outputMagicHeader = [ 0x00; 0x61; 0x73; 0x6d ]

    let private outputVersion = [ 0x01; 0x00; 0x00; 0x00 ]

    let private outputString (str: string) =
        str
        |> Seq.map int
        |> Seq.toList
        |> outputVector

    let private outputValueType valueType =
        match valueType with
        | I32 -> 0x7f

    let private outputSectionIndex section =
        match section with
        | Type -> outputInteger 0x01
        | Function -> outputInteger 0x03
        | Export -> outputInteger 0x07
        | Code -> outputInteger 0x0a

    let private outputSection section bytes = outputSectionIndex section @ outputVector bytes

    let outputResult resultType = outputValueType resultType

    let private outputFunctionTypes function' =
        let parametersCount = outputInteger 0x00
        let resultsCount = outputInteger 0x01
        let resultType = outputResult function'.Result
        let functionType = 0x60

        functionType :: parametersCount @ resultsCount @ [ resultType ]

    let private outputTypeSection function' =
        let functionCount = outputUnsignedLEB128 0x01
        let functionTypes = outputFunctionTypes function'

        functionCount @ functionTypes |> outputSection Section.Type

    let private outputFunctionSection =
        let functionsCount = outputInteger 0x01
        let firstFunctionSignatureIndex = outputInteger 0x00

        functionsCount @ firstFunctionSignatureIndex |> outputSection Section.Function

    let private outputExportType exportType =
        match exportType with
        | FunctionExport -> 0x00

    let private outputExportSection function' =
        let exportsCount = outputInteger 0x01
        let exportName = outputString function'.Name
        let exportType = outputExportType FunctionExport
        let exportFunctionIndex = outputInteger 0x00

        exportsCount @ exportName @ [ exportType ] @ exportFunctionIndex |> outputSection Export

    let private outputInstruction instruction =
        match instruction with
        | I32Const i -> [ 0x41; i ]
        | I32Add -> [ 0x6a ]
        | I32Sub -> [ 0x6b ]
        | I32Mul -> [ 0x6c ]
        | I32Div -> [ 0x6e ]

    let private outputBody instructions =
        let localDeclarationsCount = outputInteger 0x00
        let instructions = List.collect outputInstruction instructions
        let bodyEnd = 0x0b

        localDeclarationsCount @ instructions @ [ bodyEnd ] |> outputVector

    let private outputCodeSection function' =
        let functionCount = outputInteger 0x01
        let body = outputBody function'.Body

        functionCount @ body |> outputSection Code

    let private outputModule module' =
        let typeSection = outputTypeSection module'.Function
        let functionSection = outputFunctionSection
        let exportSection = outputExportSection module'.Function
        let codeSection = outputCodeSection module'.Function

        outputMagicHeader @ outputVersion @ typeSection @ functionSection @ exportSection @ codeSection

    let equationToWebAssemblyBinary equation =
        equation
        |> equationToWebAssemblyModule
        |> outputModule
        |> WebAssemblyBinary
