#r "nuget: FSharp.Json"
#r "nuget: Microsoft.Data.Sqlite"
#r "nuget: Dapper.FSharp"

#load "..\Scaler\Models.fs"
#load "..\Persistence\Mapping.fs"
#load "..\Persistence\Database.fs"

#load "Regex.fs"
#load "Models.fs"
#load "Conversions.fs"
#load "Loader.fs"

open Persistence
open FileLoader

Dapper.FSharp.SQLite.OptionTypes.register()

let conn = Database.getConnection()

Database.createCreaturesTable conn
let bestiaries = [| "Data\\bestiary-1.json"; "Data\\bestiary-2.json"; "Data\\bestiary-3.json" |]

Loader.upload conn bestiaries

Database.getCreature conn "Petitioner"

Database.getCreatures conn

conn.Dispose()
