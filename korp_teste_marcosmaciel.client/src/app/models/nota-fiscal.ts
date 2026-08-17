export interface NotaFiscalItem {
  id?: number;
  produtoId: number;
  quantidade: number;
  produto?: {
    id: number;
    codigo: string;
    descricao: string;
    saldo: number;
  };
}

export interface NotaFiscal {
  id?: number;
  numero?: number;
  status?: 'Aberta' | 'Fechada';
  itens: NotaFiscalItem[];
  criadoEm?: string;
  atualizadoEm?: string;
}

export interface CreateNotaFiscalRequest {
  itens: NotaFiscalItem[];
}
