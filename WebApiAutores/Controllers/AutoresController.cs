using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using System.Net.Http.Headers;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using System.Linq;


namespace WebApiAutores.Controllers
{       
    [ApiController]
    [Route("api/autores")]//autores rutas de api para separar recursos
   
    public class AutoresController: ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(AplicationDbContext context, IMapper mapper )
           
        {
            this.context = context;
            this.mapper = mapper;
        }              

        [HttpGet]//api/autores  
        public async Task<List<AutorDTO>> Get() 
        {               
            var autores = await context.Autores.ToListAsync();
               
            return mapper.Map<List<AutorDTO>>(autores);
        }
        
        [HttpGet("{id:int}/", Name = "obtenerAutor")]//variable de ruta con restriccion
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores
                 .Include(autorDB => autorDB.AutoresLibros)
                 .ThenInclude(autorLibroDB => autorLibroDB.Libro)
                 .FirstOrDefaultAsync(autorBD => autorBD.Id == id);                            
                 
           if(autor == null)
            {
                return NotFound();
            }
            return mapper.Map<AutorDTOConLibros>(autor);           
        }
        
        [HttpGet("{nombre}")]//rutas por tipo de dato
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string nombre)
        {
            var autores = await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync(); //firs retorna el primer registro
            

            return mapper.Map<List<AutorDTO>>(autores);
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var exsiteAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);

            if(exsiteAutorConElMismoNombre)
            {
                return BadRequest($"ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor); //guardar registros en DB
            await context.SaveChangesAsync();//guardar cambios de manera asincrona

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}")] //api/autores/1
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if(autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");
            }
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{id:int}")] //api/autores/2
        public async Task<ActionResult> Delete(int id) 
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if(!existe)
            {
                return NotFound();
            }
            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }


     }
}
