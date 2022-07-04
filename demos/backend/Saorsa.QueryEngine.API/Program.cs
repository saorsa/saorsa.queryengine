using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Saorsa.QueryEngine;
using Saorsa.QueryEngine.Tests.NpgSql.Data;

QueryEngine.ScanQueryEngineTypes().ToList().ForEach(t =>
{
    QueryEngine.EnsureCompiled(t);
});

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<QueryNpgsqlDbContext>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var depScope = app.Services.CreateScope();
    var db = depScope.ServiceProvider.GetRequiredService<QueryNpgsqlDbContext>();
    db.Database.Migrate();
    db.Dispose();
    depScope.Dispose();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors();
app.Run();
