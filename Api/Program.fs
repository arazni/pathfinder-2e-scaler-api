module Api.Program

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http.HttpResults
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
      async {
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

  let app = builder.Build()

  app.UseHttpsRedirection()
  |> ignore

  app.MapGroup("api").WithTags("Root")
  |> routes.Apply
  |> ignore

  app.Run()

  0
