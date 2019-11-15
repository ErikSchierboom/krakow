module Krakow.Website.Model

type Evaluation =
    { result: int
      wasm: int list
      wat: string }

type EvaluationError =
    | InvalidWebAssembly
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
