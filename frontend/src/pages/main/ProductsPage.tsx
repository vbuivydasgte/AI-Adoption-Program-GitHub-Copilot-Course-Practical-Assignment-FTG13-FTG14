import { productService } from "../../api/productService";
import type { Product } from "../../types/product.types";
import { useSortableData } from "../../hooks/useSortableData";
import { useDataFetching } from "../../hooks/useDataFetching";
import { DataTable } from "../../components/DataTable";
import type { DataTableColumn } from "../../components/DataTable";
import styles from "../PageStyles.module.css";

const ProductsPage = () => {
  const {
    data: products,
    loading,
    error,
  } = useDataFetching<Product[]>(() => productService.getAll(), [], {
    errorMessage: "Failed to load products",
  });

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

  if (loading) return <div>Loading products...</div>;
  if (error) return <div className={styles.error}>{error}</div>;

  return (
    <div>
      <h1>Products</h1>
      <p>Total Products: {products.length}</p>

      <DataTable
        data={sortedProducts}
        columns={columns}
        keyField="id"
        onSort={handleSort}
        sortIndicator={sortIndicator}
        emptyMessage="No products found."
      />
    </div>
  );
};

export default ProductsPage;
