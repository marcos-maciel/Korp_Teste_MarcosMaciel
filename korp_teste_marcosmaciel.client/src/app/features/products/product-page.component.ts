import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Product, CreateProductRequest } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-product-page',
  templateUrl: './product-page.component.html',
  styleUrls: ['./product-page.component.css'],
  standalone: false
})
export class ProductPageComponent implements OnInit {
  form: FormGroup;
  products: Product[] = [];
  isLoading = false;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) {
    this.form = this.fb.group({
      codigo: ['', [Validators.required, Validators.maxLength(50)]],
      descricao: ['', [Validators.required, Validators.maxLength(200)]],
      saldo: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.productService.getAll().subscribe({
      next: (items) => {
        this.products = items;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Não foi possível carregar os produtos.';
        this.toastService.show(this.errorMessage, 'error');
        this.cdr.detectChanges();
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.errorMessage = 'Preencha os campos obrigatórios e informe um saldo válido.';
      this.toastService.show(this.errorMessage, 'error');
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    const payload: CreateProductRequest = this.form.value;

    this.productService.create(payload).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.successMessage = 'Produto cadastrado com sucesso.';
        this.toastService.show(this.successMessage, 'success');
        this.form.reset({ codigo: '', descricao: '', saldo: 0 });
        this.loadProducts();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = err?.error?.message ?? 'Não foi possível salvar o produto.';
        this.toastService.show(this.errorMessage, 'error');
        this.cdr.detectChanges();
      }
    });
  }
}
