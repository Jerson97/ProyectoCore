using System.Linq;
using Aplicacion.Cursos;
using AutoMapper;
using Dominio;

namespace Aplicacion
{
    //Esta clase va a manejar los mapeos entre la clases entity.core Como cursos y el mapeo 
     // con las clases DTO como cursosDto
    public class MappingProfile : Profile
    {
        //Para inicializarlo creamos un contructor
        public MappingProfile(){
            // Dentro de este contructor vamos a indicarle quien van a mapearse
            CreateMap<Curso, CursoDto>()
            .ForMember(x => x.Instructores, y => y.MapFrom(z => z.InstructoresLink.Select(a => a.Instructor).ToList()))
            .ForMember(x => x.Comentarios, y => y.MapFrom(z => z.ComentarioLista))
            .ForMember(x => x.precio, y => y.MapFrom(z => z.PrecioPromocion));
            CreateMap<CursoInstructor, CursoInstructorDto>();
            CreateMap<Instructor, InstructorDto>();
            CreateMap<Precio, PrecioDto>();
            CreateMap<Comentario, ComentarioDto>();

        }
    }
}