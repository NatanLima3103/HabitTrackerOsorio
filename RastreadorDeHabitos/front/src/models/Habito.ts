export default interface Habito {
  id?: number;
  nome: string;
  descricao: string;
  usuarioId: number;
  CriadoEm?: string;
  concluidoHoje?: boolean;
}