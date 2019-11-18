module Krakow.Core.Tests.Helpers

module List =
    open System

    let private random = Random()

    let splitAtRandom list =
        let randomIndex = random.Next(List.length list)
        List.splitAt randomIndex list

    let insertAtRandom value list =
        let (before, after) = splitAtRandom list
        before @ value :: after

    let deleteAtRandom list =
        let (before, after) = splitAtRandom list

        match before, after with
        | [], [] -> []
        | _, [] -> List.tail before
        | [], _ -> List.tail after
        | _, _ :: xs -> before @ xs
