import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="layout">
      <nav class="sidebar">
        <div class="logo">
          <div class="logo-mark">◈</div>
          <div>
            <div class="logo-name">Library</div>
            <div class="logo-sub">SharingCSM Demo</div>
          </div>
        </div>

        <div class="nav-section">
          <div class="nav-label">Navigation</div>

          <a routerLink="/books" routerLinkActive="active" class="nav-item">
            <svg width="15" height="15" viewBox="0 0 15 15" fill="none">
              <rect x="2" y="2" width="4" height="11" rx="1" stroke="currentColor" stroke-width="1.2"/>
              <rect x="7" y="2" width="6" height="7" rx="1" stroke="currentColor" stroke-width="1.2"/>
              <rect x="7" y="10" width="6" height="3" rx="1" stroke="currentColor" stroke-width="1.2"/>
            </svg>
            Catalogue
          </a>

          <a routerLink="/loans" routerLinkActive="active" class="nav-item">
            <svg width="15" height="15" viewBox="0 0 15 15" fill="none">
              <path d="M2 4h11M2 7.5h11M2 11h6" stroke="currentColor" stroke-width="1.2" stroke-linecap="round"/>
            </svg>
            Emprunts
          </a>

          <a routerLink="/catalog" routerLinkActive="active" class="nav-item">
            <svg width="15" height="15" viewBox="0 0 15 15" fill="none">
              <path d="M7.5 1v13M1 7.5h13" stroke="currentColor" stroke-width="1.4" stroke-linecap="round"/>
            </svg>
            Import CSV
          </a>
        </div>

        <div class="sidebar-footer">
          <div class="tech-stack">
            <span class="tech-chip">Angular 19</span>
            <span class="tech-chip">.NET 10</span>
            <span class="tech-chip">Aspire</span>
          </div>
        </div>
      </nav>

      <main class="content">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [`
    .layout {
      display: flex;
      height: 100vh;
      overflow: hidden;
    }

    .sidebar {
      width: 220px;
      flex-shrink: 0;
      background: var(--bg-2);
      border-right: 1px solid var(--border);
      display: flex;
      flex-direction: column;
      padding: 20px 0;
      box-shadow: 1px 0 4px rgba(0,0,0,0.03);
    }

    .logo {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 0 18px 22px;
      border-bottom: 1px solid var(--border);
      margin-bottom: 18px;
    }

    .logo-mark {
      width: 34px;
      height: 34px;
      background: var(--accent);
      border-radius: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #fff;
      font-size: 16px;
      flex-shrink: 0;
    }

    .logo-name {
      font-size: 15px;
      font-weight: 700;
      color: var(--text);
      letter-spacing: -0.02em;
    }

    .logo-sub {
      font-size: 10px;
      color: var(--text-muted);
      letter-spacing: 0.04em;
      text-transform: uppercase;
      font-family: 'JetBrains Mono', monospace;
    }

    .nav-section {
      padding: 0 10px;
      flex: 1;
    }

    .nav-label {
      font-size: 10px;
      letter-spacing: 0.08em;
      text-transform: uppercase;
      color: var(--text-dim);
      padding: 0 8px;
      margin-bottom: 4px;
      font-family: 'JetBrains Mono', monospace;
    }

    .nav-item {
      display: flex;
      align-items: center;
      gap: 9px;
      padding: 9px 10px;
      border-radius: var(--radius);
      color: var(--text-muted);
      text-decoration: none;
      font-size: 13px;
      font-weight: 500;
      transition: all 0.15s;
      margin-bottom: 2px;

      svg { flex-shrink: 0; }

      &:hover {
        background: var(--bg-3);
        color: var(--text);
      }

      &.active {
        background: var(--accent-dim);
        color: var(--accent);
        font-weight: 600;
      }
    }

    .sidebar-footer {
      padding: 14px 18px 0;
      border-top: 1px solid var(--border);
    }

    .tech-stack {
      display: flex;
      flex-wrap: wrap;
      gap: 4px;
    }

    .tech-chip {
      background: var(--bg-3);
      border: 1px solid var(--border);
      border-radius: 4px;
      padding: 2px 7px;
      font-family: 'JetBrains Mono', monospace;
      font-size: 10px;
      color: var(--text-muted);
    }

    .content {
      flex: 1;
      overflow-y: auto;
      overflow-x: hidden;
      background: var(--bg);
    }
  `]
})
export class AppComponent {}
