import { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate, useParams } from "react-router-dom";
import Habito from "../../../models/Habito";
import Usuario from "../../../models/Usuario";

function AlterarHabito() {
  const { habitoId } = useParams();
  const [nome, setNome] = useState("");
  const [descricao, setDescricao] = useState("");
  const [idDoUsuario, setIdDoUsuario] = useState<number | null>(null);
  const navigate = useNavigate();
  useEffect(() => {
    const usuarioStorage = localStorage.getItem("usuario");
    if (!usuarioStorage) {
          // Se não houver usuário logado manda para o login
          navigate("/usuario/login");
        } else {
          const usuario: Usuario = JSON.parse(usuarioStorage);
          const idDoUsuario = usuario.id;
          if (idDoUsuario) {
            setIdDoUsuario(idDoUsuario);
            if (!habitoId || isNaN(Number(habitoId))) {
                console.error("ID de hábito inválido:", habitoId);
                navigate("/"); // volta para home ou página de erro
                return;
            }
            buscarHabitoAPI(Number(habitoId));
          } else {
            console.error("Usuário não logado ou ID não encontrado!");
          }
        }
    
  }, []);

  async function buscarHabitoAPI(habitoId: number) {
    try {
      const resposta = await axios.get<Habito>(
        `http://localhost:5000/api/habitos/buscar/${habitoId}`
      );
      console.log("Resposta buscar:", resposta.data);
      setNome(resposta.data.nome);
      setDescricao(resposta.data.descricao);
    } catch (error) {
      console.log("Erro na requisição: " + error);
    }
  }

  function submeterForm(e: any) {
    e.preventDefault();
    enviarHabitoAPI();
  }

  async function enviarHabitoAPI() {
    try {
      const habito = {
            nome,
            descricao
      };
      const resposta = await axios.patch(
        "http://localhost:5000/api/habitos/alterar/" + habitoId,
        habito
      );
      navigate("/");
    } catch (error) {
      console.log("Erro na requisição: " + error);
    }
  }

  return (
    <div>
      <h1>Alterar Produto</h1>
      <form onSubmit={submeterForm}>
        <div>
          <label>Nome:</label>
          <input
            value={nome}
            type="text"
            onChange={(e: any) => setNome(e.target.value)}
          />
        </div>
        <div>
          <label>Descrição:</label>
          <input
            value={descricao}
            type="text"
            onChange={(e: any) => setDescricao(e.target.value)}
          />
        </div>
        <div>
          <button type="submit">Salvar</button>
        </div>
      </form>
    </div>
  );
}

export default AlterarHabito;
