module Krakow.Website.Helpers

module List =
    let ofOptions options = options |> List.collect Option.toList
