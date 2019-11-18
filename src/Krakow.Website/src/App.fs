module Krakow.Website.App

open Elmish
open Elmish.React

open Krakow.Website.Model
open Krakow.Website.Update
open Krakow.Website.View

Program.mkProgram init update view
|> Program.withReactSynchronous "app"
|> Program.withConsoleTrace
|> Program.run
