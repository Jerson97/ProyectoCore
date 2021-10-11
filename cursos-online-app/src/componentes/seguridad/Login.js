import { Avatar, Button, Container, TextField, Typography } from '@material-ui/core';
import React, { useState } from 'react';
import style from '../Tool/Style';
import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import { loginUsuario } from '../../actions/UsuarioAction';
import { withRouter } from 'react-router-dom';
import { useStateValue } from '../../contexto/store';


const Login = (props) =>{
    const [{usuarioSesion}, dispatch] = useStateValue();
    const [usuario, setUsuario] = useState({
        Email : '',
        password: '',
    })
    const ingresarValoresMemoria = e => {
        const {name, value} = e.target;
        setUsuario( anterior => ({
            ...anterior,
            [name] : value
        }))
    }

    const loginUsuarioBoton = e => {
        e.preventDefault();
        loginUsuario(usuario, dispatch).then(response => {
            console.log('response.data.token', response.data.token);
            if(response.status === 200) {
                window.localStorage.setItem('token_seguridad', response.data.token);
                props.history.push("/auth/perfil");
            }else{
                dispatch({
                    type : "OPEN_SNACKBAR",
                    openMensaje : {
                        open : true,
                        mensaje : "Las credenciales del usuario son incorrectas"
                    }
                })
            }

            
            

        })
    }
    

    return (
        <Container maxWidth="xs">
            <div style={style.paper}>
                <Avatar style={style.avatar}>
                    <LockOutlinedIcon style={style.icon}/>
                </Avatar>
                <Typography component="h1" variant="h5">
                    Login de Usuario
                </Typography>
                <form style={style.form}>
                    <TextField value={usuario.Email} onChange={ingresarValoresMemoria} variant="outlined" fullWidth label="Ingrese username" name="Email" margin="normal"/>
                    <TextField value={usuario.password} onChange={ingresarValoresMemoria} variant="outlined" type="password" name="password" fullWidth label="Ingrese Password" margin="normal"/>

                    <Button onClick={loginUsuarioBoton} type="submit" fullWidth variant="contained" color="primary" style={style.submit}>Enviar</Button>
                </form>
            </div>
        </Container>
    );
};

export default withRouter(Login);