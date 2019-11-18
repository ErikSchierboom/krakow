module Krakow.Website.Model

open Elmish

type Evaluation =
    { result: int
      wasm: int list
      wat: string }

type EvaluationError =
    | WebAssemblyException of exn
    | EmptyEquation
    | InvalidEquation of string
    | UnbalancedEquation

type Model =
    { evaluation: Result<Evaluation, EvaluationError> option
      equation: string }

type Msg =
    | UpdateEquation of string
    | EvaluateEquation
    | EquationEvaluatedSuccessfully of Evaluation
    | EquationEvaluatedWithError of EvaluationError

let init() =
    { evaluation = None
      equation = "" }, Cmd.none
