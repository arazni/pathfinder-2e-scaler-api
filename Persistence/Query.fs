module Persistence.Query

open Persistence.Context
open Persistence.Mapping
open EntityFrameworkCore.FSharp.DbContextHelpers
open Microsoft.EntityFrameworkCore

let getCreature (db: CreatureContext) name =
  async {
    let! creature = tryFilterFirstAsync <@fun c -> c.name = name@> (db.Creatures.AsNoTracking())
    return Option.map toCreature creature
  }

let getCreatures (db: CreatureContext) =
  async {
    let! creatures = (db.Creatures.AsNoTracking()) |> toListAsync
    return List.map toPartialCreature creatures
  }
