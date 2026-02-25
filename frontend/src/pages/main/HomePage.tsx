import styles from "./HomePage.module.css";

const HomePage = () => {
  return (
    <div>
      <h1>Welcome to Warehouse Management System</h1>
      <p className={styles.subtitle}>
        Manage your products, warehouses, and stock inventory efficiently.
      </p>
      <div className={styles.cardsGrid}>
        <div className={styles.card}>
          <h3>📦 Products</h3>
          <p>View and manage your product catalog</p>
        </div>
        <div className={styles.card}>
          <h3>🏢 Warehouses</h3>
          <p>Manage warehouse locations</p>
        </div>
        <div className={styles.card}>
          <h3>📊 Stock</h3>
          <p>Monitor stock levels across warehouses</p>
        </div>
      </div>
    </div>
  );
};

export default HomePage;
