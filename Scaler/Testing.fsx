open System.IO
#r "nuget: FSharp.Json"
open FSharp.Json
#load "Models.fs"
open Scaler.Models
#load "Conversions.fs"
open Scaler.Conversions
#load "Gamemastery.fs"
open Scaler.Gamemastery
#load "Estimations.fs"
open Scaler.Estimations

//let read =
//  File.ReadAllText("Data\\bestiary-1.json")

//let creatures =
//  let config = JsonConfig.create(allowUntyped = true)
//  Json.deserializeEx<JsonFile> config read
//  |> fun file -> file.creature

//Seq.map takePerception creatures
//Seq.map takeAbilityModifiers creatures
//Seq.map takeArmorClass creatures
//Seq.map takeAttacks creatures
//Seq.map takeHitPoint creatures
//Seq.map takeResistances creatures
//Seq.map takeSavingThrows creatures
//Seq.map takeSkills creatures
//Seq.map takeSpellcasting creatures

Seq.map (fun level -> (moderateStrikeDamageAverage level, moderateStrikeDamageDiceSize level)) levels
|> Seq.map (fun (average, size) -> averageToMatchingDamageDiceSize (decimal average) size)

Seq.map (fun level -> moderateStrikeDamageAverage level) levels
|> Seq.map (fun average -> averageToNearestDamageDice (decimal average))
