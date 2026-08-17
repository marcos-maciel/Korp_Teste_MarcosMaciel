export interface Product {
  id: number;
  codigo: string;
  descricao: string;
  saldo: number;
  criadoEm?: string;
  atualizadoEm?: string;
}

export interface CreateProductRequest {
  codigo: string;
  descricao: string;
  saldo: number;
}
