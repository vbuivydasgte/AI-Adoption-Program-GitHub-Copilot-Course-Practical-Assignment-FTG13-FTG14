import axiosInstance from './axiosConfig';
import type { Stock, StockAdjustmentDto } from '../types/stock.types';

export const stockService = {
  getAll: async (): Promise<Stock[]> => {
    const response = await axiosInstance.get<Stock[]>('/stock');
    return response.data;
  },

  getById: async (id: number): Promise<Stock> => {
    const response = await axiosInstance.get<Stock>(`/stock/${id}`);
    return response.data;
  },

  getByWarehouse: async (warehouseId: number): Promise<Stock[]> => {
    const response = await axiosInstance.get<Stock[]>(`/stock/warehouse/${warehouseId}`);
    return response.data;
  },

  getByProduct: async (productId: number): Promise<Stock[]> => {
    const response = await axiosInstance.get<Stock[]>(`/stock/product/${productId}`);
    return response.data;
  },

  adjust: async (adjustment: StockAdjustmentDto): Promise<Stock> => {
    const response = await axiosInstance.post<Stock>('/stock/adjust', adjustment);
    return response.data;
  },
};
