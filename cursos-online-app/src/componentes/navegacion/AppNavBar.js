import React from 'react';
import { AppBar } from '@material-ui/core';
import BarSesion from './bar/BarSesion';
import { useStateValue } from '../../contexto/store';

const AppNavBar = () => {
    const [{sesionUsuario}, dispatch] = useStateValue();

    return sesionUsuario
    ? (sesionUsuario.autenticado === true ? <AppBar position="static"><BarSesion/></AppBar> : null)
    : null


    // return (
    //     <AppBar position="static">
    //         <BarSesion/>
    //     </AppBar>
        
    // );
};

export default AppNavBar;