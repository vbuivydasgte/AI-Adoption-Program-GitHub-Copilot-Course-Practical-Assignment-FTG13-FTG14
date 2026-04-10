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
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [deleting, setDeleting] = useState<number | null>(null);
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

  const handleEditClick = (product: Product) => {
    setEditingProduct(product);
    setName(product.name);
    setSku(product.sku);
    setDescription(product.description || "");
    setShowForm(false);
    setError("");
  };

  const handleUpdate = async (e: React.SyntheticEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!editingProduct) return;

    const validation = validateProduct({ name, sku, description });
    if (!validation.isValid) {
      const errorMessages = Object.values(validation.errors).join(", ");
      setError(errorMessages);
      return;
    }

    try {
      setSubmitting(true);
      setError("");
      await productService.update(editingProduct.id, {
        name,
        sku,
        description: description || undefined,
      });
      setEditingProduct(null);
      setName("");
      setSku("");
      setDescription("");
      await loadProducts();
    } catch (err) {
      setError(getErrorMessage(err, "Failed to update product"));
      console.error(err);
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (product: Product) => {
    if (
      !globalThis.confirm(
        `Delete product "${product.name}"? This action cannot be undone.`,
      )
    )
      return;

    try {
      setDeleting(product.id);
      setError("");
      await productService.delete(product.id);
      await loadProducts();
    } catch (err) {
      setError(getErrorMessage(err, "Failed to delete product"));
      console.error(err);
    } finally {
      setDeleting(null);
    }
  };

  const cancelEdit = () => {
    setEditingProduct(null);
    setName("");
    setSku("");
    setDescription("");
    setError("");
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
    {
      header: "Actions",
      field: "id",
      sortable: false,
      render: (_, row) => (
        <div style={{ display: "flex", gap: "0.5rem" }}>
          <button
            onClick={() => handleEditClick(row)}
            className={`${styles.button} ${styles.buttonStandard} ${styles.buttonPrimary}`}
          >
            Edit
          </button>
          <button
            onClick={() => handleDelete(row)}
            disabled={deleting === row.id}
            className={`${styles.button} ${styles.buttonStandard} ${styles.buttonDanger} ${deleting === row.id ? styles.buttonDisabled : ""}`}
          >
            {deleting === row.id ? "Deleting..." : "Delete"}
          </button>
        </div>
      ),
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

      {!editingProduct && (
        <button
          onClick={() => setShowForm((prev) => !prev)}
          className={`${styles.button} ${styles.buttonWide} ${styles.buttonSuccess} ${styles.buttonTop}`}
        >
          {showForm ? "Cancel" : "+ Add New Product"}
        </button>
      )}

      {showForm && !editingProduct && (
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

      {editingProduct && (
        <form
          onSubmit={handleUpdate}
          className={`${styles.section} ${styles.formGrid}`}
        >
          <h3 className={styles.formTitle}>Editing: {editingProduct.name}</h3>
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
          <div style={{ display: "flex", gap: "0.75rem" }}>
            <button
              type="submit"
              disabled={submitting}
              className={`${styles.button} ${styles.buttonWide} ${styles.buttonPrimary} ${submitting ? styles.buttonDisabled : ""}`}
            >
              {submitting ? "Saving..." : "Update Product"}
            </button>
            <button
              type="button"
              onClick={cancelEdit}
              className={`${styles.button} ${styles.buttonWide} ${styles.buttonDanger}`}
            >
              Cancel
            </button>
          </div>
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
