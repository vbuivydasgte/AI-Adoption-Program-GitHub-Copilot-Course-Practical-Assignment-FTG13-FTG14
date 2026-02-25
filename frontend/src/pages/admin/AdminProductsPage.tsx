import { useState } from "react";
import { productService } from "../../api/productService";
import type { Product } from "../../types/product.types";
import { getErrorMessage } from "../../utils/errorHandler";
import { validateProduct } from "../../utils/validation";
import { useSortableData } from "../../hooks/useSortableData";
import { useDataFetching } from "../../hooks/useDataFetching";
import { DataTable } from "../../components/DataTable";
import type { DataTableColumn } from "../../components/DataTable";
import styles from "../PageStyles.module.css";

const AdminProductsPage = () => {
  const {
    data: products,
    loading,
    error,
    refetch: loadProducts,
    setError,
  } = useDataFetching<Product[]>(() => productService.getAll(), [], {
    errorMessage: "Failed to load products",
  });
  const [showForm, setShowForm] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [name, setName] = useState("");
  const [sku, setSku] = useState("");
  const [description, setDescription] = useState("");

  const handleCreate = async (e: React.SyntheticEvent<HTMLFormElement>) => {
    e.preventDefault();

    const validation = validateProduct({ name, sku, description });
    if (!validation.isValid) {
      const errorMessages = Object.values(validation.errors).join(", ");
      setError(errorMessages);
      return;
    }

    try {
      setSubmitting(true);
      setError("");
      await productService.create({
        name,
        sku,
        description: description || undefined,
      });
      setName("");
      setSku("");
      setDescription("");
      setShowForm(false);
      await loadProducts();
    } catch (err) {
      setError(getErrorMessage(err, "Failed to create product"));
      console.error(err);
    } finally {
      setSubmitting(false);
    }
  };

  const {
    sortedData: sortedProducts,
    handleSort,
    sortIndicator,
  } = useSortableData<Product>(products, {
    defaultSortField: "name" as keyof Product,
    defaultSortDirection: "asc",
  });

  const columns: DataTableColumn<Product>[] = [
    { header: "SKU", field: "sku" },
    { header: "Name", field: "name" },
    {
      header: "Description",
      field: "description",
      render: (value) => value || "-",
    },
  ];

  let productsContent: React.ReactNode = null;

  if (loading) {
    productsContent = <p>Loading products...</p>;
  } else {
    productsContent = (
      <DataTable
        data={sortedProducts}
        columns={columns}
        keyField="id"
        onSort={handleSort}
        sortIndicator={sortIndicator}
        emptyMessage="No products found."
        headerPadding="0.75rem"
        cellPadding="0.75rem"
      />
    );
  }

  return (
    <div>
      <h2>Manage Products</h2>
      <p>Admin interface for managing products with full CRUD operations.</p>

      {error && <p className={styles.errorAlert}>{error}</p>}

      <button
        onClick={() => setShowForm((prev) => !prev)}
        className={`${styles.button} ${styles.buttonWide} ${styles.buttonSuccess} ${styles.buttonTop}`}
      >
        {showForm ? "Cancel" : "+ Add New Product"}
      </button>

      {showForm && (
        <form
          onSubmit={handleCreate}
          className={`${styles.section} ${styles.formGrid}`}
        >
          <input
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Name"
            required
            className={styles.field}
          />
          <input
            value={sku}
            onChange={(e) => setSku(e.target.value)}
            placeholder="SKU"
            required
            className={styles.field}
          />
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Description"
            rows={3}
            className={styles.field}
          />
          <button
            type="submit"
            disabled={submitting}
            className={`${styles.button} ${styles.buttonWide} ${styles.buttonPrimary} ${submitting ? styles.buttonDisabled : ""}`}
          >
            {submitting ? "Saving..." : "Save Product"}
          </button>
        </form>
      )}

      <div className={styles.section}>
        <h3>Existing Products ({products.length})</h3>
        {productsContent}
      </div>
    </div>
  );
};

export default AdminProductsPage;
