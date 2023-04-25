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
  query {
    for c in db.Creatures do
      select (c.name, c.level)
  }
  |> toListAsync