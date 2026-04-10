import { productService } from "../../api/productService";
import { warehouseService } from "../../api/warehouseService";
import { stockService } from "../../api/stockService";
import { useDataFetching } from "../../hooks/useDataFetching";
import styles from "./DashboardPage.module.css";

const DashboardPage = () => {
  const { data: products, loading: loadingProducts } = useDataFetching(
    () => productService.getAll(),
    [],
  );
  const { data: warehouses, loading: loadingWarehouses } = useDataFetching(
    () => warehouseService.getAll(),
    [],
  );
  const { data: stock, loading: loadingStock } = useDataFetching(
    () => stockService.getAll(),
    [],
  );

  return (
    <div>
      <h2>Admin Dashboard</h2>
      <p>Welcome to the admin dashboard. Use the sidebar to navigate.</p>

      <div className={styles.statsGrid}>
        <div className={styles.statCard}>
          <h3>📦 Products</h3>
          <p className={styles.statValue}>
            {loadingProducts ? "..." : products.length}
          </p>
          <p className={styles.statLabel}>Total products</p>
        </div>
        <div className={styles.statCard}>
          <h3>🏢 Warehouses</h3>
          <p className={styles.statValue}>
            {loadingWarehouses ? "..." : warehouses.length}
          </p>
          <p className={styles.statLabel}>Total warehouses</p>
        </div>
        <div className={styles.statCard}>
          <h3>📊 Stock Items</h3>
          <p className={styles.statValue}>
            {loadingStock ? "..." : stock.length}
          </p>
          <p className={styles.statLabel}>Total stock records</p>
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;
