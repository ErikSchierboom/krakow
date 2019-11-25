module Krakow.Website.Types

type Evaluation =
    { Result: int
      Wasm: int list
      Wat: string }

type EvaluationError =
    | WebAssemblyException of exn
    | EmptyEquation
    | InvalidEquation of string
    | UnbalancedEquation

type Model =
    { Evaluation: Result<Evaluation, EvaluationError> option
      Equation: string }

type Msg =
    | UpdateEquation of string
    | EvaluateEquation
    | EquationEvaluatedSuccessfully of Evaluation
    | EquationEvaluatedWithError of EvaluationError
