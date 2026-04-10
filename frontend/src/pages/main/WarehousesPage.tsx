import { warehouseService } from "../../api/warehouseService";
import type { Warehouse } from "../../types/warehouse.types";
import { useSortableData } from "../../hooks/useSortableData";
import { useDataFetching } from "../../hooks/useDataFetching";
import { useSearch } from "../../hooks/useSearch";
import { DataTable } from "../../components/DataTable";
import type { DataTableColumn } from "../../components/DataTable";
import styles from "../PageStyles.module.css";

const WAREHOUSE_SEARCH_FIELDS: (keyof Warehouse)[] = ["name", "location"];

const WarehousesPage = () => {
  const {
    data: warehouses,
    loading,
    error,
  } = useDataFetching<Warehouse[]>(() => warehouseService.getAll(), [], {
    errorMessage: "Failed to load warehouses",
  });

  const {
    searchQuery,
    setSearchQuery,
    filteredData: filteredWarehouses,
  } = useSearch<Warehouse>(warehouses, WAREHOUSE_SEARCH_FIELDS);

  const {
    sortedData: sortedWarehouses,
    handleSort,
    sortIndicator,
  } = useSortableData<Warehouse>(filteredWarehouses, {
    defaultSortField: "name" as keyof Warehouse,
    defaultSortDirection: "asc",
  });

  const columns: DataTableColumn<Warehouse>[] = [
    { header: "Name", field: "name" },
    { header: "Location", field: "location" },
  ];

  if (loading) return <div>Loading warehouses...</div>;
  if (error) return <div className={styles.error}>{error}</div>;

  return (
    <div>
      <h1>Warehouses</h1>
      <p>Total Warehouses: {warehouses.length}</p>

      <input
        type="search"
        value={searchQuery}
        onChange={(e) => setSearchQuery(e.target.value)}
        placeholder="Search by name or location..."
        className={styles.searchBar}
      />

      <DataTable
        data={sortedWarehouses}
        columns={columns}
        keyField="id"
        onSort={handleSort}
        sortIndicator={sortIndicator}
        emptyMessage="No warehouses match your search."
      />
    </div>
  );
};

export default WarehousesPage;
