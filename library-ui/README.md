# SharingCSM Library — Frontend Angular

Application Angular 19 pour la démo du projet SharingCSM.Library.

## Stack technique

- Angular 19 (standalone components, signals, inject())
- TypeScript strict
- SCSS avec design system sombre custom
- Nginx en production (Docker)
- Intégration .NET Aspire

---

## Structure

```
library-ui/
├── src/
│   ├── app/
│   │   ├── components/
│   │   │   ├── books/          ← Catalogue + recherche + emprunt
│   │   │   ├── loans/          ← Retour de prêts
│   │   │   └── catalog/        ← Import CSV (classic vs Span)
│   │   ├── models/             ← Interfaces TypeScript
│   │   ├── services/           ← BookService (HTTP)
│   │   ├── app.component.ts    ← Layout + sidebar
│   │   └── app.routes.ts       ← Routing lazy-loaded
│   ├── environments/           ← Dev / Prod
│   └── styles.scss             ← Design system global
├── Dockerfile                  ← Build multi-stage + nginx
├── nginx.conf                  ← Proxy /api → .NET API
├── proxy.conf.json             ← Proxy dev server Angular
└── angular.json
```

---

## Endpoints consommés

| Feature | Méthode | URL |
|---------|---------|-----|
| Recherche livres | GET | `/api/books?searchTerm=&category=&onlyAvailable=&page=&pageSize=` |
| Emprunter | POST | `/api/loans` `{ bookId, userId }` |
| Retourner | POST | `/api/loans/{loanId}/return` |
| Import classic | POST | `/api/catalog/import/classic` (multipart) |
| Import fast | POST | `/api/catalog/import/fast` (multipart) |

---

## Intégration Aspire

### 1. Copier le dossier `library-ui/` à la racine de la solution

```
SharingCSM.Library/
├── src/
├── tests/
├── library-ui/          ← ici
└── SharingCsm.Library.slnx
```

### 2. Remplacer `AppHost.cs` et `LibraryResourceNames.cs`

Copier les fichiers fournis dans `src/host/SharingCsm.Library.AppHost/`.

### 3. Ajouter la dépendance Aspire npm dans le `.csproj` de l'AppHost

```xml
<ItemGroup>
  <PackageReference Include="Aspire.Hosting.NodeJs" Version="9.*" />
</ItemGroup>
```

### 4. Lancer

```bash
dotnet run --project src/host/SharingCsm.Library.AppHost
```

Aspire démarre automatiquement `npm start` dans `library-ui/`.
L'interface est accessible depuis le dashboard Aspire.

---

## Développement standalone

```bash
cd library-ui
npm install
npm start
# → http://localhost:4200
# → proxy /api → http://localhost:5000 (via proxy.conf.json)
```

---

## Build Docker manuel

```bash
cd library-ui
docker build -t library-ui .
docker run -p 8080:80 \
  -e services__library__api__http__0=http://host.docker.internal:5000 \
  library-ui
```

---

## Features

### Catalogue (`/books`)
- Recherche par titre avec debounce 300ms
- Filtre par catégorie (SciFi / Fantasy / Toutes)
- Toggle disponibles seulement
- Pagination
- Emprunt en 1 clic — retourne le LoanId
- Identité utilisateur aléatoire par session (changeable)

### Emprunts (`/loans`)
- Historique des prêts de la session (sessionStorage)
- Retour en 1 clic par prêt de la session
- Retour manuel par LoanId (pour tester n'importe quel GUID)

### Import CSV (`/catalog`)
- Drag & drop ou sélection de fichier
- Choix entre mode **Classic** (String.Split) et **Fast** (Span&lt;char&gt;)
- Affichage du temps d'exécution mesuré côté client
- Historique des imports de la session
- Rappel du format CSV attendu
