module Scaler.Models

open FSharp.Json

type IntOrGarbage =
  | Int32
  | Decimal
  | String
  | Object

type RawCreatureAttribute = Map<string, obj>

type CreatureAttribute = Map<string, int>

type RawCreatureHitPoint = {
  [<JsonField("hp")>]
  hitPoint: int
}

type CreatureResistance = {
  amount: int
  name: string
}

type CreatureWeakness = CreatureResistance

type RawCreatureSavingThrows = {
  [<JsonField("fort")>]
  fortitude: RawCreatureAttribute
  [<JsonField("ref")>]
  reflex: RawCreatureAttribute
  will: RawCreatureAttribute
}

type CreatureSavingThrows = {
  fortitude: CreatureAttribute
  reflex: CreatureAttribute
  will: CreatureAttribute
}

type RawCreatureDefenses = {
  [<JsonField("ac")>]
  armorClass: RawCreatureAttribute
  savingThrows: RawCreatureSavingThrows
  [<JsonField("hp")>]
  hitPoint: RawCreatureHitPoint[]
  resistances: option<CreatureResistance[]>
}

type CreatureDefenses = {
  armorClass: CreatureAttribute
  savingThrows: CreatureSavingThrows
  hitPoint: int
  resistances: option<CreatureResistance[]>
  weaknesses: option<CreatureWeakness[]>
}

type CreatureAttack = {
  name: string
  attack: int
  damage: string
}

type CreatureSpellcasting = {
  name: string
  [<JsonField("DC")>]
  difficultyClass: int
}

type RawCreature = {
  name: string
  level: int
  skills: Map<string, RawCreatureAttribute>
  perception: RawCreatureAttribute
  [<JsonField("abilityMods")>]
  abilityModifiers: RawCreatureAttribute
  attacks: CreatureAttack[]
  spellcasting: option<CreatureSpellcasting[]>
  defenses: RawCreatureDefenses
}

type Creature = {
  name: string
  level: int
  skills: Map<string, CreatureAttribute>
  perception: CreatureAttribute
  [<JsonField("abilityMods")>]
  abilityModifiers: CreatureAttribute
  attacks: CreatureAttack[]
  spellcasting: option<CreatureSpellcasting[]>
  defenses: CreatureDefenses
}

type JsonFile = { creature: RawCreature[] }

//let success =
//  true
// keywords: 
// plus...
// {@dice ...} rounds
// {@damage ...}
