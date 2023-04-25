module Scaler.Gamemastery

let everyRationalInterval multiplier interval x =
  max (x*multiplier/interval) 0

let everyInterval interval x =
  everyRationalInterval 1 interval x

let every2 x =
  everyInterval 2 x

let every3 x =
  everyInterval 3 x

let oneAt at x =
  if at = x then 1 else 0

let oneAtEach list x =
  if Seq.contains x list then 1 else 0

let oneOnEven x =
  (x + 1) % 2

let oneStartingAt at x =
  if at <= x then 1 else 0

let oneStartingAtEach list x =
  Seq.sumBy (fun at -> oneStartingAt at x) list

let multiplierStartingAt at x =
  if at <= x then 1 + x - at else 0

let lowAbilityModifier level =
  level/4 + oneStartingAt 1 level

let moderateAbilityModifier level =
  match level with
  | x when x < 5 -> (x+11)/4
  | x when x < 20 -> (x+15)/5
  | _ -> level/2 - 3

let highAbilityModifier level =
  match level with
  | x when x < 20 -> (x+11)/3
  | _ -> level/4*2

let extremeAbilityModifier level =
  highAbilityModifier level + oneStartingAt 1 level + oneAt 6 level

let terriblePerception level =
  level + 1 + every3 (level - 1)

let lowPerception level =
  level + 3 + everyRationalInterval 2 5 (level-1)

let moderatePerception level =
  lowPerception level + 3 - everyInterval 24 level

let highPerception level =
  lowPerception level + 6 - oneAt 16 level

let extremePerception level =
  level + 10 + every2 (level-1) + every2 (level-22) + oneAt 22 level

let terribleSkill level =
  level + 2 + every3 (level-1)

let lowSkill level =
  level + 3 + every2 (level-1) - oneAt 23 level

let moderateSkill level =
  2 + lowSkill level

let highSkill level =
  level + 6 + everyRationalInterval 2 3 (level-1)

let extremeSkill level =
  3 + highSkill level

let lowArmorClass level =
  13 + level + every2 level - oneStartingAt 1 level

let moderateArmorClass level =
  2 + lowArmorClass level

let highArmorClass level =
  3 + lowArmorClass level

let extremeArmorClass level =
  6 + lowArmorClass level

let terribleSavingThrow level =
  1 + level + every3 (level-1)

let lowSavingThrow level =
  3 + level + everyRationalInterval 2 5 (level-1)

let moderateSavingThrow level =
  3 + lowSavingThrow level - oneStartingAt 24 level

let highSavingThrow level =
  6 + lowSavingThrow level - oneAt 16 level

let extremeSavingThrow level =
  match level with
  | level when level < 22 -> 10 + level + every2 (level-1)
  | _ -> 10 + level + every2 level

let moderateHitPoint level =
  match level with
  | level when level < 1 -> 15 + 8 * level
  | 1 -> 20
  | _ -> 15 * level + (5 * multiplierStartingAt 6 level) + (5 * multiplierStartingAt 21 level) + (5 * multiplierStartingAt 22 level) + (10 * multiplierStartingAt 24 level)

let highHitPoint level =
  (5 * (moderateHitPoint level)) / 4 + oneAt -1 level 

let lowHitPoint level =
  (3 * (moderateHitPoint level)) / 4

let moderateResistance level =
  (level+3)/2

let moderateWeakness level =
  moderateResistance level

let highResistance level =
  3 + level - oneAtEach [-1; 1] level - oneStartingAt 17 level

let highWeakness level =
  highResistance level

let moderateStrikeHit level =
  6 + level + oneAt -1 level + every2 level

let highStrikeHit level =
  2 + moderateStrikeHit level

let extremeStrikeHit level =
  4 + moderateStrikeHit level

let lowStrikeHit level =
  4 + level + oneAt -1 level + every3 (level+1)

let strikeDamageDiceCount level =
  1 + oneStartingAt 4 level + oneStartingAt 12 level + oneStartingAt 19 level

let lowStrikeDamageDiceSize level =
  match level with
  | level when level < 2 -> 4
  | level when level < 4 -> 6
  | level when level < 7 -> 4
  | 11 -> 8
  | _ -> 6

let moderateStrikeDamageDiceSize level =
  match level with
  | level when level < 10 -> lowStrikeDamageDiceSize level + (2 * oneStartingAt 1 level)
  | level when level < 12 -> 10
  | level when level < 15 -> 8
  | level when level < 19 -> 10
  | level when level < 23 -> 8
  | _ -> 10

let highStrikeDamageDiceSize level =
  2 + moderateStrikeDamageDiceSize level - (2 * oneAtEach [-1; 1] level)

let extremeStrikeDamageDiceSize level =
  match level with
  | level when level < 1 -> 6
  | 1 -> 8
  | 4 -> 10
  | _ -> 12

let lowStrikeDamageAverage level =
  3 + level + oneStartingAtEach [2;3;5;8;11;14;21;24] level

let moderateStrikeDamageAverage level =
  match level with
  | level when level < 16 -> 4 + level + everyRationalInterval 2 3 (level-1) + (2 * oneStartingAt 2 level)
  | _ -> 15 + level + (multiplierStartingAt 19 level) - oneStartingAt 21 level

let highStrikeDamageAverage level =
  match level with
  | level when level < 0 -> 4 + level
  | 0 -> 5
  | 1 -> 6
  | 2 -> 9
  | _ -> 6 + 2*level + - oneStartingAtEach [16;17] level

let extremeStrikeDamageAverage level =
  match level with
  | level when level < 2 -> 6 + 2*level
  | 2 -> 11
  | _ -> 8 + 2*level + every2 level

let averageDiceDamage number size =
  int (decimal number * decimal (size+1) / 2m)

let averageDamage number size bonus =
  averageDiceDamage number size
  |> (+) (Option.defaultValue 0 bonus)

let lowStrikeDamageBonus level =
  lowStrikeDamageAverage level 
  - averageDiceDamage (strikeDamageDiceCount level) (lowStrikeDamageDiceSize level)

let moderateStrikeDamageBonus level =
  moderateStrikeDamageAverage level 
  - averageDiceDamage (strikeDamageDiceCount level) (moderateStrikeDamageDiceSize level) 
  - oneAt -1 level

let highStrikeDamageBonus level =
  highStrikeDamageAverage level 
  - averageDiceDamage (strikeDamageDiceCount level) (highStrikeDamageDiceSize level)
  - oneAtEach [3;18] level

let extremeStrikeDamageBonus level =
  extremeStrikeDamageAverage level
  - averageDiceDamage (strikeDamageDiceCount level) (extremeStrikeDamageDiceSize level)
  - oneAtEach [2;3] level

let moderateSpellDifficultyClass level =
  13 + level + every3 level + oneAt -1 level

let highSpellDifficultyClass level =
  3 + moderateSpellDifficultyClass level

let extremeSpellDifficultyClass level =
  19 + level + everyRationalInterval 2 5 (level+1) + oneAt -1 level - oneAt 24 level

let moderateSpellHit level =
  moderateSpellDifficultyClass level - 8

let highSpellHit level =
  highSpellDifficultyClass level - 8

let extremeSpellHit level =
  extremeSpellDifficultyClass level - 8

let expectedHighestSpellLevel level =
  (level+1)/2
  |> max 0
  |> min 10

let limitedUseAreaDamageAverage level =
  match level with
  | level when level < 1 -> 6 + level*2
  | _ -> averageDiceDamage (level+1) 6 + oneOnEven level

let unlimitedUseAreaDamageAverage level =
  match level with
  | level when level < 15 -> 3 + level + every2 (level+2) + oneStartingAt 3 level
  | _ -> 12 + level + every2 (level-17)

let moderateRegeneration level =
  highStrikeDamageAverage level

let highRegeneration level =
  1.5 * double (highStrikeDamageAverage level)
  |> int

// at-will heals: spell level 2 lower than highest spells
// resistance hp adjustment: no guidance on fewer hp, skip
// weakness hp adjustment: bonus hp = quadruple weakness or hp = weakness if hard to exploit, skip
// special single-target damage can use extreme strike damage
// single action or condition effects should be -2 level
