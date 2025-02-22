module Api.Program

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.HttpResults
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Persistence.Database
open Persistence.Mapping
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.HttpResults

open FSharp.MinimalApi
open FSharp.MinimalApi.Builder
open type TypedResults
open Scaler.Transformations
open type Microsoft.AspNetCore.Http.TypedResults
open type Microsoft.AspNetCore.Http.OpenApiRouteHandlerBuilderExtensions
open Scaler.Models
open Microsoft.Data.Sqlite
open Persistence

let routes =
  endpoints {
    get "/" (fun (req: {|db: SqliteConnection|}) -> 
      getCreatures req.db
      |> Ok
    )

    get "/{name}" produces<Ok<Creature>, NotFound> (fun (req: {| name: string; db: SqliteConnection |}) ->
      task {
        let creature = getCreature req.db req.name
        match creature with
        | Some c -> return !! Ok(c)
        | None -> return !! NotFound()
      }
    )

    get "/{name}/{level}" produces<Ok<Creature>, NotFound> (fun (req: {| name: string; level: int; db: SqliteConnection |}) ->
      task {
        let creature = getCreature req.db req.name
        match creature with
        | Some c -> return !! Ok((scaledCreature c req.level))
        | None -> return !! NotFound()
      }
    )
  }

[<EntryPoint>]
let main args =
  Dapper.FSharp.SQLite.OptionTypes.register()
  let builder = WebApplication.CreateBuilder(args)

  builder.Services
    .AddTransient<SqliteConnection>(fun _ -> Database.getConnection())
    |> ignore

  let app = builder.Build()

  app.UseHttpsRedirection()
  |> ignore

  app.MapGroup("").WithTags("Root")
  |> routes.Apply
  |> ignore

  app.Run("http://0.0.0.0:8080")

  0
