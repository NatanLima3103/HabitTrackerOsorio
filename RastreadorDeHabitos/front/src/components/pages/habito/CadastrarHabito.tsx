import { useEffect, useState } from "react";
import Usuario from "../../../models/Usuario";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import Habito from "../../../models/Habito";

function CadastrarHabito() {
    const [nome, setNome] = useState("");
    const [descricao, setDescricao] = useState("");
    const [usuarioId, setIdDoUsuario] = useState<number>(0);
    const navigate = useNavigate();

    useEffect(() => {
      const usuarioStorage = localStorage.getItem("usuario");
      if (!usuarioStorage) {
            // Se não houver usuário logado manda para o login
            navigate("/usuario/login");
          } else {
            const usuario: Usuario = JSON.parse(usuarioStorage);
            const usuarioId = usuario.id;
            if (usuarioId) {
              setIdDoUsuario(usuarioId);
            } else {
              console.error("Usuário não logado ou ID não encontrado!");
            }
          }
      
    }, []);

    function submeterForm(e: any) {
        e.preventDefault();
        enviarHabitoAPI();
    }

    async function enviarHabitoAPI() {
        try {
        const habito: Habito = { nome, descricao, usuarioId };
        const resposta = await axios.post(
            "http://localhost:5000/api/habitos/cadastrar",
            habito
        );
        navigate("/");
        } catch (error) {
        console.log("Erro na requisição: " + error);
        }
    }

    return (
        <div>
        <h1>Cadastrar Hábito</h1>
        <form onSubmit={submeterForm}>
            <div>
            <label>Nome:</label>
            <input type="text" onChange={(e: any) => setNome(e.target.value)} />
            </div>
            <div>
            <label>Descrição:</label>
            <input
                type="text"
                onChange={(e: any) => setDescricao(e.target.value)}
            />
            </div>
            <div>
            <button type="submit">Cadastrar</button>
            </div>
        </form>
        </div>
    );
}

export default CadastrarHabito;
