import { useState } from "react";
import Usuario from "../../../models/Usuario";
import axios from "axios";
import { useNavigate } from "react-router-dom";

function CadastrarUsuario(){
    const [nome, setNome] = useState("");
    const [email, setEmail] = useState("");
    const [senha, setSenha] = useState("");
    const navigate = useNavigate();

    function submeterForm(e: any) {
        e.preventDefault();
        enviarUsuarioAPI();
    }

    async function enviarUsuarioAPI() {
        try{
            const usuario: Usuario = {nome, email, senha};
            const resposta = await axios.post(
                "http://localhost:5000/api/usuario/cadastrar",
                usuario
            );
            navigate("/");
        }
        catch(error){
            console.log("Erro de requisição: " + error)
        }
    }

    return(
        <div>
        <h1>Cadastrar Usuário</h1>
        <form onSubmit={submeterForm}>
            <div>
            <label>Nome:</label>
            <input
                type="text"
                onChange={(e: any) => setNome(e.target.value)}
            />
            </div>
            <div>
            <label>Email:</label>
            <input
                type="text"
                onChange={(e: any) => setEmail(e.target.value)}
            />
            </div>
            <div>
            <label>Senha:</label>
            <input
                type="password"
                onChange={(e: any) => setSenha(e.target.value)}
            />
            </div>
            <div>
            <button type="submit">Cadastrar</button>
            </div>
        </form>
        </div>
    );
}

export default CadastrarUsuario;