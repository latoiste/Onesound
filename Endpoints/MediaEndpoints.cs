using OneSound.Utils;

namespace OneSound.Endpoints;

public static class MediaEndpoints
{
    public static void MapMediaApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", (SessionManager sessionManager) => {
            List<string> availableAumids = sessionManager.GetAvailableAumids();  

            return Results.Ok(availableAumids);   
        });

        group.MapPost("/{aumid}", (string aumid, SessionManager sessionManager) =>
        {
            Console.WriteLine("hellow");
            if (sessionManager.Register(aumid)) {
                var session = sessionManager.GetSessionFromId(aumid);
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