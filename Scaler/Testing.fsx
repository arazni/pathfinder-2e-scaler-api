open System.IO
#r "nuget: FSharp.Json"
open FSharp.Json
#load "Models.fs"
open Scaler.Models
#load "Regex.fs"
#load "Conversions.fs"
open Scaler.Conversions
#load "Gamemastery.fs"
open Scaler.Gamemastery
#load "Estimations.fs"
open Scaler.Estimations
#load "Transformations.fs"
open Scaler.Transformations

let read =
  File.ReadAllText("Data\\bestiary-3.json")




//Seq.map takePerception creatures
//Seq.map takeAbilityModifiers creatures
//Seq.map takeArmorClass creatures
//Seq.map takeAttacks creatures
//Seq.map takeHitPoint creatures
//Seq.map takeResistances creatures
//Seq.map takeSavingThrows creatures
//Seq.map takeSkills creatures
//Seq.map takeSpellcasting creatures

//let levels = [ -1 .. 24 ]

//Seq.map (fun level -> (moderateStrikeDamageAverage level, moderateStrikeDamageDiceSize level)) levels
//|> Seq.map (fun (average, size) -> averageToMatchingDamageDiceSize average size)

//Seq.map (fun level -> moderateStrikeDamageAverage level) levels
//|> Seq.map (fun average -> averageToNearestDamageDice average)

//Seq.init 20 (fun level -> float (level + 1) * 3.75 |> int, level)
//|> Seq.map(fun (regen, level) -> level, level + 2, regen, estimateRegeneration regen level (level+2) )
//|> Seq.iter (fun (ol, nl, ore, nre) -> printfn "(ol: %d, nl: %d, ore: %d, nre: %d)" ol nl ore nre)

//Seq.init 21 (fun level -> float (level + 1) * 2.75 |> int, level)
//|> Seq.map(fun (damage, level) -> level, level + 2, damage, estimateAverageStrikeDamage damage 4 level (level+2) )
//|> Seq.iter (fun (ol, nl, ore, nre) -> printfn "(ol: %d, nl: %d, o: %d, n: %dd%d+%d (%d))" ol nl ore nre.count nre.size (nre.bonus.Value) (nre.bonus.Value + averageDiceDamage nre.count nre.size))

//Seq.init 23 (fun level -> level, extremeStrikeDamageDiceSize level, estimateDieSizeFromCategory StrikeEstimateCategory.Moderate (extremeStrikeDamageDiceSize level) level (level+2))
//|> Seq.iter (fun (l, o, n) -> printfn "%d: %d->%d" l o n)

