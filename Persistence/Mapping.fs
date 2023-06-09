﻿module Persistence.Mapping

open FSharp.Json
open Scaler.Models

type DatabasePartialCreature = {
  level: int
  name: string
}

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
  Json.deserializeEx<Creature> jsonConfig creature.self

let toPartialCreature (creature: DatabaseCreature) =
  {
    name = creature.name;
    level = creature.level;
  }
