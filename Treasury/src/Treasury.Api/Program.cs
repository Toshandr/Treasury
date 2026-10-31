using Treasury.Api.Dtos;
using Treasury.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRestoreService, RestoreService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/constellations/restore", (RestoreRequest request, IRestoreService service) =>
{
    var error = RestoreRequestValidator.Validate(request);
    return error is null
        ? Results.Ok(service.Restore(request.Reference!, request.Damaged!))
        : Results.BadRequest(new { error });
})
.Produces<RestoreResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

app.Run();

public partial class Program;
