module Krakow.Core.WebAssembly

open Krakow.Core.Domain
open Krakow.Core.Parser

type WebAssemblyText = WebAssemblyText of string

type WebAssemblyBinary = WebAssemblyBinary of int list

type WebAssembly =
    { Text: WebAssemblyText
      Binary: WebAssemblyBinary }

type WebAssemblyValueType = I32

type WebAssemblyInstruction =
    | I32Const of int
    | I32Add
    | I32Sub
    | I32Mul
    | I32Div

type WebAssemblyFunction =
    { Result: WebAssemblyValueType
      Body: WebAssemblyInstruction list
      Name: string }

type WebAssemblyModule =
    { Function: WebAssemblyFunction }

module Text =
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

    let outputModule module' =
        outputFunction module'.Function
        |> sprintf "(module %s)"
        |> WebAssemblyText

module Binary =
    type Section =
        | Type
        | Function
        | Export
        | Code

    type ExportType = FunctionExport

    let outputUnsignedLEB128 i =
        let rec encodedGroups acc remainder =
            let bitEightMask = 0b1000_0000
            let bitsOneToSevenMask = 0b0111_1111

            let group = remainder &&& bitsOneToSevenMask
            let updatedRemainder = remainder >>> 7

            let correctedGroup =
                if updatedRemainder = 0 then group
                else group ||| bitEightMask

            let updatedAcc = correctedGroup :: acc

            if updatedRemainder = 0 then updatedAcc
            else encodedGroups updatedAcc updatedRemainder

        encodedGroups [] i |> List.rev

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
        | Type -> 0x01
        | Function -> 0x03
        | Export -> 0x07
        | Code -> 0x0a

    let private outputSection section bytes = outputSectionIndex section :: outputVector bytes

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
        | I32Const i -> 0x41 :: outputInteger i
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

    let outputModule module' =
        let typeSection = outputTypeSection module'.Function
        let functionSection = outputFunctionSection
        let exportSection = outputExportSection module'.Function
        let codeSection = outputCodeSection module'.Function

        outputMagicHeader @ outputVersion @ typeSection @ functionSection @ exportSection @ codeSection
        |> WebAssemblyBinary

let private expressionToWebAssemblyInstruction expression =
    match expression with
    | OperandExpression(Integer i) -> I32Const i
    | OperatorExpression Add -> I32Add
    | OperatorExpression Sub -> I32Sub
    | OperatorExpression Mul -> I32Mul
    | OperatorExpression Div -> I32Div

let private equationToWebAssemblyCode (Equation expressions) = List.map expressionToWebAssemblyInstruction expressions

let private equationToWebAssemblyModule equation =
    { Function =
          { Result = I32
            Body = equationToWebAssemblyCode equation
            Name = "evaluate" } }

let equationToWebAssembly equation =
    let webAssemblyModule = equationToWebAssemblyModule equation

    { Text = Text.outputModule webAssemblyModule
      Binary = Binary.outputModule webAssemblyModule }

let convert str = parse str |> Result.map equationToWebAssembly
