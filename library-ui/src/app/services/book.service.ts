import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Book, PagedResult, BookSearchRequest, CreateLoanRequest } from '../models/book.model';

declare global {
  interface Window {
    __env?: { apiUrl?: string };
  }
}

function getApiUrl(): string {
  return '/api';
}

@Injectable({ providedIn: 'root' })
export class BookService {
  private http = inject(HttpClient);
  // Résolu une seule fois au démarrage — après que env.js est chargé
  private readonly api = getApiUrl();

  searchBooks(req: BookSearchRequest): Observable<PagedResult<Book>> {
    let params = new HttpParams()
      .set('onlyAvailable', req.onlyAvailable.toString())
      .set('page', req.page.toString())
      .set('pageSize', req.pageSize.toString());

    if (req.searchTerm) params = params.set('searchTerm', req.searchTerm);
    if (req.category && req.category !== 'Unknown') params = params.set('category', req.category);

    return this.http.get<PagedResult<Book>>(`${this.api}/books`, { params });
  }

  createLoan(request: CreateLoanRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${this.api}/loans`, request);
  }

  returnBook(loanId: string): Observable<void> {
    return this.http.post<void>(`${this.api}/loans/${loanId}/return`, {});
  }

  importClassic(file: File): Observable<{ message: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ message: string }>(`${this.api}/catalog/import/classic`, formData);
  }

  importFast(file: File): Observable<{ message: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ message: string }>(`${this.api}/catalog/import/fast`, formData);
  }
}
