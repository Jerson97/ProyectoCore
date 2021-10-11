import { Avatar, List, ListItem, ListItemText } from '@material-ui/core';
import React from 'react';
import { Link } from 'react-router-dom';
import FotoUsuarioTemp from "../../../logo.svg";

export const MenuDerecha = ( {
    classes,
    usuario,
    salirSesion
}) => (
    <div className={classes.list}>
        <List>
            <ListItem button component={Link}>
                <Avatar src={usuario.imagenPerfil || FotoUsuarioTemp}/>
                <ListItemText classes={{primary: classes.listItemText}} primary={usuario ? usuario.nombreCompleto : ""}/>
            </ListItem>
            <ListItem button onclick={salirSesion}>
                <ListItemText classes={{primary: classes.listItemText}} primary="Salir"/>
            </ListItem>
        </List>
    </div>
)