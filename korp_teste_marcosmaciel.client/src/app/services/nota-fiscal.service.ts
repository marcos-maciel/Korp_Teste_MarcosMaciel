import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateNotaFiscalRequest, NotaFiscal } from '../models/nota-fiscal';

@Injectable({ providedIn: 'root' })
export class NotaFiscalService {
  private readonly apiUrl = '/api/notas-fiscais';

  getAll(): Observable<NotaFiscal[]> {
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
        .then((data: NotaFiscal[]) => {
          observer.next(data);
          observer.complete();
        })
        .catch((error) => observer.error(error));
    });
  }

  create(request: CreateNotaFiscalRequest): Observable<NotaFiscal> {
    return new Observable((observer) => {
      fetch(this.apiUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json'
        },
        body: JSON.stringify(request)
      })
        .then(async (response) => {
          if (!response.ok) {
            const text = await response.text();
            throw new Error(text || 'Erro ao salvar nota fiscal.');
          }
          return response.json();
        })
        .then((data: NotaFiscal) => {
          observer.next(data);
          observer.complete();
        })
        .catch((error) => observer.error(error));
    });
  }

  imprimir(id: number): Observable<NotaFiscal> {
    return new Observable((observer) => {
      fetch(`${this.apiUrl}/${id}/imprimir`, {
        method: 'POST',
        headers: { Accept: 'application/json' }
      })
        .then(async (response) => {
          if (!response.ok) {
            const text = await response.text();
            throw new Error(text || 'Erro ao imprimir nota fiscal.');
          }
          return response.json();
        })
        .then((data: NotaFiscal) => {
          observer.next(data);
          observer.complete();
        })
        .catch((error) => observer.error(error));
    });
  }
}
