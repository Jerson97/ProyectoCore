using System;
using System.Threading.Tasks;
using Aplicacion.Documentos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class DocumentoController : MiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Unit>> GuardadrArchivo(SubirArchivo.Ejecuta parametros){
            return await Mediator.Send(parametros);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArchivoGenerico>> ObtenerDocumento(Guid id){
            return await Mediator.Send(new ObtenerArchivo.Ejecuta {Id = id});
        }
    }
}