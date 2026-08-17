import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateProductRequest, Product } from '../models/product';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly apiUrl = '/api/products';

  getAll(): Observable<Product[]> {
    return new Observable((observer) => {
      fetch(this.apiUrl, {
        method: 'GET',
        headers: { Accept: 'application/json' }
      })
        .then(async (response) => {
          if (!response.ok) {
            throw new Error(await response.text());
          }
          return response.json();
        })
        .then((data: Product[]) => {
          observer.next(data);
          observer.complete();
        })
        .catch((error) => observer.error(error));
    });
  }

  create(product: CreateProductRequest): Observable<Product> {
    return new Observable((observer) => {
      fetch(this.apiUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json'
        },
        body: JSON.stringify(product)
      })
        .then(async (response) => {
          if (!response.ok) {
            const body = await response.text();
            throw new Error(body || 'Erro ao salvar produto.');
          }
          return response.json();
        })
        .then((data: Product) => {
          observer.next(data);
          observer.complete();
        })
        .catch((error) => observer.error(error));
    });
  }
}
