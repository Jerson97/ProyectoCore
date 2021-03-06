import { TableContainer, Paper, Table, TableHead, TableBody, TableRow, TableCell, TablePagination, Hidden, Grid, TextField } from '@material-ui/core';
import React, { useEffect, useState } from 'react';
import { paginacioncurso } from '../../actions/CursoAction';
import ControlTyping from '../Tool/ControlTyping';

const PaginadorCurso = () => {

    const [textoBusquedaCurso, setTextoBusquedaCurso] = useState("");
    const typingBuscadorTexto = ControlTyping(textoBusquedaCurso, 900);


    const [paginadoRequest, setPaginadorRequest] = useState({
        titulo: "",
        numeroPagina: 0,
        cantidadElementos: 5
    });

    const [paginadorResponse, setPaginadorResponse] = useState({
        listaRecords: [],
        totalRecords: 0,
        numerpPaginas: 0
    });

    useEffect ( () => {

    
        const obtenerListaCurso = async() => {

            let tituloVariant = "";
            let paginaVariant = paginadoRequest.numeroPagina + 1;
            if(typingBuscadorTexto){
            tituloVariant = typingBuscadorTexto;
            paginaVariant = 1

        }


            const objetoPaginadorRequest = {
            titulo: tituloVariant,
            numeroPagina: paginaVariant,
            cantidadElementos: paginadoRequest.cantidadElementos
        }

            const response = await paginacioncurso(objetoPaginadorRequest);
            setPaginadorResponse(response.data);
        }

        obtenerListaCurso();

    }, [paginadoRequest, typingBuscadorTexto]);


    const cambiarPagina = (event, nuevaPagina) => {
        setPaginadorRequest((anterior) => ({
            ...anterior,
            numeroPagina : parseInt(nuevaPagina)
        }));
    }

    const cambiarCantidadRecords = (event) => {
        setPaginadorRequest((anterior) =>({
            ...anterior,
            cantidadElementos : parseInt(event.target.value),
            numeroPagina: 0,
        }));
    };


    return (
        <div style={{padding:"10px", width:"100%"}}>
            <Grid container style={{paddingTop:"20px", paddingBottom:"20px"}}>
                <Grid item xs={12} sm={4} md={6}>
                    <TextField 
                    fullWidth
                    name="textoBusquedaCurso"
                    variant="outlined"
                    label="Busca tu Curso"
                    onChange={e => setTextoBusquedaCurso(e.target.value)}
                    />
                </Grid>
            </Grid>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell align="left">Cursos</TableCell>
                                <Hidden mdDown>
                                <TableCell align="left">Descripcion</TableCell>
                                <TableCell align="left">Fecha Publicacion</TableCell>
                                <TableCell align="left">Precio Original</TableCell>
                                <TableCell align="left">Precio Promocion</TableCell>
                                </Hidden>
                            

                        </TableRow>
                    </TableHead>
                    <TableBody>
                            {paginadorResponse.listaRecords.map((curso)=> (
                                <TableRow key={curso.titulo}>
                                    <TableCell align="left">{curso.Titulo}</TableCell>
                                    <Hidden mdDown >
                                    <TableCell align="left">{curso.Descripcion}</TableCell>
                                    <TableCell align="left">{(new Date(curso.FechaPublicacion)).toLocaleString()}</TableCell>
                                    <TableCell align="left">{curso.PrecioActual}</TableCell>
                                    <TableCell align="left">{curso.Promocion}</TableCell>
                                    </Hidden>
                                    
                                </TableRow>
                            ))}
                    </TableBody>
                </Table>
            </TableContainer>
            <TablePagination
                component="div"
                rowsPerPageOptions={[5, 10, 25]}
                count={paginadorResponse.totalRecords}
                rowsPerPage={paginadoRequest.cantidadElementos}
                page={paginadoRequest.numeroPagina}
                onChangePage = {cambiarPagina}
                onChangeRowsPerPage = {cambiarCantidadRecords}
                labelRowsPerPage = "Cursos por pagina"
            />
        </div>
    );
};

export default PaginadorCurso;