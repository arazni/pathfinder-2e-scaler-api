module FileLoader.Program

open System.IO
open FileLoader.Loader
open Persistence

Dapper.FSharp.SQLite.OptionTypes.register()

let bestiaries = [| "Data\\bestiary-1.json"; "Data\\bestiary-2.json"; "Data\\bestiary-3.json" |]

let conn = Database.getConnection()

Database.createCreaturesTable conn

upload conn bestiaries

// verify
Database.getCreature conn "Petitioner"
|> Async.RunSynchronously
|> Option.iter (fun c -> printfn "%A" c)

Database.getCreatures conn
|> Async.RunSynchronously
|> Seq.head
|> printfn "%A"

conn.Dispose()

// manually copy and move sqlite file to Api
