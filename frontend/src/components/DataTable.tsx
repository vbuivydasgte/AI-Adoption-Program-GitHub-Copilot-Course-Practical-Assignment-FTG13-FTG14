import React from "react";

export interface DataTableColumn<T> {
  header: string;
  field: keyof T;
  align?: "left" | "right" | "center";
  sortable?: boolean;
  render?: (value: T[keyof T], row: T) => React.ReactNode;
}

interface DataTableProps<T> {
  readonly data: readonly T[];
  readonly columns: readonly DataTableColumn<T>[];
  readonly keyField: keyof T;
  onSort?: (field: keyof T) => void;
  sortIndicator?: (field: keyof T) => string;
  emptyMessage?: string;
  headerPadding?: string;
  cellPadding?: string;
}

/**
 * Generic reusable data table component with sorting support
 * @param data - Array of data to display
 * @param columns - Column configuration
 * @param keyField - Field to use as row key
 * @param onSort - Optional sort handler from useSortableData
 * @param sortIndicator - Optional sort indicator from useSortableData
 * @param emptyMessage - Message to display when no data
 * @param headerPadding - Custom header padding (default: "1rem")
 * @param cellPadding - Custom cell padding (default: "1rem")
 */
export function DataTable<T>({
  data,
  columns,
  keyField,
  onSort,
  sortIndicator,
  emptyMessage = "No data found.",
  headerPadding = "1rem",
  cellPadding = "1rem",
}: Readonly<DataTableProps<T>>) {
  if (data.length === 0) {
    return <p>{emptyMessage}</p>;
  }

  const handleHeaderClick = (field: keyof T, sortable: boolean = true) => {
    if (sortable && onSort) {
      onSort(field);
    }
  };

  return (
    <div className="table-scroll" style={{ marginTop: "1rem" }}>
      <table
        style={{
          width: "100%",
          borderCollapse: "collapse",
          background: "white",
          boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
          color: "#111827",
        }}
      >
        <thead>
          <tr style={{ background: "#2c3e50", color: "white" }}>
            {columns.map((column) => {
              const isSortable =
                column.sortable !== false && onSort !== undefined;
              return (
                <th
                  key={String(column.field)}
                  onClick={() => handleHeaderClick(column.field, isSortable)}
                  style={{
                    padding: headerPadding,
                    textAlign: column.align || "left",
                    cursor: isSortable ? "pointer" : "default",
                  }}
                >
                  {column.header}
                  {isSortable && sortIndicator?.(column.field)}
                </th>
              );
            })}
          </tr>
        </thead>
        <tbody>
          {data.map((row) => (
            <tr
              key={String(row[keyField])}
              style={{ borderBottom: "1px solid #ddd" }}
            >
              {columns.map((column) => {
                const value = row[column.field];
                const content = column.render
                  ? column.render(value, row)
                  : String(value ?? "");

                return (
                  <td
                    key={String(column.field)}
                    style={{
                      padding: cellPadding,
                      textAlign: column.align || "left",
                    }}
                  >
                    {content}
                  </td>
                );
              })}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
