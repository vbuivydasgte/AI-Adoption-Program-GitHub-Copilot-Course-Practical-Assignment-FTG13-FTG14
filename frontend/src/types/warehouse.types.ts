export interface Warehouse {
  id: number;
  name: string;
  location: string;
  createdAt: string;
  modifiedAt: string;
}

export interface CreateWarehouseDto {
  name: string;
  location: string;
}

export interface UpdateWarehouseDto {
  name: string;
  location: string;
}
