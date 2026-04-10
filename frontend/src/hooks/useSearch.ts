import { useState, useMemo } from "react";

interface UseSearchReturn<T> {
  searchQuery: string;
  setSearchQuery: (query: string) => void;
  filteredData: T[];
}

export function useSearch<T>(
  data: T[],
  searchFields: (keyof T)[],
): UseSearchReturn<T> {
  const [searchQuery, setSearchQuery] = useState("");

  const filteredData = useMemo(() => {
    const trimmed = searchQuery.trim().toLowerCase();
    if (!trimmed) return data;

    return data.filter((item) =>
      searchFields.some((field) => {
        const value = item[field];
        if (typeof value === "string")
          return value.toLowerCase().includes(trimmed);
        if (typeof value === "number") return String(value).includes(trimmed);
        return false;
      }),
    );
  }, [data, searchQuery, searchFields]);

  return { searchQuery, setSearchQuery, filteredData };
}
