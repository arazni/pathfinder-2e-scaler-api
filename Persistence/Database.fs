module Persistence.Database

open Microsoft.Data.Sqlite
open Persistence.Mapping
open Dapper.FSharp.SQLite

let mutable isAlreadyInitialized = false

let internal command (connection: SqliteConnection) sql =
  new SqliteCommand(sql, connection)

let internal execute (command: SqliteCommand) = 
  command.ExecuteNonQueryAsync()
  |> Async.AwaitTask
  |> Async.RunSynchronously

let internal safeInit (conn: SqliteConnection) =
  if isAlreadyInitialized |> not then
    Dapper.FSharp.SQLite.OptionTypes.register()
    isAlreadyInitialized <- true
    
  conn.OpenAsync()
  |> Async.AwaitTask
  |> Async.RunSynchronously
  
  conn

let getConnection =
  new SqliteConnection("Data Source=creatures.sqlite")
  |> safeInit

let createCreaturesTable =
  use conn = getConnection
  
  "drop table if exists [creatures]"
  |> command conn
  |> execute
  |> ignore

  @"create table [creatures] (
    [name] text not null primary key,
    [level] int not null,
    [self] text not null
  )"
  |> command conn
  |> execute
  |> ignore

  @"create index [creatureNameLevel]
  on [creatures]
  ([name], [level])"
  |> command conn
  |> execute
  |> ignore

  conn.CloseAsync()
  |> Async.AwaitTask
  |> Async.RunSynchronously

let getCreatures() =
  use conn = getConnection
  let creaturesTable = table'<DatabasePartialCreature> "creatures"

  let creatures = 
    select {
      for c in creaturesTable do
      selectAll
    } |> conn.SelectAsync<DatabasePartialCreature>
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> Seq.toList

  conn.CloseAsync()
  |> Async.AwaitTask
  |> Async.RunSynchronously

  creatures

let getCreature name =
  use conn = getConnection
  let creaturesTable = table'<DatabaseCreature> "creatures"

  let creature =
    select {
      for c in creaturesTable do
      where (c.name = name)
    } |> conn.SelectAsync<DatabaseCreature>
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> Seq.tryHead

  conn.CloseAsync()
  |> Async.AwaitTask
  |> Async.RunSynchronously

  creature

let load creatures =
  use conn = getConnection

  let creaturesTable = table'<DatabaseCreature> "creatures"

  insert {
    into creaturesTable
    values creatures
  } |> conn.InsertAsync
  |> Async.AwaitTask
  |> Async.RunSynchronously
  |> ignore

  conn.CloseAsync()
  |> Async.AwaitTask
  |> Async.RunSynchronously
  |> ignore
