import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { NotaFiscal } from '../../models/nota-fiscal';
import { NotaFiscalService } from '../../services/nota-fiscal.service';
import { ProductService } from '../../services/product.service';
import { ToastService } from '../../services/toast.service';

interface NotaFiscalItemDraft {
  produtoId: number;
  quantidade: number;
  produto?: Product;
}

@Component({
  selector: 'app-nota-fiscal-page',
  templateUrl: './nota-fiscal-page.component.html',
  styleUrls: ['./nota-fiscal-page.component.css'],
  standalone: false
})
export class NotaFiscalPageComponent implements OnInit {
  products: Product[] = [];
  notas: NotaFiscal[] = [];
  itens: NotaFiscalItemDraft[] = [];
  selectedProductId = '';
  quantidade = 1;
  isLoadingProducts = false;
  isLoadingNotas = false;
  isSaving = false;
  isPrintingIds: number[] = [];
  errorMessage = '';
  successMessage = '';

  constructor(
    private productService: ProductService,
    private notaFiscalService: NotaFiscalService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadNotas();
  }

  loadProducts(): void {
    this.isLoadingProducts = true;
    this.productService.getAll().subscribe({
      next: (items) => {
        this.products = items;
        this.isLoadingProducts = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoadingProducts = false;
        this.errorMessage = 'Não foi possível carregar os produtos disponíveis.';
        this.toastService.show(this.errorMessage, 'error');
        this.cdr.detectChanges();
      }
    });
  }

  loadNotas(): void {
    this.isLoadingNotas = true;
    this.notaFiscalService.getAll().subscribe({
      next: (items) => {
        this.notas = items;
        this.isLoadingNotas = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoadingNotas = false;
        this.errorMessage = 'Não foi possível carregar as notas fiscais.';
        this.toastService.show(this.errorMessage, 'error');
        this.cdr.detectChanges();
      }
    });
  }

  addItem(): void {
    const produtoId = Number(this.selectedProductId);

    if (!produtoId || this.quantidade <= 0) {
      this.errorMessage = 'Selecione um produto e informe uma quantidade maior que zero.';
      this.toastService.show(this.errorMessage, 'error');
      return;
    }

    const produto = this.products.find((item) => item.id === produtoId);
    if (!produto) {
      this.errorMessage = 'Produto inválido.';
      this.toastService.show(this.errorMessage, 'error');
      return;
    }

    const existing = this.itens.find((item) => item.produtoId === produtoId);
    if (existing) {
      existing.quantidade += this.quantidade;
    } else {
      this.itens.push({
        produtoId,
        quantidade: this.quantidade,
        produto
      });
    }

    this.selectedProductId = '';
    this.quantidade = 1;
    this.errorMessage = '';
    this.successMessage = '';
    this.toastService.show('Item adicionado com sucesso.', 'info');
  }

  removeItem(produtoId: number): void {
    this.itens = this.itens.filter((item) => item.produtoId !== produtoId);
  }

  salvarNota(): void {
    if (this.itens.length === 0) {
      this.errorMessage = 'Adicione pelo menos um item antes de salvar a nota.';
      this.toastService.show(this.errorMessage, 'error');
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const payload = {
      itens: this.itens.map((item) => ({
        produtoId: item.produtoId,
        quantidade: item.quantidade
      }))
    };

    this.notaFiscalService.create(payload).subscribe({
      next: () => {
        this.isSaving = false;
        this.successMessage = 'Nota fiscal salva com sucesso.';
        this.toastService.show(this.successMessage, 'success');
        this.itens = [];
        this.selectedProductId = '';
        this.quantidade = 1;
        this.loadProducts();
        this.loadNotas();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isSaving = false;
        this.errorMessage = err?.error?.message ?? 'Não foi possível salvar a nota fiscal.';
        this.toastService.show(this.errorMessage, 'error');
        this.cdr.detectChanges();
      }
    });
  }

  imprimirNota(notaFiscal: NotaFiscal): void {
    if (!notaFiscal.id || notaFiscal.status === 'Fechada') {
      return;
    }

    this.isPrintingIds = [...this.isPrintingIds, notaFiscal.id];
    this.errorMessage = '';
    this.successMessage = '';

    this.notaFiscalService.imprimir(notaFiscal.id).subscribe({
      next: () => {
        this.successMessage = 'Nota impressa com sucesso. Estoque atualizado.';
        this.toastService.show(this.successMessage, 'success');
        this.isPrintingIds = this.isPrintingIds.filter((id) => id !== notaFiscal.id);
        this.loadProducts();
        this.loadNotas();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isPrintingIds = this.isPrintingIds.filter((id) => id !== notaFiscal.id);
        this.errorMessage = err?.error?.message ?? 'Não foi possível imprimir a nota fiscal.';
        this.toastService.show(this.errorMessage, 'error');
        this.cdr.detectChanges();
      }
    });
  }

  formatDate(value?: string): string {
    if (!value) {
      return '—';
    }

    return new Date(value).toLocaleString('pt-BR');
  }
}
