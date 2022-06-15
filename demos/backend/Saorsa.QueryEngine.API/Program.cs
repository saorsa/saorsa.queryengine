using System.Text.Json.Serialization;
using Saorsa.QueryEngine;
using Saorsa.QueryEngine.Tests.NpgSql.Data;

QueryEngine.ScanQueryEngineTypes().ToList().ForEach(t =>
{
    QueryEngine.EnsureCompiled(t);
});

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<QueryNpgsqlDbContext>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
