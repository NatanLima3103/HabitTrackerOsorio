import { useState } from "react";
import Usuario from "../../../models/Usuario";
import axios from "axios";
import { useNavigate } from "react-router-dom";

interface LoginProps {
  onLogin: (usuario: Usuario) => void;
}

function LoginUsuario({ onLogin }: LoginProps) {
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const navigate = useNavigate();

  function submeterForm(e: any) {
    e.preventDefault();
    loginUsuarioAPI();
  }

  async function loginUsuarioAPI() {
    try {
      const usuarioLogado: Usuario = { email, senha };
      const respostaLogin = await axios.post(
        "http://localhost:5000/api/usuario/login",
        usuarioLogado
      );
      //retorno da api
      const usuarioAutenticado: Usuario = respostaLogin.data;
      onLogin(usuarioAutenticado);

      navigate("/");
    } catch (error) {
      console.log("Erro na requisição: " + error);
      alert("Email ou senha inválidos.");
    }
  }

  return (
    <div className="login-usuario-page">
      <div className="login-usuario-container">
        <h1 className="login-usuario-title">Login</h1>

        <form onSubmit={submeterForm} className="login-usuario-form">
          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input
              id="email"
              type="email"
              className="form-input"
              onChange={(e: any) => setEmail(e.target.value)}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="senha">Senha</label>
            <input
              id="senha"
              type="password"
              className="form-input"
              onChange={(e: any) => setSenha(e.target.value)}
              required
            />
          </div>

          <div className="form-actions">
            <button type="submit" className="btn-primary">
              Login
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default LoginUsuario;
