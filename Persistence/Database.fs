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

let internal openConnection (conn: SqliteConnection) =
  conn.OpenAsync()
  |> Async.AwaitTask
  |> Async.RunSynchronously

let internal closeConnection (conn: SqliteConnection) =
  conn.CloseAsync()
  |> Async.AwaitTask
  |> Async.RunSynchronously

let getConnection() =
  new SqliteConnection("Data Source=../creatures.sqlite")

let createCreaturesTable (conn: SqliteConnection) =
  openConnection conn
  
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

  closeConnection conn

let getCreatures (conn: SqliteConnection) =
  async {
    openConnection conn

    let creaturesTable = table'<DatabasePartialCreature> "creatures"

    let! creatures = 
      select {
        for c in creaturesTable do
        selectAll
      } |> conn.SelectAsync<DatabasePartialCreature>
      |> Async.AwaitTask

    closeConnection conn

    return creatures
    |> Seq.toList
  }

let getCreature (conn: SqliteConnection) name =
  async {
    openConnection conn
    let creaturesTable = table'<DatabaseCreature> "creatures"

    let! creature =
      select {
        for c in creaturesTable do
        where (c.name = name)
      } |> conn.SelectAsync<DatabaseCreature>
      |> Async.AwaitTask

    closeConnection conn

    return creature
    |> Seq.tryHead
    |> Option.map toCreature
  }

let load (conn: SqliteConnection) creatures =
  openConnection conn

  let creaturesTable = table'<DatabaseCreature> "creatures"

  insert {
    into creaturesTable
    values creatures
  } |> conn.InsertAsync
  |> Async.AwaitTask
  |> Async.RunSynchronously
  |> ignore

  closeConnection conn

