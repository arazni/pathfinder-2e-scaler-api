module Persistence.Mapping

open FSharp.Json
open Scaler.Models

[<CLIMutable>]
type DatabaseCreature = {
  level: int
  name: string
  self: string
}

let private jsonConfig = JsonConfig.create(allowUntyped = true)

let toDatabaseCreature (creature : Creature) =
  {
    name = creature.name;
    level = creature.level;
    self = Json.serializeEx jsonConfig creature
  }

let toCreature (creature: DatabaseCreature) =
  Json.deserializeEx jsonConfig creature.self