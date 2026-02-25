import styles from "./DashboardPage.module.css";

const DashboardPage = () => {
  return (
    <div>
      <h2>Admin Dashboard</h2>
      <p>Welcome to the admin dashboard. Use the sidebar to navigate.</p>

      <div className={styles.statsGrid}>
        <div className={styles.statCard}>
          <h3>📦 Products</h3>
          <p className={styles.statValue}>-</p>
          <p className={styles.statLabel}>Total products</p>
        </div>
        <div className={styles.statCard}>
          <h3>🏢 Warehouses</h3>
          <p className={styles.statValue}>-</p>
          <p className={styles.statLabel}>Total warehouses</p>
        </div>
        <div className={styles.statCard}>
          <h3>📊 Stock Items</h3>
          <p className={styles.statValue}>-</p>
          <p className={styles.statLabel}>Total stock records</p>
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;
