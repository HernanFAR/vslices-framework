using Sample.Core;
using Sample.Core.UseCases;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

builder.Services.AddGeneralDependencies();

builder.Services.AddEndpointDefinition<CreateQuestionEndpoint>();
builder.Services.AddEndpointDefinition<GetAllQuestionsEndpoint>();
builder.Services.AddEndpointDefinition<GetQuestionEndpoint>();
builder.Services.AddEndpointDefinition<RemoveQuestionEndpoint>();
builder.Services.AddEndpointDefinition<UpdateQuestionEndpoint>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseEndpointDefinitions();

app.Run();
