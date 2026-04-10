import { productService } from "../../api/productService";
import type { Product } from "../../types/product.types";
import { useSortableData } from "../../hooks/useSortableData";
import { useDataFetching } from "../../hooks/useDataFetching";
import { useSearch } from "../../hooks/useSearch";
import { DataTable } from "../../components/DataTable";
import type { DataTableColumn } from "../../components/DataTable";
import styles from "../PageStyles.module.css";

const PRODUCT_SEARCH_FIELDS: (keyof Product)[] = ["name", "sku", "description"];

const ProductsPage = () => {
  const {
    data: products,
    loading,
    error,
  } = useDataFetching<Product[]>(() => productService.getAll(), [], {
    errorMessage: "Failed to load products",
  });

  const {
    searchQuery,
    setSearchQuery,
    filteredData: filteredProducts,
  } = useSearch<Product>(products, PRODUCT_SEARCH_FIELDS);

  const {
    sortedData: sortedProducts,
    handleSort,
    sortIndicator,
  } = useSortableData<Product>(filteredProducts, {
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

  if (loading) return <div>Loading products...</div>;
  if (error) return <div className={styles.error}>{error}</div>;

  return (
    <div>
      <h1>Products</h1>
      <p>Total Products: {products.length}</p>

      <input
        type="search"
        value={searchQuery}
        onChange={(e) => setSearchQuery(e.target.value)}
        placeholder="Search by name, SKU or description..."
        className={styles.searchBar}
      />

      <DataTable
        data={sortedProducts}
        columns={columns}
        keyField="id"
        onSort={handleSort}
        sortIndicator={sortIndicator}
        emptyMessage="No products match your search."
      />
    </div>
  );
};

export default ProductsPage;
