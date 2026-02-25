import { historyService } from "../../api/historyService";
import type { StockHistory } from "../../types/history.types";
import { useSortableData } from "../../hooks/useSortableData";
import { useDataFetching } from "../../hooks/useDataFetching";
import { DataTable } from "../../components/DataTable";
import type { DataTableColumn } from "../../components/DataTable";
import styles from "../PageStyles.module.css";

type StockSortField =
  | "changedAt"
  | "changedBy"
  | "productName"
  | "warehouseName"
  | "previousQuantity"
  | "quantityChange"
  | "newQuantity"
  | "reason";

const HistoryPage = () => {
  const {
    data: stockHistory,
    loading,
    error,
  } = useDataFetching<StockHistory[]>(
    () => historyService.getAllStockHistory(),
    [],
    { errorMessage: "Failed to load history data" },
  );

  const {
    sortedData: sortedStockHistory,
    handleSort: handleStockSort,
    sortIndicator: stockSortIndicator,
  } = useSortableData<StockHistory>(stockHistory, {
    defaultSortField: "changedAt" as keyof StockHistory,
    defaultSortDirection: "desc",
    customComparator: (a, b, field) => {
      if (field === "changedAt") {
        return (
          new Date(a.changedAt).getTime() - new Date(b.changedAt).getTime()
        );
      }
      return 0;
    },
  });

  const columns: DataTableColumn<StockHistory>[] = [
    {
      header: "Date",
      field: "changedAt",
      render: (value) => new Date(value as string).toLocaleString(),
    },
    { header: "Changed By", field: "changedBy" },
    { header: "Product", field: "productName" },
    { header: "Warehouse", field: "warehouseName" },
    { header: "Previous", field: "previousQuantity", align: "right" },
    { header: "Change", field: "quantityChange", align: "right" },
    { header: "New", field: "newQuantity", align: "right" },
    { header: "Reason", field: "reason" },
  ];

  return (
    <div>
      <h2>Stock Change History</h2>
      <p>View history of stock level adjustments.</p>

      {error && <p className={styles.errorAlert}>{error}</p>}
      {loading && <p>Loading history...</p>}

      <div className={styles.sectionLarge}>
        <h3>Stock History ({sortedStockHistory.length})</h3>
        <DataTable
          data={sortedStockHistory}
          columns={columns}
          keyField="id"
          onSort={handleStockSort}
          sortIndicator={stockSortIndicator}
          emptyMessage="No history records to display."
          headerPadding="0.75rem"
          cellPadding="0.75rem"
        />
      </div>
    </div>
  );
};

export default HistoryPage;
