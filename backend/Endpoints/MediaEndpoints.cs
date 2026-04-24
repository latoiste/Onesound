using Microsoft.AspNetCore.Mvc;
using OneSound.DTO;
using OneSound.Media.Manager;
using OneSound.Media.Session;

namespace OneSound.Endpoints;

public static class MediaEndpoints
{
    public static void MapMediaApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (SessionManager sessionManager, [FromQuery] bool registered = false) => {
            List<string> aumids = registered ? sessionManager.GetRegisteredAumids() : sessionManager.GetAvailableAumids();
            List<SessionDto> sessionDtos = new();

            foreach (var aumid in aumids)
            {
                var dto = await DtoHelper.GetFromAumid(aumid);
                
                if (dto != null) sessionDtos.Add(dto);
            }

            return Results.Ok(sessionDtos);   
        });

        group.MapPost("/{aumid}", (string aumid, SessionManager sessionManager, MediaManager mediaManager) =>
        {
            Console.WriteLine("hellow");
            if (sessionManager.Register(aumid)) {
                var session = mediaManager.GetSessionFromId(aumid);
                if (session != null)
                {
                    sessionManager.AddActiveApp(aumid, session);
                }
            }
            return Results.NoContent();
        });

        group.MapDelete("/{aumid}", (string aumid, SessionManager sessionManager) =>
        {
            sessionManager.RemoveRegisteredAumid(aumid);
            sessionManager.RemoveActiveApp(aumid);

            return Results.NoContent();
        });
    }
}