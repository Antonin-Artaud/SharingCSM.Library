import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'books',
    pathMatch: 'full'
  },
  {
    path: 'books',
    loadComponent: () => import('./components/books/books.component').then(m => m.BooksComponent)
  },
  {
    path: 'catalog',
    loadComponent: () => import('./components/catalog/catalog.component').then(m => m.CatalogComponent)
  },
  {
    path: 'loans',
    loadComponent: () => import('./components/loans/loans.component').then(m => m.LoansComponent)
  }
];
