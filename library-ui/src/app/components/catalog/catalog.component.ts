import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../services/book.service';
import { catchError, finalize, of } from 'rxjs';

type ImportMode = 'classic' | 'fast';
type ImportStatus = 'idle' | 'loading' | 'success' | 'error';

interface ImportResult {
  mode: ImportMode;
  fileName: string;
  status: 'success' | 'error';
  message: string;
  durationMs: number;
  timestamp: Date;
}

@Component({
  selector: 'app-catalog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page">
      <div class="page-header">
        <div>
          <h1>Import CSV</h1>
          <p class="subtitle">Importer un catalogue de livres</p>
        </div>
      </div>

      <!-- Format spec -->
      <div class="card format-card">
        <div class="card-label">Format CSV attendu</div>
        <div class="code-block">
          <span style="color:var(--accent)">&lt;uuid&gt;</span>;<span style="color:var(--text-muted)">&lt;titre&gt;</span>;<span style="color:var(--warning)">&lt;catégorie&gt;</span><br/>
          <span style="color:var(--text-dim)">7f814d48-356a-4d24-8148-bc6b6a2254e4;1984;SciFi</span><br/>
          <span style="color:var(--text-dim)">f3c1b81e-4505-4ef5-9a84-0e31846461a2;Le Seigneur des Anneaux;Fantasy</span>
        </div>
        <div class="tags">
          <span class="tag">Séparateur ;</span>
          <span class="tag">UTF-8</span>
          <span class="tag">SciFi | Fantasy | Unknown</span>
        </div>
      </div>

      <!-- Mode selector -->
      <div class="mode-grid">
        <button class="mode-card" [class.active]="mode === 'classic'" (click)="mode = 'classic'">
          <div class="mode-icon-wrap">
            <svg width="20" height="20" viewBox="0 0 20 20" fill="none">
              <rect x="3" y="3" width="14" height="14" rx="2" stroke="currentColor" stroke-width="1.4"/>
              <path d="M7 10l2 2 4-4" stroke="currentColor" stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
          <div class="mode-text">
            <div class="mode-name">Classic</div>
            <div class="mode-desc">String.Split(';') — simple</div>
          </div>
          <div class="mode-check" *ngIf="mode === 'classic'">
            <svg width="12" height="12" viewBox="0 0 12 12" fill="none">
              <path d="M2 6l3 3 5-6" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
        </button>

        <button class="mode-card mode-fast" [class.active]="mode === 'fast'" (click)="mode = 'fast'">
          <div class="mode-icon-wrap fast">
            <svg width="20" height="20" viewBox="0 0 20 20" fill="none">
              <path d="M10 2L4 11h6l-2 7 8-10h-6l2-6z" stroke="currentColor" stroke-width="1.4" stroke-linejoin="round"/>
            </svg>
          </div>
          <div class="mode-text">
            <div class="mode-name">Fast <span class="span-badge">Span&lt;char&gt;</span></div>
            <div class="mode-desc">Zero-allocation — ReadOnlySpan</div>
          </div>
          <div class="mode-check" *ngIf="mode === 'fast'">
            <svg width="12" height="12" viewBox="0 0 12 12" fill="none">
              <path d="M2 6l3 3 5-6" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
        </button>
      </div>

      <!-- Drop zone -->
      <div
        class="drop-zone"
        [class.dragover]="isDragOver()"
        [class.has-file]="selectedFile() !== null"
        [class.loading]="status() === 'loading'"
        (dragover)="onDragOver($event)"
        (dragleave)="onDragLeave()"
        (drop)="onDrop($event)"
        (click)="fileInput.click()"
      >
        <input #fileInput type="file" accept=".csv" style="display:none" (change)="onFileSelected($event)" />

        <ng-container *ngIf="status() !== 'loading'">
          <div *ngIf="!selectedFile()" class="drop-inner">
            <div class="drop-icon">
              <svg width="26" height="26" viewBox="0 0 26 26" fill="none">
                <path d="M13 3v16M6 9l7-7 7 7" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/>
                <path d="M3 19v1a2 2 0 002 2h16a2 2 0 002-2v-1" stroke="currentColor" stroke-width="1.4" stroke-linecap="round"/>
              </svg>
            </div>
            <div class="drop-text">Glisser-déposer un fichier CSV</div>
            <div class="drop-sub">ou cliquer pour sélectionner</div>
          </div>

          <div *ngIf="selectedFile()" class="file-row">
            <div class="file-icon">
              <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
                <path d="M9 2H4a1 1 0 00-1 1v10a1 1 0 001 1h8a1 1 0 001-1V6l-4-4z" stroke="var(--accent)" stroke-width="1.2"/>
                <path d="M9 2v4h4" stroke="var(--accent)" stroke-width="1.2" stroke-linecap="round"/>
              </svg>
            </div>
            <div>
              <div class="file-name">{{ selectedFile()!.name }}</div>
              <div class="file-size">{{ formatSize(selectedFile()!.size) }}</div>
            </div>
            <button class="btn btn-ghost btn-sm" style="margin-left:auto" (click)="clearFile($event)">✕</button>
          </div>
        </ng-container>

        <div *ngIf="status() === 'loading'" class="drop-loading">
          <span class="spinner" style="width:20px;height:20px;border-width:2px;"></span>
          <span style="color:var(--text-muted);font-size:13px">Import en cours…</span>
        </div>
      </div>

      <!-- Actions -->
      <div class="import-bar">
        <button
          class="btn btn-primary"
          style="min-width:160px;justify-content:center"
          [disabled]="!selectedFile() || status() === 'loading'"
          (click)="runImport()"
        >
          <span *ngIf="status() !== 'loading'">
            {{ mode === 'fast' ? '⚡ Import Fast' : 'Import Classic' }}
          </span>
          <span *ngIf="status() === 'loading'" class="spinner" style="width:13px;height:13px;border-width:1.5px;"></span>
        </button>
        <span class="mode-hint">Mode : <strong>{{ mode === 'fast' ? 'Fast (Span&lt;char&gt;)' : 'Classic (Split)' }}</strong></span>
      </div>

      <!-- Result -->
      <div *ngIf="currentResult()" class="result-row fade-in"
        [class.result-ok]="currentResult()!.status === 'success'"
        [class.result-err]="currentResult()!.status === 'error'"
      >
        <svg *ngIf="currentResult()!.status === 'success'" width="15" height="15" viewBox="0 0 15 15" fill="none">
          <path d="M3 7.5l3.5 3.5 6-8" stroke="var(--success)" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
        <svg *ngIf="currentResult()!.status === 'error'" width="15" height="15" viewBox="0 0 15 15" fill="none">
          <circle cx="7.5" cy="7.5" r="5.5" stroke="var(--danger)" stroke-width="1.3"/>
          <path d="M7.5 5v3.5" stroke="var(--danger)" stroke-width="1.3" stroke-linecap="round"/>
          <circle cx="7.5" cy="11" r="0.7" fill="var(--danger)"/>
        </svg>
        <span style="flex:1;font-size:13px">{{ currentResult()!.message }}</span>
        <span class="mono" style="font-size:11px;color:var(--text-muted)">{{ currentResult()!.durationMs }}ms</span>
      </div>

      <!-- History -->
      <div *ngIf="history().length > 0">
        <div class="section-row">
          <div class="section-label">Historique de session</div>
          <button class="btn btn-ghost btn-sm" (click)="clearHistory()">Vider</button>
        </div>
        <div class="history-list">
          <div *ngFor="let e of history()" class="history-row">
            <span class="h-badge" [class.h-fast]="e.mode === 'fast'" [class.h-classic]="e.mode === 'classic'">
              {{ e.mode }}
            </span>
            <span class="h-file">{{ e.fileName }}</span>
            <span class="h-ok" *ngIf="e.status === 'success'">✓</span>
            <span class="h-err" *ngIf="e.status === 'error'">✗</span>
            <span class="mono h-ms">{{ e.durationMs }}ms</span>
            <span class="h-time">{{ formatTime(e.timestamp) }}</span>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page { padding: 32px; max-width: 720px; }

    .page-header {
      margin-bottom: 22px;
      h1 { font-size: 26px; margin-bottom: 3px; }
      .subtitle { font-size: 13px; color: var(--text-muted); }
    }

    .format-card { margin-bottom: 24px; background: var(--bg-2); }

    .card-label {
      font-size: 11px;
      text-transform: uppercase;
      letter-spacing: 0.07em;
      color: var(--text-muted);
      font-family: 'JetBrains Mono', monospace;
      margin-bottom: 10px;
    }

    .code-block {
      font-family: 'JetBrains Mono', monospace;
      font-size: 12px;
      line-height: 1.8;
      background: var(--bg-3);
      border: 1px solid var(--border);
      border-radius: var(--radius);
      padding: 10px 14px;
      margin-bottom: 10px;
    }

    .tags { display: flex; gap: 6px; flex-wrap: wrap; }

    .tag {
      background: var(--bg-3);
      border: 1px solid var(--border);
      border-radius: 4px;
      padding: 3px 8px;
      font-size: 11px;
      color: var(--text-muted);
      font-family: 'JetBrains Mono', monospace;
    }

    .mode-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 10px;
      margin-bottom: 20px;
    }

    .mode-card {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 14px 16px;
      background: var(--bg-2);
      border: 1.5px solid var(--border);
      border-radius: var(--radius-lg);
      cursor: pointer;
      transition: all 0.15s;
      text-align: left;
      box-shadow: 0 1px 2px rgba(0,0,0,0.04);

      &:hover { border-color: var(--border-strong); }
      &.active {
        border-color: var(--accent);
        background: var(--accent-light);
      }
    }

    .mode-icon-wrap {
      width: 36px; height: 36px;
      background: var(--bg-3);
      border-radius: var(--radius);
      display: flex;
      align-items: center;
      justify-content: center;
      color: var(--text-muted);
      flex-shrink: 0;
      &.fast { background: rgba(124,58,237,0.08); color: #7c3aed; }
    }

    .mode-text { flex: 1; }

    .mode-name {
      font-size: 14px;
      font-weight: 600;
      color: var(--text);
      margin-bottom: 2px;
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .mode-desc {
      font-size: 11px;
      color: var(--text-muted);
      font-family: 'JetBrains Mono', monospace;
    }

    .span-badge {
      font-size: 10px;
      background: rgba(124,58,237,0.1);
      color: #7c3aed;
      padding: 1px 6px;
      border-radius: 3px;
      font-family: 'JetBrains Mono', monospace;
      font-weight: 400;
    }

    .mode-check {
      width: 20px; height: 20px;
      border-radius: 50%;
      background: var(--accent);
      color: #fff;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }

    .drop-zone {
      border: 1.5px dashed var(--border-strong);
      border-radius: var(--radius-lg);
      min-height: 130px;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: all 0.2s;
      margin-bottom: 14px;
      background: var(--bg-2);

      &:hover, &.dragover {
        border-color: var(--accent);
        background: var(--accent-light);
      }

      &.has-file {
        border-style: solid;
        border-color: var(--accent);
        background: var(--accent-light);
        padding: 20px;
      }

      &.loading { cursor: default; pointer-events: none; opacity: 0.7; }
    }

    .drop-inner {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 8px;
      padding: 24px;
    }

    .drop-icon { color: var(--text-dim); }
    .drop-text { font-size: 14px; font-weight: 500; color: var(--text-muted); }
    .drop-sub { font-size: 12px; color: var(--text-dim); }

    .file-row {
      display: flex;
      align-items: center;
      gap: 10px;
      width: 100%;
    }

    .file-icon {
      width: 32px; height: 32px;
      background: var(--accent-dim);
      border-radius: var(--radius);
      display: flex; align-items: center; justify-content: center;
      flex-shrink: 0;
    }

    .file-name { font-size: 13px; font-weight: 600; color: var(--text); }
    .file-size { font-size: 11px; color: var(--text-muted); font-family: 'JetBrains Mono', monospace; }

    .drop-loading { display: flex; align-items: center; gap: 12px; }

    .import-bar {
      display: flex;
      align-items: center;
      gap: 16px;
      margin-bottom: 20px;
    }

    .mode-hint {
      font-size: 12px;
      color: var(--text-muted);
      strong { color: var(--text); font-weight: 600; }
    }

    .result-row {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 12px 16px;
      border-radius: var(--radius);
      margin-bottom: 24px;
      font-size: 13px;

      &.result-ok  { background: var(--success-dim); border: 1px solid rgba(2,122,72,0.15); }
      &.result-err { background: var(--danger-dim);  border: 1px solid rgba(217,45,32,0.15); }
    }

    .section-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 10px;
    }

    .section-label {
      font-size: 11px;
      text-transform: uppercase;
      letter-spacing: 0.07em;
      color: var(--text-muted);
      font-family: 'JetBrains Mono', monospace;
    }

    .history-list { display: flex; flex-direction: column; gap: 4px; }

    .history-row {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 8px 12px;
      background: var(--bg-2);
      border: 1px solid var(--border);
      border-radius: var(--radius);
      font-size: 12px;
    }

    .h-badge {
      padding: 2px 7px;
      border-radius: 3px;
      font-size: 10px;
      font-family: 'JetBrains Mono', monospace;
      font-weight: 600;
      flex-shrink: 0;
      text-transform: uppercase;
      &.h-fast    { background: rgba(124,58,237,0.1); color: #7c3aed; }
      &.h-classic { background: var(--bg-3); color: var(--text-muted); border: 1px solid var(--border); }
    }

    .h-file { flex: 1; color: var(--text-muted); overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
    .h-ok   { color: var(--success); font-weight: 700; flex-shrink: 0; }
    .h-err  { color: var(--danger);  font-weight: 700; flex-shrink: 0; }
    .h-ms   { font-size: 11px; color: var(--text-dim); flex-shrink: 0; }
    .h-time { font-size: 11px; color: var(--text-dim); flex-shrink: 0; }
  `]
})
export class CatalogComponent {
  private bookService = inject(BookService);

  mode: ImportMode = 'fast';
  isDragOver = signal(false);
  selectedFile = signal<File | null>(null);
  status = signal<ImportStatus>('idle');
  currentResult = signal<ImportResult | null>(null);
  history = signal<ImportResult[]>([]);

  onDragOver(e: DragEvent) { e.preventDefault(); this.isDragOver.set(true); }
  onDragLeave() { this.isDragOver.set(false); }
  onDrop(e: DragEvent) {
    e.preventDefault();
    this.isDragOver.set(false);
    const f = e.dataTransfer?.files[0];
    if (f?.name.endsWith('.csv')) this.selectedFile.set(f);
  }
  onFileSelected(e: Event) {
    const f = (e.target as HTMLInputElement).files?.[0];
    if (f) this.selectedFile.set(f);
    (e.target as HTMLInputElement).value = '';
  }
  clearFile(e: Event) { e.stopPropagation(); this.selectedFile.set(null); this.status.set('idle'); }

  runImport() {
    const file = this.selectedFile();
    if (!file) return;
    const mode = this.mode;
    this.status.set('loading');
    this.currentResult.set(null);
    const t0 = performance.now();
    const obs = mode === 'fast' ? this.bookService.importFast(file) : this.bookService.importClassic(file);

    obs.pipe(
      finalize(() => this.status.set('idle')),
      catchError(err => {
        const msg = err?.error?.detail || err?.error?.message || 'Erreur lors de l\'import';
        const r: ImportResult = { mode, fileName: file.name, status: 'error', message: msg, durationMs: Math.round(performance.now() - t0), timestamp: new Date() };
        this.currentResult.set(r);
        this.history.update(h => [r, ...h].slice(0, 20));
        return of(null);
      })
    ).subscribe(res => {
      if (res !== null) {
        const r: ImportResult = { mode, fileName: file.name, status: 'success', message: res.message || 'Import terminé', durationMs: Math.round(performance.now() - t0), timestamp: new Date() };
        this.currentResult.set(r);
        this.history.update(h => [r, ...h].slice(0, 20));
        this.selectedFile.set(null);
      }
    });
  }

  clearHistory() { this.history.set([]); this.currentResult.set(null); }
  formatSize(b: number): string {
    if (b < 1024) return `${b} B`;
    if (b < 1024 * 1024) return `${(b / 1024).toFixed(1)} KB`;
    return `${(b / (1024 * 1024)).toFixed(1)} MB`;
  }
  formatTime(d: Date): string {
    return new Intl.DateTimeFormat('fr-FR', { hour: '2-digit', minute: '2-digit', second: '2-digit' }).format(d);
  }
}
