module Scaler.Models

type DamageDice = {
  count: int
  size: int
}

type Damage = {
  count: int
  size: int
  bonus: option<int>
}

type CreatureAttribute = Map<string, int>

type CreatureResistance = {
  amount: int
  name: string
}

type CreatureWeakness = CreatureResistance

type CreatureSavingThrows = {
  fortitude: CreatureAttribute
  reflex: CreatureAttribute
  will: CreatureAttribute
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
  damage: option<Damage>
}

type CreatureSpellcasting = {
  name: string
  difficultyClass: int
  attack: Option<int>
}

type Creature = {
  name: string
  level: int
  skills: Map<string, CreatureAttribute>
  perception: CreatureAttribute
  abilityModifiers: CreatureAttribute
  attacks: CreatureAttack[]
  spellcasting: option<CreatureSpellcasting[]>
  defenses: CreatureDefenses
}

//let success =
//  true
// keywords: 
// plus...
// {@dice ...} rounds
// {@damage ...}
