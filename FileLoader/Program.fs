module FileLoader.Program

open FileLoader.Loader
open Persistence.Context

let bestiaries = [| "Data\\bestiary-1.json"; "Data\\bestiary-2.json"; "Data\\bestiary-3.json" |]

let db = new CreatureContext()

upload db bestiaries

// verify
db.Creatures.Find("Petitioner")
|> fun c -> printfn "%s" c.name
