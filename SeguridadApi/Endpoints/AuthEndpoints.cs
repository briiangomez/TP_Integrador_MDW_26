using Application.DTO;
using Application.UseCases;

namespace SeguridadApi.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/token", async (TokenRequest req, EmitirTokenClienteUseCase useCase) =>
            ToHttpResult(await useCase.ExecuteAsync(req)))
            .WithName("EmitirTokenCliente")
            .WithSummary("Client Credentials (M2M): emite un JWT para un microservicio")
            .Produces<TokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/login", async (LoginRequest req, LoginUsuarioUseCase useCase) =>
            ToHttpResult(await useCase.ExecuteAsync(req)))
            .WithName("LoginUsuario")
            .WithSummary("Login de usuario humano: emite un JWT")
            .Produces<TokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }

    private static IResult ToHttpResult(AuthResult result) => result.Status switch
    {
        AuthStatus.Success => Results.Ok(result.Response),
        AuthStatus.BadRequest => Results.Problem(
            title: "Request inválido",
            statusCode: StatusCodes.Status400BadRequest,
            extensions: new Dictionary<string, object?> { ["errors"] = result.Errors }),
        AuthStatus.Forbidden => Results.Problem(
            title: "Acceso denegado",
            detail: result.ErrorMessage,
            statusCode: StatusCodes.Status403Forbidden),
        AuthStatus.Error => Results.Problem(
            title: "Error interno del servidor de autorización",
            detail: result.ErrorMessage,
            statusCode: StatusCodes.Status500InternalServerError),
        _ => Results.Unauthorized()
    };
}
