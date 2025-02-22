module FileLoader.Program

open FileLoader.Loader
open Persistence

let bestiaries = [| "Data\\bestiary-1.json"; "Data\\bestiary-2.json"; "Data\\bestiary-3.json" |]

Database.createCreaturesTable

upload bestiaries

// verify
Database.getCreature "Petitioner"
|> Option.iter (fun c -> printfn "%A" c)

Database.getCreatures()
|> Seq.head
|> printfn "%A"

// manually copy and move sqlite file to Api