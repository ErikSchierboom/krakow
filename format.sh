dotnet restore
dotnet fantomas --recurse ./src/Krakow.Core
dotnet fantomas --recurse ./src/Krakow.Repl
dotnet fantomas --recurse ./src/Krakow.Website/src
dotnet fantomas --recurse ./test/Krakow.Core.Tests