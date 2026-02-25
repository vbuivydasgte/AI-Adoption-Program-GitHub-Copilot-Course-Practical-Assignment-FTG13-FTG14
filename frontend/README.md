# Warehouse Management Frontend

React + TypeScript + Vite frontend for the Warehouse Management System.

## Features

- JWT-based authentication with protected routes.
- Main and admin layouts.
- Products, warehouses, stock, and stock history views.
- Sortable tabular views through reusable table/sorting utilities.
- Shared data-fetching and validation utilities.
- CSS Modules-based page styling.

## Setup

### Prerequisites

- Node.js 20+
- npm 10+

### Install and Run

```powershell
cd frontend
npm install
npm run dev
```

Frontend runs on Vite default host/port (typically `http://localhost:5173`).

## Build

```powershell
cd frontend
npm run build
```

## Project Structure (high level)

- `src/pages` – route pages (main/admin/auth)
- `src/components` – reusable UI components (`DataTable`)
- `src/hooks` – reusable hooks (`useSortableData`, `useDataFetching`)
- `src/api` – HTTP service layer and CRUD service factory
- `src/context` – auth context
- `src/routes` – route constants and router configuration
- `src/utils` – validation and error handling helpers

## Backend Dependency

The frontend expects the backend API to be running and reachable with the configured base URL from the axios config in `src/api/axiosConfig.ts`.

## Refactoring Highlights

- Extracted generic hooks for sorting and data fetching.
- Introduced reusable `DataTable` component and removed duplicated table markup.
- Added API CRUD service factory for product/warehouse services.
- Migrated page inline styles to CSS modules.
- Standardized error state and validation usage across pages.
