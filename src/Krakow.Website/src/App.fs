module Krakow.Website.App

open Elmish
open Elmish.React

open Krakow.Website.State
open Krakow.Website.View

let placeHolderId = "app"

Program.mkProgram init update view
|> Program.withReactSynchronous placeHolderId
|> Program.withConsoleTrace
|> Program.run
