using Core.UseCases;
using FluentValidation;
using Infrastructure.EntityFramework;
using Infrastructure.UseCases;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

builder.Services.AddEndpointDefinitionsFrom<Application.Anchor>();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblyContaining<Application.Anchor>();
});

builder.Services.AddValidatorsFromAssemblyContaining<Application.Anchor>();
builder.Services.AddValidatorsFromAssemblyContaining<Domain.Anchor>();

builder.Services.AddScoped<ICreateQuestionRepository, CreateQuestionRepository>();
builder.Services.AddScoped<IUpdateQuestionRepository, UpdateQuestionRepository>();
builder.Services.AddScoped<IRemoveQuestionRepository, RemoveQuestionRepository>();
builder.Services.AddScoped<IGetQuestionRepository, GetQuestionRepository>();
builder.Services.AddScoped<IGetAllQuestionsRepository, GetAllQuestionsRepository>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services
    .AddDbContext<ApplicationDbContext>(opts =>
    {
        const string connectionString = "DataSource=mydatabase.db;";

        var connection = new SqliteConnection(connectionString);

        connection.Open();
        connection.Close();

        connection.Dispose();

        opts.UseSqlite(connectionString);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.UseEndpointDefinition();

app.Run();
