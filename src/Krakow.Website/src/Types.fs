module Krakow.Website.Types

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
