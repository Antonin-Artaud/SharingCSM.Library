import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../services/book.service';
import { Book, BookCategory, PagedResult } from '../../models/book.model';
import { debounceTime, Subject, switchMap, catchError, of, finalize } from 'rxjs';

interface LoanState {
  bookId: string;
  loading: boolean;
  loanId?: string;
  error?: string;
}

@Component({
  selector: 'app-books',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page">
      <div class="page-header">
        <div>
          <h1>Catalogue</h1>
          <p class="subtitle">Recherche et emprunt de livres</p>
        </div>
        <div *ngIf="result()" class="stat-chip">
          {{ result()!.totalCount }} livre{{ result()!.totalCount > 1 ? 's' : '' }}
        </div>
      </div>

      <!-- Search -->
      <div class="card search-card">
        <div class="search-fields">
          <div class="field">
            <label>Recherche</label>
            <div class="input-icon-wrap">
              <svg class="input-icon" width="14" height="14" viewBox="0 0 14 14" fill="none">
                <circle cx="6" cy="6" r="4.5" stroke="currentColor" stroke-width="1.2"/>
                <path d="M9.5 9.5L13 13" stroke="currentColor" stroke-width="1.2" stroke-linecap="round"/>
              </svg>
              <input [(ngModel)]="searchTerm" (ngModelChange)="onSearch()" placeholder="Titre du livre…" />
            </div>
          </div>
          <div class="field">
            <label>Catégorie</label>
            <select [(ngModel)]="category" (ngModelChange)="onSearch()">
              <option value="Unknown">Toutes les catégories</option>
              <option value="SciFi">Science-Fiction</option>
              <option value="Fantasy">Fantasy</option>
            </select>
          </div>
          <div class="field field-toggle">
            <label>Disponibles seulement</label>
            <label class="toggle">
              <input type="checkbox" [(ngModel)]="onlyAvailable" (ngModelChange)="onSearch()" />
              <span class="track"><span class="thumb"></span></span>
            </label>
          </div>
        </div>
      </div>

      <!-- Loading -->
      <div *ngIf="loading()" class="loading-row">
        <span class="spinner"></span>
        <span class="loading-txt">Chargement…</span>
      </div>

      <!-- Empty -->
      <div *ngIf="!loading() && result()?.items?.length === 0" class="empty-state">
        <div class="icon">◎</div>
        <p>Aucun livre trouvé pour ces critères</p>
      </div>

      <!-- Grid -->
      <div *ngIf="!loading() && (result()?.items?.length ?? 0) > 0" class="books-grid fade-in">
        <div *ngFor="let book of result()!.items" class="book-card" [class.borrowed]="!book.isAvailable">

          <div class="book-header">
            <span [class]="'badge badge-' + book.category.toLowerCase()">
              {{ getCategoryLabel(book.category) }}
            </span>
            <span [class]="'badge badge-' + (book.isAvailable ? 'available' : 'unavailable')">
              {{ book.isAvailable ? 'Disponible' : 'Emprunté' }}
            </span>
          </div>

          <div class="book-title">{{ book.title }}</div>

          <div class="book-footer">
            <ng-container *ngIf="getLoanState(book.id) as ls">

              <div *ngIf="ls.loanId" class="loan-ok">
                <svg width="13" height="13" viewBox="0 0 13 13" fill="none">
                  <path d="M2 6.5L5.5 10L11 3" stroke="var(--success)" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                <span class="mono" style="font-size:10px; color:var(--success)">{{ ls.loanId.slice(0,8) }}…</span>
              </div>

              <div *ngIf="ls.error" class="loan-err" [title]="ls.error">
                <svg width="13" height="13" viewBox="0 0 13 13" fill="none">
                  <circle cx="6.5" cy="6.5" r="5" stroke="var(--danger)" stroke-width="1.2"/>
                  <path d="M6.5 4v3" stroke="var(--danger)" stroke-width="1.2" stroke-linecap="round"/>
                  <circle cx="6.5" cy="9" r="0.6" fill="var(--danger)"/>
                </svg>
                <span style="font-size:11px;color:var(--danger)">Indisponible</span>
              </div>

              <button
                *ngIf="!ls.loanId && !ls.error"
                class="btn btn-primary btn-sm"
                [disabled]="!book.isAvailable || ls.loading"
                (click)="borrowBook(book)"
              >
                <span *ngIf="ls.loading" class="spinner" style="width:11px;height:11px;border-width:1.5px;"></span>
                <span *ngIf="!ls.loading">Emprunter</span>
              </button>

            </ng-container>
          </div>
        </div>
      </div>

      <!-- Pagination -->
      <div *ngIf="result() && result()!.totalCount > pageSize" class="pagination">
        <button class="btn btn-ghost btn-sm" [disabled]="page === 1" (click)="goToPage(page - 1)">← Préc.</button>
        <span class="mono" style="font-size:12px;color:var(--text-muted)">{{ page }} / {{ totalPages() }}</span>
        <button class="btn btn-ghost btn-sm" [disabled]="page >= totalPages()" (click)="goToPage(page + 1)">Suiv. →</button>
      </div>
    </div>

    <!-- User pill fixed -->
    <div class="user-pill">
      <div class="user-dot"></div>
      <span class="mono" style="font-size:10px;color:var(--text-muted)">{{ userId.slice(0,8) }}…</span>
      <button class="btn btn-ghost btn-sm" style="padding:3px 8px;font-size:10px;" (click)="refreshUserId()">↺</button>
    </div>
  `,
  styles: [`
    .page { padding: 32px; max-width: 1100px; }

    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 22px;
      h1 { font-size: 26px; margin-bottom: 3px; }
      .subtitle { font-size: 13px; color: var(--text-muted); }
    }

    .stat-chip {
      background: var(--accent-light);
      color: var(--accent);
      padding: 4px 12px;
      border-radius: 20px;
      font-size: 12px;
      font-weight: 600;
      border: 1px solid var(--accent-dim);
    }

    .search-card { margin-bottom: 24px; }

    .search-fields {
      display: flex;
      gap: 16px;
      align-items: flex-end;
      flex-wrap: wrap;
    }

    .field {
      display: flex;
      flex-direction: column;
      gap: 5px;
      flex: 1;
      min-width: 160px;

      label {
        font-size: 11px;
        text-transform: uppercase;
        letter-spacing: 0.06em;
        color: var(--text-muted);
        font-family: 'JetBrains Mono', monospace;
      }
    }

    .field-toggle {
      flex: 0 0 auto;
      flex-direction: row;
      align-items: center;
      gap: 10px;
      padding-bottom: 1px;
    }

    .input-icon-wrap {
      position: relative;
      .input-icon {
        position: absolute;
        left: 12px; top: 50%;
        transform: translateY(-50%);
        color: var(--text-dim);
        pointer-events: none;
      }
      input { padding-left: 34px; }
    }

    .toggle {
      display: flex;
      align-items: center;
      cursor: pointer;
      input { display: none; }
      &:has(input:checked) .track {
        background: var(--accent);
        .thumb { transform: translateX(14px); background: #fff; }
      }
    }

    .track {
      width: 30px; height: 16px;
      background: var(--bg-4);
      border: 1px solid var(--border-strong);
      border-radius: 8px;
      position: relative;
      transition: all 0.15s;
    }

    .thumb {
      position: absolute;
      left: 2px; top: 2px;
      width: 10px; height: 10px;
      border-radius: 50%;
      background: var(--text-muted);
      transition: all 0.15s;
    }

    .loading-row {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 32px;
      .loading-txt { font-size: 13px; color: var(--text-muted); }
    }

    .books-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
      gap: 12px;
      margin-bottom: 24px;
    }

    .book-card {
      background: var(--bg-2);
      border: 1px solid var(--border);
      border-radius: var(--radius-lg);
      padding: 16px;
      display: flex;
      flex-direction: column;
      gap: 10px;
      transition: border-color 0.15s, box-shadow 0.15s;
      box-shadow: 0 1px 3px rgba(0,0,0,0.04);

      &:hover { border-color: var(--border-strong); box-shadow: 0 2px 8px rgba(0,0,0,0.06); }
      &.borrowed { opacity: 0.65; }
    }

    .book-header {
      display: flex;
      gap: 6px;
      flex-wrap: wrap;
    }

    .book-title {
      font-size: 14px;
      font-weight: 600;
      line-height: 1.35;
      color: var(--text);
      flex: 1;
    }

    .book-footer {
      display: flex;
      justify-content: flex-end;
      align-items: center;
      margin-top: auto;
    }

    .loan-ok, .loan-err {
      display: flex;
      align-items: center;
      gap: 5px;
    }

    .pagination {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 12px;
      padding-top: 20px;
      border-top: 1px solid var(--border);
    }

    .user-pill {
      position: fixed;
      bottom: 20px; right: 24px;
      display: flex;
      align-items: center;
      gap: 8px;
      background: var(--bg-2);
      border: 1px solid var(--border-strong);
      border-radius: 20px;
      padding: 6px 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08);
      z-index: 100;
    }

    .user-dot {
      width: 7px; height: 7px;
      border-radius: 50%;
      background: var(--success);
      flex-shrink: 0;
    }
  `]
})
export class BooksComponent implements OnInit {
  private bookService = inject(BookService);

  searchTerm = '';
  category: BookCategory = 'Unknown';
  onlyAvailable = false;
  page = 1;
  pageSize = 12;

  loading = signal(false);
  result = signal<PagedResult<Book> | null>(null);
  loanStates = signal<Map<string, LoanState>>(new Map());
  userId = crypto.randomUUID();

  private searchSubject = new Subject<void>();

  totalPages = computed(() => {
    if (!this.result()) return 1;
    return Math.max(1, Math.ceil(this.result()!.totalCount / this.pageSize));
  });

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(300),
      switchMap(() => {
        this.loading.set(true);
        return this.bookService.searchBooks({
          searchTerm: this.searchTerm || undefined,
          category: this.category,
          onlyAvailable: this.onlyAvailable,
          page: this.page,
          pageSize: this.pageSize
        }).pipe(
          catchError(() => of(null)),
          finalize(() => this.loading.set(false))
        );
      })
    ).subscribe(r => this.result.set(r));

    this.triggerSearch();
  }

  onSearch() { this.page = 1; this.searchSubject.next(); }
  triggerSearch() { this.searchSubject.next(); }
  goToPage(p: number) { this.page = p; this.triggerSearch(); }

  getLoanState(bookId: string): LoanState {
    return this.loanStates().get(bookId) ?? { bookId, loading: false };
  }

  borrowBook(book: Book) {
    const s = new Map(this.loanStates());
    s.set(book.id, { bookId: book.id, loading: true });
    this.loanStates.set(s);

    this.bookService.createLoan({ bookId: book.id, userId: this.userId }).subscribe({
      next: (res) => {
        const s2 = new Map(this.loanStates());
        s2.set(book.id, { bookId: book.id, loading: false, loanId: res.id });
        this.loanStates.set(s2);
        this.persistLoan({ loanId: res.id, bookTitle: book.title, userId: this.userId });
        setTimeout(() => this.triggerSearch(), 500);
      },
      error: (err) => {
        const s2 = new Map(this.loanStates());
        const msg = err?.error?.detail || err?.error?.message || 'Indisponible';
        s2.set(book.id, { bookId: book.id, loading: false, error: msg });
        this.loanStates.set(s2);
      }
    });
  }

  private persistLoan(l: { loanId: string; bookTitle: string; userId: string }) {
    try {
      const raw = sessionStorage.getItem('library-loans');
      const list = raw ? JSON.parse(raw) : [];
      list.unshift({ loanId: l.loanId, bookTitle: l.bookTitle, userId: l.userId, borrowedAt: new Date().toISOString(), status: 'active' });
      sessionStorage.setItem('library-loans', JSON.stringify(list));
    } catch { /* ignore */ }
  }

  refreshUserId() { this.userId = crypto.randomUUID(); }

  getCategoryLabel(cat: BookCategory): string {
    const labels: Record<BookCategory, string> = { SciFi: 'Sci-Fi', Fantasy: 'Fantasy', Unknown: '—' };
    return labels[cat] ?? cat;
  }
}
