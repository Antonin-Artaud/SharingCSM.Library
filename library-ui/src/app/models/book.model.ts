export type BookCategory = 'Unknown' | 'SciFi' | 'Fantasy';

export interface Book {
  id: string;
  title: string;
  category: BookCategory;
  isAvailable: boolean;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface BookSearchRequest {
  searchTerm?: string;
  category?: BookCategory;
  onlyAvailable: boolean;
  page: number;
  pageSize: number;
}

export interface CreateLoanRequest {
  bookId: string;
  userId: string;
}

export interface CreateLoanResponse {
  id: string;
}
