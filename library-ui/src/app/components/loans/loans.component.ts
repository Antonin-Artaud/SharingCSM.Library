import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../services/book.service';
import { catchError, finalize, EMPTY } from 'rxjs';

interface LoanEntry {
  loanId: string;
  bookTitle: string;
  userId: string;
  borrowedAt: Date;
  status: 'active' | 'returning' | 'returned' | 'error';
  error?: string;
}

@Component({
  selector: 'app-loans',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page">
      <div class="page-header">
        <div>
          <h1>Emprunts</h1>
          <p class="subtitle">Retourner un livre emprunté</p>
        </div>
      </div>

      <!-- Manual return -->
      <div class="card return-card">
        <div class="card-label">Retour par Loan ID</div>
        <div class="form-row">
          <input
            [(ngModel)]="manualLoanId"
            placeholder="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
            class="mono"
            style="font-size:12px"
          />
          <button
            class="btn btn-primary"
            [disabled]="!manualLoanId.trim() || returning()"
            (click)="returnManual()"
          >
            <span *ngIf="returning()" class="spinner" style="width:13px;height:13px;border-width:1.5px;"></span>
            <span *ngIf="!returning()">Retourner</span>
          </button>
        </div>
        <div *ngIf="manualResult()" class="alert mt-8"
          [class.alert-success]="manualResult()!.ok"
          [class.alert-error]="!manualResult()!.ok">
          {{ manualResult()!.message }}
        </div>
      </div>

      <!-- Session history -->
      <div class="section-row">
        <div class="section-title">
          Emprunts de la session
          <span class="count-chip">{{ sessionLoans().length }}</span>
        </div>
        <button *ngIf="sessionLoans().length > 0" class="btn btn-ghost btn-sm" (click)="clearHistory()">
          Vider
        </button>
      </div>

      <div *ngIf="sessionLoans().length === 0" class="empty-state">
        <div class="icon">○</div>
        <p>Aucun emprunt dans cette session.<br/>Utilisez le Catalogue pour emprunter.</p>
      </div>

      <div *ngIf="sessionLoans().length > 0" class="loans-list fade-in">
        <div *ngFor="let loan of sessionLoans()" class="loan-row" [class.returned]="loan.status === 'returned'">
          <div class="loan-avatar">
            <svg width="14" height="14" viewBox="0 0 14 14" fill="none">
              <rect x="1.5" y="2" width="8" height="10" rx="1" stroke="currentColor" stroke-width="1.2"/>
              <path d="M4.5 5h5M4.5 7.5h3" stroke="currentColor" stroke-width="1" stroke-linecap="round"/>
            </svg>
          </div>
          <div class="loan-info">
            <div class="loan-title">{{ loan.bookTitle }}</div>
            <div class="loan-meta">
              <span class="mono" style="font-size:10px;color:var(--text-dim)">{{ loan.loanId.slice(0,16) }}…</span>
              <span class="sep">·</span>
              <span style="font-size:11px;color:var(--text-muted)">{{ formatDate(loan.borrowedAt) }}</span>
            </div>
          </div>
          <div class="loan-actions">
            <ng-container [ngSwitch]="loan.status">
              <span *ngSwitchCase="'active'"></span>
              <span *ngSwitchCase="'returned'" class="badge" style="background:var(--bg-3);color:var(--text-muted)">Retourné</span>
              <span *ngSwitchCase="'error'" class="badge badge-unavailable" [title]="loan.error">Erreur</span>
              <span *ngSwitchCase="'returning'" class="spinner" style="width:13px;height:13px;border-width:1.5px;"></span>
            </ng-container>
            <!-- Bouton visible uniquement si statut 'active', jamais pendant 'returning' -->
            <button
              *ngIf="loan.status === 'active'"
              class="btn btn-ghost btn-sm"
              (click)="returnLoan(loan)"
            >
              ↩ Retourner
            </button>
          </div>
        </div>
      </div>

      <div *ngIf="sessionLoans().length === 0" class="hint-row">
        <div class="hint-step"><span class="step-num">1</span> Aller dans <strong>Catalogue</strong></div>
        <span class="hint-arrow">→</span>
        <div class="hint-step"><span class="step-num">2</span> Cliquer <strong>Emprunter</strong></div>
        <span class="hint-arrow">→</span>
        <div class="hint-step"><span class="step-num">3</span> Revenir ici pour retourner</div>
      </div>
    </div>
  `,
  styles: [`
    .page { padding: 32px; max-width: 760px; }

    .page-header {
      margin-bottom: 24px;
      h1 { font-size: 26px; margin-bottom: 3px; }
      .subtitle { font-size: 13px; color: var(--text-muted); }
    }

    .return-card { margin-bottom: 32px; }

    .card-label {
      font-size: 11px;
      text-transform: uppercase;
      letter-spacing: 0.07em;
      color: var(--text-muted);
      font-family: 'JetBrains Mono', monospace;
      margin-bottom: 12px;
    }

    .form-row {
      display: flex;
      gap: 10px;
      input { flex: 1; }
    }

    .mt-8 { margin-top: 8px; }

    .section-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 12px;
    }

    .section-title {
      font-size: 13px;
      font-weight: 600;
      color: var(--text-muted);
      text-transform: uppercase;
      letter-spacing: 0.06em;
      font-family: 'JetBrains Mono', monospace;
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .count-chip {
      background: var(--bg-4);
      color: var(--text-muted);
      padding: 1px 7px;
      border-radius: 10px;
      font-size: 11px;
    }

    .loans-list { display: flex; flex-direction: column; gap: 6px; }

    .loan-row {
      display: flex;
      align-items: center;
      gap: 12px;
      background: var(--bg-2);
      border: 1px solid var(--border);
      border-radius: var(--radius);
      padding: 12px 16px;
      transition: border-color 0.15s;
      box-shadow: 0 1px 2px rgba(0,0,0,0.03);

      &:hover { border-color: var(--border-strong); }
      &.returned { opacity: 0.55; }
    }

    .loan-avatar {
      width: 32px; height: 32px;
      background: var(--accent-dim);
      border-radius: var(--radius);
      display: flex;
      align-items: center;
      justify-content: center;
      color: var(--accent);
      flex-shrink: 0;
    }

    .loan-info { flex: 1; min-width: 0; }

    .loan-title {
      font-size: 14px;
      font-weight: 600;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .loan-meta {
      display: flex;
      align-items: center;
      gap: 6px;
      margin-top: 2px;
    }

    .sep { color: var(--border-strong); }

    .loan-actions {
      display: flex;
      align-items: center;
      gap: 8px;
      flex-shrink: 0;
    }

    .hint-row {
      display: flex;
      align-items: center;
      gap: 12px;
      background: var(--bg-2);
      border: 1px dashed var(--border-strong);
      border-radius: var(--radius-lg);
      padding: 20px 24px;
      margin-top: 24px;
      flex-wrap: wrap;
    }

    .hint-step {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 13px;
      color: var(--text-muted);
    }

    .step-num {
      width: 20px; height: 20px;
      border-radius: 50%;
      background: var(--accent-dim);
      color: var(--accent);
      font-size: 11px;
      font-weight: 700;
      display: flex;
      align-items: center;
      justify-content: center;
      font-family: 'JetBrains Mono', monospace;
      flex-shrink: 0;
    }

    .hint-arrow { color: var(--text-dim); }
  `]
})
export class LoansComponent implements OnInit {
  private bookService = inject(BookService);

  sessionLoans = signal<LoanEntry[]>([]);
  manualLoanId = '';
  returning = signal(false);
  manualResult = signal<{ ok: boolean; message: string } | null>(null);

  ngOnInit() { this.loadSession(); }

  private loadSession() {
    try {
      const raw = sessionStorage.getItem('library-loans');
      if (raw) {
        const list = JSON.parse(raw) as LoanEntry[];
        list.forEach(l => l.borrowedAt = new Date(l.borrowedAt));
        this.sessionLoans.set(list);
      }
    } catch { /* ignore */ }
  }

  private saveSession() {
    sessionStorage.setItem('library-loans', JSON.stringify(this.sessionLoans()));
  }

  private updateLoanStatus(loanId: string, status: LoanEntry['status'], error?: string) {
    this.sessionLoans.update(list =>
      list.map(l => l.loanId === loanId ? { ...l, status, error } : l)
    );
    this.saveSession();
  }

  returnLoan(loan: LoanEntry) {
    // 1. Passer en 'returning' immédiatement → masque le bouton, affiche le spinner
    this.updateLoanStatus(loan.loanId, 'returning');

    this.bookService.returnBook(loan.loanId).pipe(
      // 2. finalize() s'exécute TOUJOURS (succès ET erreur)
      //    → garantit qu'on ne reste jamais bloqué sur 'returning'
      //    → inutile ici car on gère explicitement les deux cas,
      //       mais on l'ajoute comme filet de sécurité
      finalize(() => {
        // Si après tout le flux le statut est encore 'returning',
        // c'est qu'un cas non géré est passé — on repasse en 'error'
        this.sessionLoans.update(list =>
          list.map(l =>
            l.loanId === loan.loanId && l.status === 'returning'
              ? { ...l, status: 'error' as const, error: 'Réponse inattendue du serveur' }
              : l
          )
        );
        this.saveSession();
      }),
      catchError(err => {
        // 3. Erreur HTTP → statut 'error' avec le message métier
        const msg = err?.error?.detail || err?.error?.message || 'Erreur lors du retour';
        this.updateLoanStatus(loan.loanId, 'error', msg);
        // EMPTY complète l'observable sans émettre de valeur
        // → subscribe(next) n'est pas appelé, finalize() l'est
        return EMPTY;
      })
    ).subscribe({
      // 4. Succès (204 No Content → next reçoit undefined, ce qui est OK avec void)
      next: () => {
        this.updateLoanStatus(loan.loanId, 'returned');
      },
      // 5. Les erreurs sont déjà gérées par catchError ci-dessus
    });
  }

  returnManual() {
    const loanId = this.manualLoanId.trim();
    if (!loanId) return;
    this.returning.set(true);
    this.manualResult.set(null);

    this.bookService.returnBook(loanId).pipe(
      finalize(() => this.returning.set(false)),
      catchError(err => {
        const msg = err?.error?.detail || err?.error?.message || 'Prêt introuvable ou déjà retourné.';
        this.manualResult.set({ ok: false, message: msg });
        return EMPTY;
      })
    ).subscribe({
      next: () => {
        this.manualResult.set({ ok: true, message: `Prêt ${loanId.slice(0, 8)}… retourné avec succès.` });
        this.manualLoanId = '';
        // Mettre à jour dans la liste si le prêt est présent
        this.sessionLoans.update(list =>
          list.map(l => l.loanId === loanId ? { ...l, status: 'returned' as const } : l)
        );
        this.saveSession();
      }
    });
  }

  clearHistory() {
    this.sessionLoans.set([]);
    sessionStorage.removeItem('library-loans');
  }

  formatDate(d: Date): string {
    return new Intl.DateTimeFormat('fr-FR', {
      day: '2-digit', month: '2-digit',
      hour: '2-digit', minute: '2-digit'
    }).format(d);
  }
}
