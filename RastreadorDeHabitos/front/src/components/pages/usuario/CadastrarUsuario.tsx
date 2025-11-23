import { useState } from "react";
import Usuario from "../../../models/Usuario";
import axios from "axios";
import { useNavigate } from "react-router-dom";

// importa o CSS desta tela
import "../../../styles/cadastro-usuario.css";

function CadastrarUsuario() {
    const [nome, setNome] = useState("");
    const [email, setEmail] = useState("");
    const [senha, setSenha] = useState("");
    const navigate = useNavigate();

    function submeterForm(e: any) {
        e.preventDefault();
        enviarUsuarioAPI();
    }

    async function enviarUsuarioAPI() {
        try {
            const usuario: Usuario = { nome, email, senha };
            const resposta = await axios.post(
                "http://localhost:5000/api/usuario/cadastrar",
                usuario
            );
            navigate("/");
        } catch (error) {
            console.log("Erro de requisição: " + error);
        }
    }

    return (
        <div className="cadastro-usuario-page">
            <div className="cadastro-usuario-container">
                <h1 className="cadastro-usuario-title">Cadastrar Usuário</h1>

                <form onSubmit={submeterForm} className="cadastro-usuario-form">
                    <div className="form-group">
                        <label htmlFor="nome">Nome</label>
                        <input
                            id="nome"
                            type="text"
                            className="form-input"
                            onChange={(e: any) => setNome(e.target.value)}
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="email">Email</label>
                        <input
                            id="email"
                            type="email"
                            className="form-input"
                            onChange={(e: any) => setEmail(e.target.value)}
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="senha">Senha</label>
                        <input
                            id="senha"
                            type="password"
                            className="form-input"
                            onChange={(e: any) => setSenha(e.target.value)}
                        />
                    </div>

                    <div className="form-actions">
                        <button type="submit" className="btn-primary">
                            Cadastrar
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default CadastrarUsuario;
