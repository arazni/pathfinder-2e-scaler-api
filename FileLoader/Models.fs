module FileLoader.Models
open FSharp.Json

type RawCreatureAttribute = Map<string, obj>

type RawCreatureHitPoint = {
  [<JsonField("hp")>]
  hitPoint: int
}

type RawCreatureResistance = {
  amount: option<int>
  name: string
}

type RawCreatureWeakness = RawCreatureResistance

type RawCreatureSavingThrows = {
  [<JsonField("fort")>]
  fortitude: RawCreatureAttribute
  [<JsonField("ref")>]
  reflex: RawCreatureAttribute
  will: RawCreatureAttribute
}

type RawCreatureDefenses = {
  [<JsonField("ac")>]
  armorClass: RawCreatureAttribute
  savingThrows: RawCreatureSavingThrows
  [<JsonField("hp")>]
  hitPoint: RawCreatureHitPoint[]
  resistances: option<RawCreatureResistance[]>
  weaknesses: option<RawCreatureWeakness[]>
}

type RawCreatureAttack = {
  name: string
  attack: int
  damage: string
}

type RawCreatureSpellcasting = {
  name: string
  [<JsonField("DC")>]
  difficultyClass: int
  attack: Option<int>
}

type RawCreature = {
  name: string
  level: int
  skills: Map<string, RawCreatureAttribute>
  perception: RawCreatureAttribute
  [<JsonField("abilityMods")>]
  abilityModifiers: RawCreatureAttribute
  attacks: RawCreatureAttack[]
  spellcasting: option<RawCreatureSpellcasting[]>
  defenses: RawCreatureDefenses
}
