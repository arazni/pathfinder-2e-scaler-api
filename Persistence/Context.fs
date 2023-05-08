module Persistence.Context

open Microsoft.EntityFrameworkCore
open EntityFrameworkCore.FSharp.Extensions
open Persistence.Mapping

type CreatureContext() =
  inherit DbContext()

  [<DefaultValue>]
  val mutable private creatures : DbSet<DatabaseCreature>

  member this.Creatures with get() = this.creatures and set v = this.creatures <- v

  override _.OnModelCreating builder =
    let creatureBuilder = builder.Entity<DatabaseCreature>()

    creatureBuilder.HasKey (fun c -> c.name : obj) |> ignore

    creatureBuilder.HasIndex(fun c -> c.level : obj) |> ignore

    creatureBuilder.Property(fun c -> c.self : obj) |> ignore

  override _.OnConfiguring(options: DbContextOptionsBuilder) =
    options.UseSqlite("Data Source=creatures.sqlite")
      .UseFSharpTypes()
    |> ignore
