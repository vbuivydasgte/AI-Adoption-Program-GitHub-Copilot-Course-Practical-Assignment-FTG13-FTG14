import axiosInstance from "./axiosConfig";
import type { StockHistory } from "../types/history.types";

export const historyService = {
  getAllStockHistory: async (): Promise<StockHistory[]> => {
    const response = await axiosInstance.get<StockHistory[]>("/history/stock");
    return response.data;
  },

  getStockHistory: async (stockId: number): Promise<StockHistory[]> => {
    const response = await axiosInstance.get<StockHistory[]>(
      `/history/stock/${stockId}`,
    );
    return response.data;
  },

  getStockHistoryByProduct: async (
    productId: number,
  ): Promise<StockHistory[]> => {
    const response = await axiosInstance.get<StockHistory[]>(
      `/history/stock/product/${productId}`,
    );
    return response.data;
  },

  getStockHistoryByWarehouse: async (
    warehouseId: number,
  ): Promise<StockHistory[]> => {
    const response = await axiosInstance.get<StockHistory[]>(
      `/history/stock/warehouse/${warehouseId}`,
    );
    return response.data;
  },
};
