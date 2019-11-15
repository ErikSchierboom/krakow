module Krakow.Website.App

open Elmish
open Elmish.React

open Krakow.Website.View
open Krakow.Website.Update

Program.mkProgram init update view
|> Program.withReactSynchronous "app"
|> Program.withConsoleTrace
|> Program.run
