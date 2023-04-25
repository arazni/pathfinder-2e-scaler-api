module FileLoader.Loader

open System.IO
open FSharp.Json
open FileLoader.Models
open FileLoader.Conversions
open Persistence.Context
open Persistence.Mapping
open EntityFrameworkCore.FSharp.DbContextHelpers

type JsonFile = { creature: RawCreature[] }

let creaturesFromFile file =
  let config = JsonConfig.create(allowUntyped = true)
  File.ReadAllText file
  |> Json.deserializeEx<JsonFile> config  
  |> fun file -> file.creature
  |> Array.map rawCreatureToCreature

let load context creatures =
  creatures 
  |> addEntityRange context 
  |> ignore

  context
  |> saveChanges
  |> ignore

let upload context books = 
  books
  |> Array.map creaturesFromFile
  |> Array.map (Array.map toDatabaseCreature)
  |> Array.iter (load context)
  |> ignore