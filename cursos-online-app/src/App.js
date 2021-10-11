import { ThemeProvider as MuiThemeProvider } from "@material-ui/core/styles";
import theme from "./theme/theme";
import RegistrarUsuario from "./componentes/seguridad/RegistrarUsuario";
import PerfilUsuario from "./componentes/seguridad/PerfilUsuario";
import Login from "./componentes/seguridad/Login";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";
import { Grid, Snackbar } from "@material-ui/core";
import AppNavBar from "./componentes/navegacion/AppNavBar";
import { useStateValue } from "./contexto/store";
import { obtenerUsuarioActual } from "./actions/UsuarioAction";
import React, { useEffect, useState } from "react";
import RutaSegura from "./componentes/navegacion/RutaSegura";
import NuevoCursos from "./componentes/cursos/NuevoCursos";
import PaginadorCurso from "./componentes/cursos/PaginadorCurso";

// import './App.css';

function App() {
  const [{ sesionUsuario, openSnackbar }, dispatch] = useStateValue();

  const [ iniciaApp, setIniciaApp ] = useState(false);

  useEffect(() => {
    if (!iniciaApp) {
      console.log('dispatch',dispatch);
      obtenerUsuarioActual(dispatch)
        .then((response) => {
          setIniciaApp(true);         
        })
        .catch((error) => {
          setIniciaApp(true);
        });
    }
  }, [iniciaApp]);

  return  iniciaApp === false ? null : (
    <React.Fragment>
      <Snackbar
       anchorOrigin={{vertical: "bottom", horizontal: "center"}}
       open= {openSnackbar ? openSnackbar.open : false}
       autoHideDuration={3000}
       ContentProps={{"aria-describedby": "message-id"}}
       message = {
         <span id="message-id">{openSnackbar ? openSnackbar.mensaje : ""}</span>
       }
       onClose= { () => 
          dispatch({
            type : "OPEN_SNACKBAR",
            openMensaje : {
              open : false,
              mensaje : ""
            }
          })
      }
      >
        
      </Snackbar>
      <Router>
        <MuiThemeProvider theme={theme}>
          <AppNavBar />
          <Grid container>
            <Switch>
              <Route exact path="/auth/login" component={Login} />
              <Route
                exact
                path="/auth/registrar"
                component={RegistrarUsuario}
              />
              
              <RutaSegura
              exact
              path = "/auth/perfil"
              component = {PerfilUsuario}

              />

              

              <RutaSegura
              exact
              path = "/"
              component = {PerfilUsuario}

              />

              <RutaSegura
                exact
                path="/curso/nuevo"
                component={NuevoCursos}
              />

              <RutaSegura
              exact
              path="/curso/paginador"
              component={PaginadorCurso}
              />


            </Switch>
          </Grid>
        </MuiThemeProvider>
      </Router>
    </React.Fragment>
  );
}

export default App;
