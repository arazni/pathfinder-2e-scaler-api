module FileLoader.Conversions

open FileLoader.Models
open FileLoader.Regex
open Scaler.Models

let isInt raw =
  try
    raw |> int |> ignore
    true
  with
    | _ -> false

let takeIntegerValues raw =
  raw
  |> Map.map (fun _ v -> string v)
  |> Map.filter (fun _ v -> isInt v)
  |> Map.map (fun _ v -> int v)

let takeDamage raw =
  let damageString = raw |> matchDamage
  if damageString = "" then None
  else
    let pieces = damageString.Split([|"d"; "+"|], System.StringSplitOptions.RemoveEmptyEntries ||| System.StringSplitOptions.TrimEntries)
    Some {
      count = pieces[0] |> int;
      size = pieces[1] |> int;
      bonus = if pieces.Length > 2 then Some (pieces[2] |> int) else None
    }

let takePerception (creature: RawCreature) : CreatureAttribute =
  takeIntegerValues creature.perception

let takeAbilityModifiers (creature: RawCreature) : CreatureAttribute =
  takeIntegerValues creature.abilityModifiers

let takeSkills (creature: RawCreature) : Map<string, CreatureAttribute> = 
  creature.skills
  |> Map.map (fun _ v -> takeIntegerValues v)

let takeArmorClass (creature: RawCreature) : CreatureAttribute =
  takeIntegerValues creature.defenses.armorClass

let takeSavingThrows (creature: RawCreature) =
  {
    fortitude = takeIntegerValues creature.defenses.savingThrows.fortitude
    reflex = takeIntegerValues creature.defenses.savingThrows.reflex
    will = takeIntegerValues creature.defenses.savingThrows.will
  }

let takeHitPoint (creature: RawCreature) =
  creature.defenses.hitPoint[0].hitPoint

let takeResistances (creature: RawCreature) : option<CreatureResistance[]> =
  creature.defenses.resistances
  |> Option.map (fun resistances -> 
    Array.filter (fun (r: RawCreatureResistance) -> 
      Option.isSome r.amount) resistances 
      |> Array.map (fun r -> { name = r.name; amount = r.amount.Value; })
  )

let takeWeaknesses (creature: RawCreature) : option<CreatureWeakness[]> =
  creature.defenses.weaknesses
  |> Option.map (fun weaknesses -> 
    Array.filter (fun (w: RawCreatureWeakness) -> 
      Option.isSome w.amount) weaknesses 
      |> Array.map (fun w -> { name = w.name; amount = w.amount.Value; })
  )

let takeAttacks (creature: RawCreature) =
  creature.attacks
  |> Array.map (fun a -> {
    name = a.name;
    attack = a.attack;
    damage = takeDamage a.damage
  })

let takeSpellcasting (creature: RawCreature) : option<CreatureSpellcasting[]> =
  creature.spellcasting
  |> Option.map (fun spellLists ->
    spellLists
    |> Array.map (fun (s: RawCreatureSpellcasting) -> 
      { name = s.name; difficultyClass = s.difficultyClass; attack = s.attack}
    )
  )

let takeDefenses (creature: RawCreature) =
  {
    armorClass = takeArmorClass creature;
    savingThrows = takeSavingThrows creature;
    hitPoint = takeHitPoint creature;
    resistances = takeResistances creature;
    weaknesses = takeWeaknesses creature;
  }

let rawCreatureToCreature (raw: RawCreature) : Creature = 
  {
    name = raw.name;
    level = raw.level;
    skills = takeSkills raw;
    perception = takePerception raw;
    abilityModifiers = takeAbilityModifiers raw;
    attacks = takeAttacks raw;
    spellcasting = takeSpellcasting raw;
    defenses = takeDefenses raw;
  }