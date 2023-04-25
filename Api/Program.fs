module Api.Program

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.HttpResults
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Persistence.Context
open Persistence.Query
open FSharp.MinimalApi
open Scaler.Transformations
open type Microsoft.AspNetCore.Http.TypedResults
open type Microsoft.AspNetCore.Http.OpenApiRouteHandlerBuilderExtensions
open Scaler.Models

let routes =
  endpoints {
    get "/" (fun (db: CreatureContext) -> 
      task {
        let! creatures = getCreatures db
        return Ok(creatures)
      })

    get "/{name}" produces<Ok<Creature>, NotFound> (fun name (db: CreatureContext) ->
      task {
        let! creature = getCreature db name
        match creature with
        | Some c -> return !> Ok(c)
        | None -> return !> NotFound()
      }
    )

    get "/{name}/{level}" produces<Ok<Creature>, NotFound> (fun name level (db: CreatureContext) ->
      task {
        let! creature = getCreature db name
        match creature with
        | Some c -> return !> Ok((scaledCreature c level))
        | None -> return !> NotFound()
      }
    )
  }

[<EntryPoint>]
let main args =
  let builder = WebApplication.CreateBuilder(args)

  builder.Services
    .AddDbContext<CreatureContext>(ServiceLifetime.Transient, ServiceLifetime.Transient)
    |> ignore

  let app = builder.Build()

  app.UseHttpsRedirection()
  |> ignore

  app.MapGroup("api").WithTags("Root")
  |> routes.Apply
  |> ignore

  app.Services.GetRequiredService<CreatureContext>().Database.EnsureCreated()
  |> ignore

  app.Run()

  0
