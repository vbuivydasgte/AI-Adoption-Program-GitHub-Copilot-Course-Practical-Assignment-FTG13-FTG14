export interface StockHistory {
  id: number;
  stockId: number;
  productId: number;
  productName: string;
  warehouseId: number;
  warehouseName: string;
  changedBy: string;
  changedAt: string;
  previousQuantity: number;
  newQuantity: number;
  quantityChange: number;
  reason: string;
}
