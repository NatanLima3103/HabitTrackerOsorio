import { useState, useEffect } from "react";
import Habito from "../../../models/Habito";
import Usuario from "../../../models/Usuario";
import axios from "axios";
import { useNavigate } from "react-router-dom";

function ListarHabitos() {
    const [habitos, setHabitos] = useState<Habito[]>([]);
    const navigate = useNavigate();

    // useEffect é o local correto para carregar dados da API
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
            listarHabitosAPI(idDoUsuario);
        } else {
            console.error("Usuário não logado ou ID não encontrado!");
        }
        }
    }, [navigate]);

    async function listarHabitosAPI(idDoUsuario: number) {
        try{
            const resposta =await axios.get<Habito[]>(
                `http://localhost:5000/api/habitos/usuario/${idDoUsuario}`
            )
            const dados = resposta.data;
            setHabitos(dados);
        }
        catch(error){
            console.log("erro na requisição" + error)
        }
    };
    return (
        <div id="componente_listar_habitos">
        <h1>Habitos</h1>
        <table>
            <thead>
            <tr>
                <th>#</th>
                <th>Nome</th>
                <th>Descrição</th>
            </tr>
            </thead>
            <tbody>
            {habitos.map((habito) => (
                <tr key={habito.id}>
                <td>{habito.id}</td>
                <td>{habito.nome}</td>
                <td>{habito.descricao}</td>
                </tr>
            ))}
            </tbody>
        </table>
        </div>
  );
}

export default ListarHabitos;