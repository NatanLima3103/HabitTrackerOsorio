import React, {useEffect, useState} from "react";
import CadastrarUsuario from "../src/components/pages/usuario/CadastrarUsuario";
import LoginUsuario from "../src/components/pages/usuario/LoginUsuario";
import ListarHabitos from "./components/pages/habito/ListarHabitos";
import { BrowserRouter, Link, Route, Routes, useNavigate, Navigate } from "react-router-dom";
import Usuario from "../src/models/Usuario";

function App() {
  //state para saber quem está logado
  const [usuarioLogado, setUsuarioLogado] = useState<Usuario | null>(null);

  //Ao carregar o App verifica se o usuário já está no localStorage
  useEffect(() => {
    const usuarioStorage = localStorage.getItem("usuario");
    if (usuarioStorage) {
      setUsuarioLogado(JSON.parse(usuarioStorage));
    }
  }, []);

  // Função que o LoginUsuario vai chamar quando o login der certo
  const handleLogin = (usuario: Usuario) => {
    setUsuarioLogado(usuario);
    localStorage.setItem("usuario", JSON.stringify(usuario));
  };

  // Função que o botão de Logout vai chamar
  const handleLogout = () => {
    setUsuarioLogado(null);
    localStorage.removeItem("usuario");
  };

return (
    <BrowserRouter>
      <div className="App">
        {/*Passa o estado de login e a função de logout para o menu */}
        <NavWrapper usuario={usuarioLogado} onLogout={handleLogout} />

        <Routes>
          <Route
            path="/usuario/cadastrar"
            element={<CadastrarUsuario />}
          />
          <Route
            path="/usuario/login"
            element={<LoginUsuario onLogin={handleLogin} />}
          />
          <Route 
            path="/"
            element={
              // Se o usuário estiver logado mostra a lista de hábitos
              usuarioLogado ? <ListarHabitos /> : 
              // Se não manda para a página de login
              <Navigate to="/usuario/login" />
            }
          />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

interface NavProps {
  usuario: Usuario | null;
  onLogout: () => void;
}

function NavWrapper({ usuario, onLogout }: NavProps) {
  const navigate = useNavigate();

  function executarLogout() {
    onLogout(); // Limpa o state e o localStorage
    navigate("/usuario/login"); // Redireciona para o login
  }

  return (
    <nav>
      <ul>
        {usuario ? (
          // Se logado, mostra o nome e o botão de deslogar
          <>
              <Link to="/">Meus Hábitos</Link>
              <button onClick={executarLogout}>Deslogar</button>
          </>
        ) : (
          // Se NÃO ESTIVER logado, mostra os links de Login e Cadastro
          <>
            <li>
              <Link to="/usuario/cadastrar">Cadastro de usuário</Link>
            </li>
            <li>
              <Link to="/usuario/login">Login de usuário</Link>
            </li>
          </>
        )}
      </ul>
    </nav>
  );
}

export default App;
