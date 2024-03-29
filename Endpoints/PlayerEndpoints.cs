﻿using FluentValidation;
using Gc_Broadcasting_Api.Interfaces;
using Gc_Broadcasting_Api.Models;

namespace Gc_Broadcasting_Api.Endpoints;

public static class PlayerEndpoints {
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("api/v0/player");
        group.MapGet("{playerId}", GetPlayer).WithOpenApi();
        group.MapPost("/", CreatePlayer).WithOpenApi();
        group.MapPut("/", UpdatePlayer).WithOpenApi();
        //group.MapPatch("{name}", UpdateStats);
        group.MapDelete("{playerId}", DeletePlayer).WithOpenApi();
        // app.MapGet("api/v0/players/{teamId:int}", GetPlayersByTeamId);
    }

    private static async Task<IResult> CreatePlayer(Player player, IPlayerRepo playerRepo, IValidator<Player> playerRequestValidator, CancellationToken ct) {
        try {
            ct.ThrowIfCancellationRequested();
            var res = await playerRequestValidator.ValidateAsync(player, ct);
            if (!res.IsValid) {
                return TypedResults.BadRequest("Validation error, invalid player data.");
            }

            var created = await playerRepo.CreatePlayer(player, ct);
            if (!created) {
                return TypedResults.StatusCode(500);
            }

            return TypedResults.Created();
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            return TypedResults.StatusCode(500);
        }
    }

    private static async Task<IResult> GetPlayer(string playerId, IPlayerRepo playerRepo, 
        CancellationToken ct) {
        try {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(playerId)) {
                return TypedResults.BadRequest("CollegeId cannot be empty or just whitespaces.");
            }

            var player = await playerRepo.GetPlayer(playerId, ct) ?? throw new NullReferenceException
                ("Player object reference is null");
            if (string.IsNullOrWhiteSpace(player.Id)) {
                return TypedResults.NotFound(playerId);
            }

            return TypedResults.Ok(player);
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            return TypedResults.StatusCode(500);
        }
    }

    // private static async Task<IResult> GetPlayersByTeamId(int teamId, IPlayerRepo playerRepo, CancellationToken ct) {
    //     try {
    //         ct.ThrowIfCancellationRequested();
    //         if (teamId <= 0) {
    //             return TypedResults.BadRequest("Team Id cannot be zero or negative.");
    //         }
    //
    //         var players = await playerRepo.GetPlayers(teamId, ct) ?? throw new NullReferenceException("Players object reference is null.");
    //         return TypedResults.Ok(players);
    //     }
    //     catch (Exception ex) {
    //         Console.WriteLine(ex);
    //         return TypedResults.StatusCode(500);
    //     }
    // }

    private static async Task<IResult> UpdatePlayer(Player player, IPlayerRepo playerRepo, IValidator<Player> playerRequestValidator, CancellationToken ct) {
        try {
            ct.ThrowIfCancellationRequested();
            var res = await playerRequestValidator.ValidateAsync(player, ct);
            if (!res.IsValid) {
                return TypedResults.BadRequest("Validation error, invalid player data.");
            }

            var updated = await playerRepo.UpdatePlayer(player, ct);
            if (!updated) {
                return TypedResults.StatusCode(500);
            }

            return TypedResults.Ok(player);
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            return TypedResults.StatusCode(500);
        }
    }

    private static async Task<IResult> DeletePlayer(string playerId, IPlayerRepo playerRepo, CancellationToken ct) {
        try {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(playerId)) {
                return TypedResults.BadRequest("PlayerId cannot be empty or just whitespaces.");
            }
            var deleted = await playerRepo.DeletePlayer(playerId, ct);
            if (!deleted) {
                return TypedResults.StatusCode(500);
            }
            return TypedResults.Ok();
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            return TypedResults.StatusCode(500);
        }
    }
}
