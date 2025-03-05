using Microsoft.AspNetCore.Authorization;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ristorante_backend.Services;
using ristorante_backend.Repositories;
using ristorante_backend.Models;

namespace ristorante_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PiattoController : ControllerBase
    {

        private PiattoRepository PiattoRepository { get; set; }

        public PiattoController(ICustomLogger logger, PiattoRepository piattoRepository)
        {
            PiattoRepository = piattoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string? nome)
        {

            try
            {
                if (nome == null)
                {
                    List<Piatto> result = (await PiattoRepository.GetAllPiatto());
                    return Ok(result);


                }
                else
                {
                    List<Piatto> result = (await PiattoRepository.GetPiattoByName(nome));
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetPizzaById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Inserire un id maggiore di 0");
            };

            try
            {
                Piatto p = await PiattoRepository.GetPiattoByIdAsync(id);

                if (p != null)
                {
                    return Ok(p);
                }
                else
                {
                    return NotFound($"Non è stata trovata nessuna pizza con l' id: {id}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Top")]
        public async Task<IActionResult> GetLimit(int? limit)
        {

            try
            {
                if (limit == null)
                {
                    List<Piatto> result = (await PiattoRepository.GetAllPiatto());
                    return Ok(result);


                }
                else
                {
                    List<Piatto> result = (await PiattoRepository.GetAllPiatto(limit));
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreatePiatto([FromBody] Piatto p)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest($"Dati non validi: {ModelState.Values}");
                }
                p.Id = 0; 
                (int piattoID, Piatto createdPiatto) createdTupla = await PiattoRepository.CreatePiatto(p);
                return Created($"/{ControllerContext.ActionDescriptor.ControllerName}/{createdTupla.piattoID}", $"è stato creato un piatto con id: {createdTupla.piattoID} e nome: {createdTupla.createdPiatto.Nome}"); 
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> UpdatePiatto(int id, [FromBody] Piatto updatedPiatto)
        {
            
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest($"Dati non validi: {ModelState.Values}");
                }
                int affectedRows = await PiattoRepository.UpdatePiatto(id, updatedPiatto);
                if (affectedRows == 0)
                {
                    return NotFound();
                }
                return Ok(affectedRows);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("{id}/Menu")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddPiattoMenu(int id, [FromBody]List<int> menu)
        {
            try
            {
                if (await PiattoRepository.GetPiattoByIdAsync(id) == null)
                {
                    return NotFound();
                }
                int menuInseriti = await PiattoRepository.AddPiattoMenu(id, menu);
                return Ok($"Sono stati inseriti {menuInseriti} menù");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> DeletePiatto(int id)
        {
            try
            {
                int affectedRows = await PiattoRepository.DeletePiatto(id);
                if (affectedRows == 0)
                {
                    return NotFound();
                }
                return Ok(affectedRows);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}/Menu")]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> DeletePiattoMenu(int id, int menu)
        {
            try
            {
                int deletedMenu = await PiattoRepository.DeletePiattoMenu(id, menu);
                if (deletedMenu == 0)
                {
                    return NotFound();
                }
                return Ok(deletedMenu);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


    }
}
