import { useEffect, useState } from "react";
import axios from 
 "axios";
import { useNavigate, useParams } from "react-router-dom";
import Habito from "../../../models/Habito";
import Usuario from "../../../models/Usuario";
import "../../../styles/cadastro-habito.css"; // CORREÇÃO: Volta 3 níveis para a pasta src/styles/

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

  function submeterForm(e: any) 
 {
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
    <div className="cadastro-habito-page">
      <div className="cadastro-habito-container">
        <h1 className="cadastro-habito-title">Alterar Hábito</h1>
      
        <form onSubmit={submeterForm} className="cadastro-habito-form">
            <div className="form-group">
                <label htmlFor="nome">Nome:</label>
                <input
                    id="nome"
                    value={nome}
                    type="text"
                    className="form-input"
                    onChange={(e: any) => setNome(e.target.value)}
                />
            </div>
            <div className="form-group">
                <label htmlFor="descricao">Descrição:</label>
                <input
                    id="descricao"
                    value={descricao}
                    type="text"
                    className="form-input"
                    onChange={(e: any) => setDescricao(e.target.value)}
                />
            </div>
            <div className="form-actions">
                <button type="submit" className="btn-primary">Salvar</button>
            </div>
        </form>
      </div>
    </div>
  );
}

export default AlterarHabito;