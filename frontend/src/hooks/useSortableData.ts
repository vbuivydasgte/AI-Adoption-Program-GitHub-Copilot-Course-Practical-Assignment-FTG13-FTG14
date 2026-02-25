import { useState, useMemo } from "react";

export type SortDirection = "asc" | "desc";

type SortConfig<T> = {
  field: keyof T;
  direction: SortDirection;
};

interface UseSortableDataOptions<T> {
  defaultSortField?: keyof T;
  defaultSortDirection?: SortDirection;
  customComparator?: (a: T, b: T, field: keyof T) => number;
}

interface UseSortableDataReturn<T> {
  sortedData: T[];
  handleSort: (field: keyof T) => void;
  sortIndicator: (field: keyof T) => string;
  sortField: keyof T;
  sortDirection: SortDirection;
}

/**
 * Generic hook for sortable data tables
 * @param data - Array of data to sort
 * @param options - Configuration options
 * @returns Sorted data and sort controls
 */
export function useSortableData<T>(
  data: T[],
  options: UseSortableDataOptions<T> = {},
): UseSortableDataReturn<T> {
  const {
    defaultSortField,
    defaultSortDirection = "asc",
    customComparator,
  } = options;

  const [sortConfig, setSortConfig] = useState<SortConfig<T>>({
    field: defaultSortField ?? (Object.keys(data[0] ?? {})[0] as keyof T),
    direction: defaultSortDirection,
  });

  const handleSort = (field: keyof T) => {
    setSortConfig((prevConfig) => {
      if (prevConfig.field === field) {
        return {
          ...prevConfig,
          direction: prevConfig.direction === "asc" ? "desc" : "asc",
        };
      }
      return {
        field,
        direction: "asc",
      };
    });
  };

  const sortedData = useMemo(() => {
    const sortableData = [...data];

    if (customComparator) {
      sortableData.sort((a, b) => {
        const result = customComparator(a, b, sortConfig.field);
        return sortConfig.direction === "asc" ? result : -result;
      });
      return sortableData;
    }

    sortableData.sort((a, b) => {
      const aValue = a[sortConfig.field];
      const bValue = b[sortConfig.field];

      // Handle null/undefined values
      if (aValue == null && bValue == null) return 0;
      if (aValue == null) return 1;
      if (bValue == null) return -1;

      // Handle number comparison
      if (typeof aValue === "number" && typeof bValue === "number") {
        return sortConfig.direction === "asc"
          ? aValue - bValue
          : bValue - aValue;
      }

      // Handle string comparison
      const aString = aValue.toString().toLowerCase();
      const bString = bValue.toString().toLowerCase();

      if (aString < bString) {
        return sortConfig.direction === "asc" ? -1 : 1;
      }
      if (aString > bString) {
        return sortConfig.direction === "asc" ? 1 : -1;
      }
      return 0;
    });

    return sortableData;
  }, [data, sortConfig, customComparator]);

  const sortIndicator = (field: keyof T) => {
    if (sortConfig.field !== field) {
      return "";
    }
    return sortConfig.direction === "asc" ? " ↑" : " ↓";
  };

  return {
    sortedData,
    handleSort,
    sortIndicator,
    sortField: sortConfig.field,
    sortDirection: sortConfig.direction,
  };
}
