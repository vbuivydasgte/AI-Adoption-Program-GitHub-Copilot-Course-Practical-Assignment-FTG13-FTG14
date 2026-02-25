import { Link, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../context/useAuth";
import { ROUTES } from "../routes/routesPaths";

export const AdminLayout = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate(ROUTES.LOGIN);
  };

  return (
    <div className="admin-shell">
      <aside
        className="admin-sidebar"
        style={{
          background: "#2c3e50",
          color: "white",
          padding: "1rem",
          boxShadow: "2px 0 4px rgba(0,0,0,0.1)",
        }}
      >
        <div
          style={{
            marginBottom: "2rem",
            padding: "1rem",
            borderBottom: "1px solid #34495e",
          }}
        >
          <h2 style={{ margin: "0 0 0.5rem 0" }}>Admin Panel</h2>
          <p style={{ margin: 0, fontSize: "0.9rem", opacity: 0.8 }}>
            {user?.username}
          </p>
        </div>
        <nav
          style={{ display: "flex", flexDirection: "column", gap: "0.5rem" }}
        >
          <Link
            to={ROUTES.ADMIN.PRODUCTS}
            style={{
              padding: "0.75rem 1rem",
              color: "white",
              textDecoration: "none",
              borderRadius: "4px",
              transition: "background 0.2s",
            }}
          >
            📦 Products
          </Link>
          <Link
            to={ROUTES.ADMIN.WAREHOUSES}
            style={{
              padding: "0.75rem 1rem",
              color: "white",
              textDecoration: "none",
              borderRadius: "4px",
              transition: "background 0.2s",
            }}
          >
            🏢 Warehouses
          </Link>
          <Link
            to={ROUTES.ADMIN.HISTORY}
            style={{
              padding: "0.75rem 1rem",
              color: "white",
              textDecoration: "none",
              borderRadius: "4px",
              transition: "background 0.2s",
            }}
          >
            📜 History
          </Link>
          <hr
            style={{
              margin: "1rem 0",
              border: "none",
              borderTop: "1px solid #34495e",
            }}
          />
          <Link
            to={ROUTES.HOME}
            style={{
              padding: "0.75rem 1rem",
              color: "#3498db",
              textDecoration: "none",
              borderRadius: "4px",
            }}
          >
            ← Back to Main Site
          </Link>
          <button
            onClick={handleLogout}
            style={{
              padding: "0.75rem 1rem",
              background: "#e74c3c",
              color: "white",
              border: "none",
              borderRadius: "4px",
              cursor: "pointer",
              textAlign: "left",
              marginTop: "1rem",
            }}
          >
            🚪 Logout
          </button>
        </nav>
      </aside>
      <div style={{ flex: 1, display: "flex", flexDirection: "column" }}>
        <header
          style={{
            background: "white",
            padding: "1rem",
            boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
          }}
        >
          <h1 style={{ margin: 0, color: "#111827" }}>Admin Panel</h1>
        </header>
        <main style={{ flex: 1, padding: "1rem", background: "#ecf0f1" }}>
          <div className="content-container">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  );
};
