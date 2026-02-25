export interface Stock {
  id: number;
  productId: number;
  productName: string;
  productSKU: string;
  warehouseId: number;
  warehouseName: string;
  quantity: number;
  lastUpdated: string;
}

export interface StockAdjustmentDto {
  productId: number;
  warehouseId: number;
  quantityChange: number;
  reason: string;
  changedBy: string;
}
