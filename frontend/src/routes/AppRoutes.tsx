import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { MainLayout } from "../layouts/MainLayout";
import { AdminLayout } from "../layouts/AdminLayout";
import { ProtectedRoute } from "./ProtectedRoute";
import { ROUTES } from "./routesPaths";

// Main pages
import HomePage from "../pages/main/HomePage.tsx";
import ProductsPage from "../pages/main/ProductsPage.tsx";
import WarehousesPage from "../pages/main/WarehousesPage.tsx";
import StockPage from "../pages/main/StockPage.tsx";

// Admin pages
import AdminProductsPage from "../pages/admin/AdminProductsPage.tsx";
import AdminWarehousesPage from "../pages/admin/AdminWarehousesPage.tsx";
import HistoryPage from "../pages/admin/HistoryPage.tsx";

// Auth page
import LoginPage from "../pages/LoginPage.tsx";

export const AppRoutes = () => {
  return (
    <BrowserRouter>
      <Routes>
        {/* Main Layout Routes */}
        <Route element={<MainLayout />}>
          <Route path={ROUTES.HOME} element={<HomePage />} />
          <Route path={ROUTES.PRODUCTS} element={<ProductsPage />} />
          <Route path={ROUTES.WAREHOUSES} element={<WarehousesPage />} />
          <Route path={ROUTES.STOCK} element={<StockPage />} />
        </Route>

        {/* Admin Layout Routes */}
        <Route
          element={
            <ProtectedRoute>
              <AdminLayout />
            </ProtectedRoute>
          }
        >
          <Route path={ROUTES.ADMIN.PRODUCTS} element={<AdminProductsPage />} />
          <Route
            path={ROUTES.ADMIN.WAREHOUSES}
            element={<AdminWarehousesPage />}
          />
          <Route path={ROUTES.ADMIN.HISTORY} element={<HistoryPage />} />
          <Route
            path="/admin/dashboard"
            element={<Navigate to={ROUTES.ADMIN.PRODUCTS} replace />}
          />
        </Route>

        {/* Login Route (No Layout) */}
        <Route path={ROUTES.LOGIN} element={<LoginPage />} />

        {/* Catch all - redirect to home */}
        <Route path="*" element={<Navigate to={ROUTES.HOME} replace />} />
      </Routes>
    </BrowserRouter>
  );
};
