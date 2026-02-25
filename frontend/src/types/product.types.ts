export interface Product {
  id: number;
  name: string;
  sku: string;
  description?: string;
  createdAt: string;
  modifiedAt: string;
}

export interface CreateProductDto {
  name: string;
  sku: string;
  description?: string;
}

export interface UpdateProductDto {
  name: string;
  sku: string;
  description?: string;
}
