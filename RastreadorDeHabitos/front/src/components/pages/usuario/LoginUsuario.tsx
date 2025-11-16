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
    <div>
      <h1>Login Usuário</h1>
      <form onSubmit={submeterForm}>
        <div>
          <label>Email:</label>
          <input type="email" onChange={(e: any) => setEmail(e.target.value)} required />
        </div>
        <div>
          <label>Senha:</label>
          <input type="password" onChange={(e: any) => setSenha(e.target.value)} required/>
        </div>
        <div>
          <button type="submit">Login</button>
        </div>
      </form>
    </div>
  );
}

export default LoginUsuario;