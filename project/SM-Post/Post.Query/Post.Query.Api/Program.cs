using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Dispatcher;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Action<DbContextOptionsBuilder> configureDBContext = o => o.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
builder.Services.AddDbContext<DatabaseContext>(configureDBContext);
builder.Services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDBContext));

//create database and tables from code
var databaseContext = builder.Services.BuildServiceProvider().GetService<DatabaseContext>();
databaseContext.Database.EnsureCreated(); // enures that database gets created

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IQueryHandler, QueryHandler>();
builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddScoped<IEventConsumer, EventConsumer>();

//register query handler methods
var queryHandler = builder.Services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
var dispatcher = new QueryDispatcher();
dispatcher.RegisterHandler<FindAllPostsQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostByIdQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsByAuthorQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsByLikesQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsWithCommentsQuery>(queryHandler.HandleAsync);
builder.Services.AddSingleton<IQueryDispatcher<PostEntity>>(_ => dispatcher);

builder.Services.AddControllers();
builder.Services.AddHostedService<ConsumerHostedService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
