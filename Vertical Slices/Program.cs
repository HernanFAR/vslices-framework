using Application.UseCases;
using FluentValidation;
using Infrastructure.EntityFramework;
using Infrastructure.UseCases;
using Microsoft.EntityFrameworkCore;
using Vertical_Slices.Extensions;

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
        opts.UseSqlServer("Server=localhost;Database=QuestionDatabase;Trusted_Connection=Yes;TrustServerCertificate=True;");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.UseEndpointDefinition();

app.Run();
