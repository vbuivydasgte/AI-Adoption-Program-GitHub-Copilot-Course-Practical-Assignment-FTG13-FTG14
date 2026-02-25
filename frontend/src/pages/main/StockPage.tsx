import { useState } from "react";
import { stockService } from "../../api/stockService";
import { productService } from "../../api/productService";
import { warehouseService } from "../../api/warehouseService";
import type { Stock } from "../../types/stock.types";
import type { Product } from "../../types/product.types";
import type { Warehouse } from "../../types/warehouse.types";
import { useAuth } from "../../context/useAuth";
import { getErrorMessage } from "../../utils/errorHandler";
import { validateStockAdjustment } from "../../utils/validation";
import { useSortableData } from "../../hooks/useSortableData";
import { useDataFetching } from "../../hooks/useDataFetching";
import { DataTable } from "../../components/DataTable";
import type { DataTableColumn } from "../../components/DataTable";
import styles from "../PageStyles.module.css";

const StockPage = () => {
  const {
    data: stocks,
    loading,
    error,
    refetch: loadStocks,
    setError,
  } = useDataFetching<Stock[]>(() => stockService.getAll(), [], {
    errorMessage: "Failed to load stock",
  });
  const { data: products } = useDataFetching<Product[]>(
    () => productService.getAll(),
    [],
    { errorMessage: "Failed to load products" },
  );
  const { data: warehouses } = useDataFetching<Warehouse[]>(
    () => warehouseService.getAll(),
    [],
    { errorMessage: "Failed to load warehouses" },
  );
  const [productId, setProductId] = useState("");
  const [warehouseId, setWarehouseId] = useState("");
  const [quantityChange, setQuantityChange] = useState("");
  const [reason, setReason] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const { user } = useAuth();

  const handleAdjustStock = async (
    e: React.SyntheticEvent<HTMLFormElement>,
  ) => {
    e.preventDefault();

    const currentStock = stocks.find(
      (s) =>
        s.productId === Number(productId) &&
        s.warehouseId === Number(warehouseId),
    );

    const validation = validateStockAdjustment({
      productId: Number(productId),
      warehouseId: Number(warehouseId),
      quantityChange: Number(quantityChange),
      reason,
      currentStock: currentStock?.quantity,
    });

    if (!validation.isValid) {
      const errorMessages = Object.values(validation.errors).join(", ");
      setError(errorMessages);
      return;
    }

    try {
      setSubmitting(true);
      setError("");

      await stockService.adjust({
        productId: Number(productId),
        warehouseId: Number(warehouseId),
        quantityChange: Number(quantityChange),
        reason,
        changedBy: user?.username || "System",
      });

      setProductId("");
      setWarehouseId("");
      setQuantityChange("");
      setReason("");
      await loadStocks();
    } catch (err) {
      setError(getErrorMessage(err, "Failed to adjust stock"));
      console.error(err);
    } finally {
      setSubmitting(false);
    }
  };

  const {
    sortedData: sortedStocks,
    handleSort,
    sortIndicator,
  } = useSortableData<Stock>(stocks, {
    defaultSortField: "productName" as keyof Stock,
    defaultSortDirection: "asc",
  });

  const columns: DataTableColumn<Stock>[] = [
    { header: "Product", field: "productName" },
    { header: "SKU", field: "productSKU" },
    { header: "Warehouse", field: "warehouseName" },
    { header: "Quantity", field: "quantity", align: "right" },
  ];

  if (loading) return <div>Loading stock...</div>;

  return (
    <div>
      <h1>Stock Levels</h1>
      <p>Total Stock Records: {stocks.length}</p>

      {error && <div className={styles.errorAlert}>{error}</div>}

      {user && (
        <form
          onSubmit={handleAdjustStock}
          className={`${styles.formCard} ${styles.formGrid}`}
        >
          <h3 className={styles.formTitle}>Adjust Stock</h3>
          <select
            value={productId}
            onChange={(e) => setProductId(e.target.value)}
            required
            className={styles.field}
          >
            <option value="">Select Product</option>
            {products.map((product) => (
              <option key={product.id} value={String(product.id)}>
                {product.name} ({product.sku})
              </option>
            ))}
          </select>
          <select
            value={warehouseId}
            onChange={(e) => setWarehouseId(e.target.value)}
            required
            className={styles.field}
          >
            <option value="">Select Warehouse</option>
            {warehouses.map((warehouse) => (
              <option key={warehouse.id} value={String(warehouse.id)}>
                {warehouse.name} ({warehouse.location})
              </option>
            ))}
          </select>
          <input
            type="number"
            placeholder="Quantity change (e.g. 10 or -5)"
            value={quantityChange}
            onChange={(e) => setQuantityChange(e.target.value)}
            required
            className={styles.field}
          />
          <input
            type="text"
            placeholder="Reason"
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            required
            className={styles.field}
          />
          <button
            type="submit"
            disabled={submitting}
            className={`${styles.button} ${styles.buttonStandard} ${styles.buttonPrimary} ${submitting ? styles.buttonDisabled : ""}`}
          >
            {submitting ? "Saving..." : "Apply Adjustment"}
          </button>
        </form>
      )}

      <div className={styles.section}>
        <h3>Stock Records</h3>
        <DataTable
          data={sortedStocks}
          columns={columns}
          keyField="id"
          onSort={handleSort}
          sortIndicator={sortIndicator}
          emptyMessage="No stock records found."
        />
      </div>
    </div>
  );
};

export default StockPage;
