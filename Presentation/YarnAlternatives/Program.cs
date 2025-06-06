using Application.Services;
using Database;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IMongoDb, MongoDb>();
builder.Services.AddScoped<IYarnAlternativeService, YarnAlternativeService>();
builder.Services.AddScoped<IYarnService, YarnService>();
builder.Services.AddControllers();

// Add services to the container.
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
app.MapControllers();

app.Run();