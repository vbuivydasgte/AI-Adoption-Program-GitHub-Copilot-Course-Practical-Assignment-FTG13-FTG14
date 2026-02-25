import { useState } from "react";
import { warehouseService } from "../../api/warehouseService";
import type { Warehouse } from "../../types/warehouse.types";
import { getErrorMessage } from "../../utils/errorHandler";
import { validateWarehouse } from "../../utils/validation";
import { useSortableData } from "../../hooks/useSortableData";
import { useDataFetching } from "../../hooks/useDataFetching";
import { DataTable } from "../../components/DataTable";
import type { DataTableColumn } from "../../components/DataTable";
import styles from "../PageStyles.module.css";

const AdminWarehousesPage = () => {
  const {
    data: warehouses,
    loading,
    error,
    refetch: loadWarehouses,
    setError,
  } = useDataFetching<Warehouse[]>(() => warehouseService.getAll(), [], {
    errorMessage: "Failed to load warehouses",
  });
  const [showForm, setShowForm] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [name, setName] = useState("");
  const [location, setLocation] = useState("");

  const handleCreate = async (e: React.SyntheticEvent<HTMLFormElement>) => {
    e.preventDefault();

    const validation = validateWarehouse({ name, location });
    if (!validation.isValid) {
      const errorMessages = Object.values(validation.errors).join(", ");
      setError(errorMessages);
      return;
    }

    try {
      setSubmitting(true);
      setError("");
      await warehouseService.create({ name, location });
      setName("");
      setLocation("");
      setShowForm(false);
      await loadWarehouses();
    } catch (err) {
      setError(getErrorMessage(err, "Failed to create warehouse"));
      console.error(err);
    } finally {
      setSubmitting(false);
    }
  };

  const {
    sortedData: sortedWarehouses,
    handleSort,
    sortIndicator,
  } = useSortableData<Warehouse>(warehouses, {
    defaultSortField: "name" as keyof Warehouse,
    defaultSortDirection: "asc",
  });

  const columns: DataTableColumn<Warehouse>[] = [
    { header: "Name", field: "name" },
    { header: "Location", field: "location" },
  ];

  let warehousesContent: React.ReactNode = null;

  if (loading) {
    warehousesContent = <p>Loading warehouses...</p>;
  } else {
    warehousesContent = (
      <DataTable
        data={sortedWarehouses}
        columns={columns}
        keyField="id"
        onSort={handleSort}
        sortIndicator={sortIndicator}
        emptyMessage="No warehouses found."
        headerPadding="0.75rem"
        cellPadding="0.75rem"
      />
    );
  }

  return (
    <div>
      <h2>Manage Warehouses</h2>
      <p>Admin interface for managing warehouses with full CRUD operations.</p>

      {error && <p className={styles.errorAlert}>{error}</p>}

      <button
        onClick={() => setShowForm((prev) => !prev)}
        className={`${styles.button} ${styles.buttonWide} ${styles.buttonSuccess} ${styles.buttonTop}`}
      >
        {showForm ? "Cancel" : "+ Add New Warehouse"}
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
            value={location}
            onChange={(e) => setLocation(e.target.value)}
            placeholder="Location"
            required
            className={styles.field}
          />
          <button
            type="submit"
            disabled={submitting}
            className={`${styles.button} ${styles.buttonWide} ${styles.buttonPrimary} ${submitting ? styles.buttonDisabled : ""}`}
          >
            {submitting ? "Saving..." : "Save Warehouse"}
          </button>
        </form>
      )}

      <div className={styles.section}>
        <h3>Existing Warehouses ({warehouses.length})</h3>
        {warehousesContent}
      </div>
    </div>
  );
};

export default AdminWarehousesPage;
