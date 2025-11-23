import { useState, useEffect } from "react";
import Habito from "../../../models/Habito";
import Usuario from "../../../models/Usuario";
import axios from "axios";
import { Link, useNavigate } from "react-router-dom";

function ListarHabitos() {
  const [habitos, setHabitos] = useState<Habito[]>([]);
  const navigate = useNavigate();
  const [idDoUsuario, setIdDoUsuario] = useState<number | null>(null);
  const [streaks, setStreaks] = useState<number | null>(null);

  useEffect(() => {
    // pega o usuário do localStorage
    const usuarioStorage = localStorage.getItem("usuario");

    if (!usuarioStorage) {
      // Se não houver usuário logado manda para o login
      navigate("/usuario/login");
    } else {
      const usuario: Usuario = JSON.parse(usuarioStorage);
      const idDoUsuario = usuario.id;

      if (idDoUsuario) {
        setIdDoUsuario(idDoUsuario);
        listarHabitosAPI(idDoUsuario);
        exibirStreaks(idDoUsuario);
      } else {
        console.error("Usuário não logado ou ID não encontrado!");
      }
    }
  }, [navigate]);

  async function listarHabitosAPI(idDoUsuario: number) {
    try {
      const resposta = await axios.get<Habito[]>(
        `http://localhost:5000/api/habitos/usuario/${idDoUsuario}`
      );
      const dados = resposta.data;
      setHabitos(dados);
    } catch (error) {
      console.log("erro na requisição" + error);
    }
  }

  async function deletarHabitoAPI(id: number, idUsuario: number) {
    try {
      const resposta = await axios.delete(
        "http://localhost:5000/api/habitos/deletar/" + id
      );
      listarHabitosAPI(idDoUsuario!);
    } catch (error) {
      console.log("Erro na requisição: " + error);
    }
  }

  async function exibirStreaks(idDoUsuario: number) {
    try {
      const resposta = await axios.get(
        `http://localhost:5000/api/usuarios/${idDoUsuario}/streaks`
      );
      setStreaks(resposta.data.streakTotal);
    } catch (error) {
      console.log("Erro na requisição: " + error);
    }
  }

  async function marcarComoConcluido(habitoId: number) {
    if (!idDoUsuario) return;

    try {
      const resposta = await axios.post("http://localhost:5000/api/registros", {
        habitoId: habitoId,
      });
      listarHabitosAPI(idDoUsuario);
      exibirStreaks(idDoUsuario);
    } catch (error) {
      console.log("Erro ao marcar hábito: " + error);
    }
  }

  return (
    <div className="listar-habitos-page">
      <h1>Meus Hábitos</h1>

      <div id="streaks">
        <h1>Streaks</h1>
        <p>🔥{streaks !== null ? streaks : "⏳Carregando..."}</p>
      </div>

      <table className="habitos-table">
        <thead>
          {habitos.length === 0 ? (
            <tr>
              <th colSpan={6} style={{ textAlign: "center", padding: "20px" }}>
                Seus Hábitos.
              </th>
            </tr>
          ) : (
            <tr>
              <th>#</th>
              <th>Nome</th>

              <th>Descrição</th>
              <th>Ações</th>
              <th>Status de Hoje</th>
            </tr>
          )}
        </thead>
        <tbody>
          {habitos.length === 0 ? (
            <tr>
              <td colSpan={6} style={{ textAlign: "center", padding: "20px" }}>
                Nenhum hábito cadastrado ainda.
              </td>
            </tr>
          ) : (
            habitos.map((habito) => (
              <tr key={habito.id}>
                <td>{habito.id}</td>
                <td>{habito.nome}</td>
                <td>{habito.descricao}</td>
                <td>
                  <button
                    className="deletar"
                    onClick={() => deletarHabitoAPI(habito.id!, idDoUsuario!)}
                  >
                    Deletar
                  </button>
                  <Link
                    to={`habito/alterar/${habito.id}`}
                    className="link-alterar"
                  >
                    Alterar
                  </Link>
                </td>
                <td>
                  {habito.concluidoHoje ? (
                    <span className="concluido-hoje">Concluído!</span>
                  ) : (
                    <input
                      type="checkbox"
                      disabled={false}
                      onChange={() => marcarComoConcluido(habito.id!)}
                    />
                  )}
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}

export default ListarHabitos;
