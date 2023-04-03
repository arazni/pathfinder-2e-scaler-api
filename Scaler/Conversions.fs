module Scaler.Conversions

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

let takePerception (creature: RawCreature) : CreatureAttribute =
  takeIntegerValues creature.perception

let takeAbilityModifiers (creature: RawCreature) : CreatureAttribute =
  takeIntegerValues creature.abilityModifiers

let takeSkills (creature: RawCreature) : Map<string, CreatureAttribute> = 
  creature.skills
  |> Map.map (fun _ v -> takeIntegerValues v)

let takeArmorClass (creature: RawCreature) : CreatureAttribute =
  takeIntegerValues creature.defenses.armorClass

let takeSavingThrows (creature: RawCreature) : CreatureSavingThrows =
  {
    fortitude = takeIntegerValues creature.defenses.savingThrows.fortitude
    reflex = takeIntegerValues creature.defenses.savingThrows.reflex
    will = takeIntegerValues creature.defenses.savingThrows.will
  }

let takeHitPoint (creature: RawCreature) =
  creature.defenses.hitPoint[0].hitPoint

let takeResistances (creature: RawCreature) =
  creature.defenses.resistances

let takeAttacks (creature: RawCreature) =
  creature.attacks

let takeSpellcasting (creature: RawCreature) =
  creature.spellcasting

