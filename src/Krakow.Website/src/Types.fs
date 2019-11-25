module Krakow.Website.Types

type EvaluationResult =
    { Result: int
      Wasm: int list
      Wat: string }

type EvaluationError =
    | WebAssemblyException of exn
    | EmptyEquation
    | InvalidEquation of string
    | UnbalancedEquation

type Model =
    { Evaluation: Result<EvaluationResult, EvaluationError> option
      Equation: string }

type Msg =
    | UpdateEquation of string
    | EvaluateEquation
    | EquationEvaluatedSuccessfully of EvaluationResult
    | EquationEvaluatedWithError of EvaluationError
