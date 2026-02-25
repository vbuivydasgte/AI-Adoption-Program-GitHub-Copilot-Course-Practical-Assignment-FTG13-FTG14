import { Link, Outlet } from "react-router-dom";
import { useAuth } from "../context/useAuth";
import { ROUTES } from "../routes/routesPaths";

export const MainLayout = () => {
  const { user, logout, isAdmin } = useAuth();

  return (
    <div
      style={{ minHeight: "100vh", display: "flex", flexDirection: "column" }}
    >
      <header
        style={{
          background: "#2c3e50",
          color: "white",
          padding: "1rem",
          boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
        }}
      >
        <nav className="main-nav content-container">
          <div className="main-nav-left">
            <h1 style={{ margin: 0 }}>Warehouse Management</h1>
            <div className="main-nav-links">
              <Link
                to={ROUTES.HOME}
                style={{ color: "white", textDecoration: "none" }}
              >
                Home
              </Link>
              {user && (
                <>
                  <Link
                    to={ROUTES.PRODUCTS}
                    style={{ color: "white", textDecoration: "none" }}
                  >
                    Products
                  </Link>
                  <Link
                    to={ROUTES.WAREHOUSES}
                    style={{ color: "white", textDecoration: "none" }}
                  >
                    Warehouses
                  </Link>
                  <Link
                    to={ROUTES.STOCK}
                    style={{ color: "white", textDecoration: "none" }}
                  >
                    Stock
                  </Link>
                </>
              )}
              {user && isAdmin && (
                <Link
                  to={ROUTES.ADMIN.PRODUCTS}
                  style={{
                    color: "#f39c12",
                    textDecoration: "none",
                    fontWeight: "bold",
                  }}
                >
                  Admin
                </Link>
              )}
            </div>
          </div>
          <div className="main-nav-right">
            {user ? (
              <>
                <span>Welcome, {user.username}</span>
                <button
                  onClick={logout}
                  style={{
                    padding: "0.5rem 1rem",
                    background: "#e74c3c",
                    color: "white",
                    border: "none",
                    borderRadius: "4px",
                    cursor: "pointer",
                  }}
                >
                  Logout
                </button>
              </>
            ) : (
              <Link
                to={ROUTES.LOGIN}
                style={{
                  padding: "0.5rem 1rem",
                  background: "#3498db",
                  color: "white",
                  textDecoration: "none",
                  borderRadius: "4px",
                }}
              >
                Login
              </Link>
            )}
          </div>
        </nav>
      </header>
      <main
        className="content-container"
        style={{ flex: 1, padding: "1rem 0" }}
      >
        <Outlet />
      </main>
      <footer
        style={{
          background: "#34495e",
          color: "white",
          textAlign: "center",
          padding: "1rem",
        }}
      >
        <p>© 2026 Warehouse Management System</p>
      </footer>
    </div>
  );
};
