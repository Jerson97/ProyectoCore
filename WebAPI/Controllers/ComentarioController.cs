using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aplicacion.Comentarios;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class ComentarioController : MiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Unit>> Crear(Nuevo.Ejecuta data){
            return await Mediator.Send(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Eliminar(Guid id){
            return await Mediator.Send(new Eliminar.Ejecuta{Id = id});
        }

        [HttpGet]
        public async Task<List<Comentario>> Get(){
            return await Mediator.Send(new Consulta.Ejecuta());
        }
    }
}