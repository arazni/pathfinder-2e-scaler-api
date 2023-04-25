module Scaler.Transformations

open Scaler.Models
open Scaler.Estimations
open Scaler.Gamemastery

let newLevelSkills oldLevel newLevel skills =
  skills
  |> Map.map (fun name attribute -> Map.map (fun key value -> estimateSkill value oldLevel newLevel) attribute)

let newPerception oldLevel newLevel perception =
  perception
  |> Map.map (fun k v -> estimatePerception v oldLevel newLevel)

let newAbilityModifiers oldLevel newLevel abilityModifiers =
  abilityModifiers
  |> Map.map (fun k v -> estimateAbilityModifier v oldLevel newLevel)

let newAttacks oldLevel newLevel (attacks: CreatureAttack[]) =
  attacks
  |> Array.map (fun a -> { 
    name = a.name;
    attack = estimateStrikeHit a.attack oldLevel newLevel;
    damage = Option.map (fun d -> estimateAverageStrikeDamage (averageDamage d.count d.size d.bonus) d.size oldLevel newLevel) a.damage
  })

let newSpellcasting oldLevel newLevel (spellOptions: Option<CreatureSpellcasting[]>) =
  match spellOptions with
  | None -> None
  | Some spells ->
    spells
    |> Array.map (fun spell -> {
      name = spell.name;
      difficultyClass = estimateSpellDifficultyClass spell.difficultyClass oldLevel newLevel;
      attack = Option.map (fun hit -> estimateSpellHit hit oldLevel newLevel) spell.attack;
    })
    |> Some

let newDefenses oldLevel newLevel (defenses: CreatureDefenses) =
  {
    armorClass = Map.map (fun k v -> estimateArmorClass v oldLevel newLevel) defenses.armorClass;
    savingThrows = {
      fortitude = Map.map (fun k v -> estimateSavingThrow v oldLevel newLevel) defenses.savingThrows.fortitude
      reflex = Map.map (fun k v -> estimateSavingThrow v oldLevel newLevel) defenses.savingThrows.reflex
      will = Map.map (fun k v -> estimateSavingThrow v oldLevel newLevel) defenses.savingThrows.will
    };
    hitPoint = estimateHitPoint defenses.hitPoint oldLevel newLevel;
    resistances = 
      defenses.resistances
      |> Option.map (fun resistances -> Array.map (fun (resistance: CreatureResistance) -> { name = resistance.name; amount = estimateResistanceOrWeakness resistance.amount oldLevel newLevel }) resistances)
    weaknesses =
      defenses.weaknesses
      |> Option.map (fun weaknesses -> Array.map (fun (weakness: CreatureWeakness) -> { name = weakness.name; amount = estimateResistanceOrWeakness weakness.amount oldLevel newLevel }) weaknesses)
  }

let scaledCreature (baseCreature: Creature) newLevel =
  {
    name = baseCreature.name;
    level = newLevel;
    skills = newLevelSkills baseCreature.level newLevel baseCreature.skills;
    perception = newPerception baseCreature.level newLevel baseCreature.perception;
    abilityModifiers = newAbilityModifiers baseCreature.level newLevel baseCreature.abilityModifiers;
    attacks = newAttacks baseCreature.level newLevel baseCreature.attacks;
    spellcasting = newSpellcasting baseCreature.level newLevel baseCreature.spellcasting;
    defenses = newDefenses baseCreature.level newLevel baseCreature.defenses;
  }

