module Persistence.Query

open Persistence.Context
open Persistence.Mapping
open EntityFrameworkCore.FSharp.DbContextHelpers

let getCreature (db: CreatureContext) name =
  async {
    let! creature = tryFilterFirstAsync <@fun c -> c.name = name@> db.Creatures
    return Option.map toCreature creature
  }

let getCreatures (db: CreatureContext) =
  async {
    let! creatures = db.Creatures |> toListAsync
    return List.map (fun c -> { name = c.name; level = c.level }) creatures
  }
