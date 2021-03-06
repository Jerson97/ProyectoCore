import {
  IconButton,
  Toolbar,
  Typography,
  makeStyles,
  Button,
  Avatar,
  Drawer,
  List,
  ListItem,
  ListItemText,
} from "@material-ui/core";
import React, { useState } from "react";
import { withRouter } from "react-router-dom";
import { useStateValue } from "../../../contexto/store";
//import theme from '../../../theme/theme';
import FotoUsuarioTemp from "../../../logo.svg";
import { MenuIzquierda } from "./menuIzquierda";
import {MenuDerecha} from './menuDerecha';

const useStyles = makeStyles((theme) => ({
  seccionDesktop: {
    display: "none",
    [theme.breakpoints.up("md")]: {
      display: "flex",
    },
  },
  seccionMobile: {
    display: "flex",
    [theme.breakpoints.up("md")]: {
      display: "none",
    },
  },
  grow: {
    flexGrow: 1,
  },
  avatarSize: {
    width: 40,
    heigth: 40,
  },
  list: {
    width: 250,
  },
  listItemText: {
    fontSize: "14px",
    fontWeight: 600,
    paddingLeft: "15",
    color: "#212121",
  },
}));

const BarSesion = (props) => {
  const classes = useStyles();
  const [{ sesionUsuario }, dispatch] = useStateValue(); // Si tengo importado arriba

  const [abrirMenuIzquierda, setAbrirMenuIzquierda] = useState(false);

  const [abrirMenuDerecha, setAbrirMenuDerecha] = useState(false);

  const cerrarMenuIzquierda = () => {
    setAbrirMenuIzquierda(false);
  };

  const abrirMenuIzquierdaAction = () => {
    setAbrirMenuIzquierda(true);
  };
  const cerrarMenuDerecha = () => {
    setAbrirMenuDerecha(false);
  };
  const salirSesionApp = () => {
    console.log('salir sesion');
    localStorage.removeItem("token_seguridad");

    dispatch(
      {
        type : "SALIR_SESION",
        nuevoUsuario : null,
        autenticado : false
      }
    )
    props.history.push("/auth/login");
  };
  const abrirMenuDerechaAction = () => {
      setAbrirMenuDerecha(true);
  }

  return (
    <React.Fragment>
      <Drawer
        open={abrirMenuIzquierda}
        onClose={cerrarMenuIzquierda}
        anchor="left"
      >
        <div
          className={classes.list}
          onKeyDown={cerrarMenuIzquierda}
          onClick={cerrarMenuIzquierda}
        >
          <MenuIzquierda classes={classes} />
        </div>
      </Drawer>

      <Drawer
        open={abrirMenuDerecha}
        onClose={cerrarMenuDerecha}
        anchor="right"
      >
        <div
          role="button"
          onClick={cerrarMenuDerecha}
          onKeyDown={cerrarMenuDerecha}
        >
          <MenuDerecha
           classes={classes}
            salirSesion={salirSesionApp}
            usuario = { sesionUsuario ? sesionUsuario.usuario : null}
            />
        </div>
      </Drawer>

      <Toolbar>
        <IconButton color="inherit" onClick={abrirMenuIzquierdaAction}>
          <i className="material-icons">menu</i>
        </IconButton>
        <Typography variant="h6">Cursos Online</Typography>
        <div className={classes.grow}></div>

        <div className={classes.seccionDesktop}>
          <Button color="inherit" onClick={salirSesionApp}>Salir</Button>
          <Button color="inherit">
            {sesionUsuario ? sesionUsuario.usuario.nombreCompleto : ""}
          </Button>
          <Avatar src={sesionUsuario.usuario.imagenPerfil || FotoUsuarioTemp}></Avatar>
        </div>

        <div className={classes.seccionMobile}>
          <IconButton color="inherit" onClick={abrirMenuDerechaAction}>
            <i className="material-icons">more_vert</i>
          </IconButton>
        </div>
      </Toolbar>
    </React.Fragment>
  );
};

export default withRouter(BarSesion);
