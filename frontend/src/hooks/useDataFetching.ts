import { useState, useEffect, useCallback, useRef } from "react";
import { getErrorMessage } from "../utils/errorHandler";

interface UseDataFetchingOptions {
  errorMessage?: string;
  immediate?: boolean;
}

interface UseDataFetchingReturn<T> {
  data: T;
  loading: boolean;
  error: string;
  refetch: () => Promise<void>;
  setData: React.Dispatch<React.SetStateAction<T>>;
  setError: React.Dispatch<React.SetStateAction<string>>;
}

/**
 * Generic hook for data fetching with loading and error states
 * @param fetchFn - Async function that fetches data
 * @param initialData - Initial value for data (e.g., empty array)
 * @param options - Optional configuration
 * @returns Data, loading state, error state, and refetch function
 */
export function useDataFetching<T>(
  fetchFn: () => Promise<T>,
  initialData: T,
  options: UseDataFetchingOptions = {},
): UseDataFetchingReturn<T> {
  const { errorMessage = "Failed to load data", immediate = true } = options;

  const [data, setData] = useState<T>(initialData);
  const [loading, setLoading] = useState(immediate);
  const [error, setError] = useState<string>("");
  const fetchFnRef = useRef(fetchFn);

  useEffect(() => {
    fetchFnRef.current = fetchFn;
  }, [fetchFn]);

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      setError("");
      const result = await fetchFnRef.current();
      setData(result);
    } catch (err) {
      setError(getErrorMessage(err, errorMessage));
      console.error(err);
    } finally {
      setLoading(false);
    }
  }, [errorMessage]);

  useEffect(() => {
    if (immediate) {
      fetchData();
    }
  }, [fetchData, immediate]);

  return {
    data,
    loading,
    error,
    refetch: fetchData,
    setData,
    setError,
  };
}
