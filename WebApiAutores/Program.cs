
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAutores;
using WebApiAutores.Controllers;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<AplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.

builder.Services.AddControllers(opciones =>
{
    opciones.Filters.Add(typeof(FiltroDeExcepcion));
}).AddJsonOptions(x =>
x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.AddAutoMapper(typeof(Program));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.

//app.Use(async (contexto, siguiente) =>
//{   //guardar el cuerpo de la rpta en un logg
//    {
//        using (var ms = new MemoryStream())
//        {
//            var cuerpoOriginalRespuesta = contexto.Response.Body;
//            contexto.Response.Body = ms;

//            await siguiente.Invoke();

//            ms.Seek(0, SeekOrigin.Begin);
//            string respuesta = new StreamReader(ms).ReadToEnd();
//            ms.Seek(0, SeekOrigin.Begin);

//            await ms.CopyToAsync(cuerpoOriginalRespuesta);
//            contexto.Response.Body = cuerpoOriginalRespuesta;

//           // logger.LogInformation(respuesta);
//        }
//    }
//});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var servicioLogger = (ILogger<Program>)app.Services.GetService(typeof(ILogger<Program>));

app.Run();
