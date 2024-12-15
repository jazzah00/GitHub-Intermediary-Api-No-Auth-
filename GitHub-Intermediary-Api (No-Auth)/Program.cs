using GitHub_Intermediary_Api.Interfaces;
using GitHub_Intermediary_Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.AddScoped<IApiConnector, ApiConnector>();
builder.Services.AddScoped<IValidator, Validator>();
builder.Services.AddScoped<IConverter, Converter>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
