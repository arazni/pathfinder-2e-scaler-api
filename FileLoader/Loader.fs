module FileLoader.Loader

open System.IO
open FSharp.Json
open FileLoader.Models
open FileLoader.Conversions
open Persistence.Mapping
open Persistence.Database

type JsonFile = { creature: RawCreature[] }

let creaturesFromFile file =
  let config = JsonConfig.create(allowUntyped = true)
  File.ReadAllText file
  |> Json.deserializeEx<JsonFile> config  
  |> fun file -> file.creature
  |> Array.map rawCreatureToCreature

let upload conn books =
  books
  |> Array.map creaturesFromFile
  |> Array.map (Seq.map toDatabaseCreature >> Seq.toList)
  |> Array.iter (load conn)