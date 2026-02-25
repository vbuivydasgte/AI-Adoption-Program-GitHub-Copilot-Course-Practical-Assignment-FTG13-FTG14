import type {
  Warehouse,
  CreateWarehouseDto,
  UpdateWarehouseDto,
} from "../types/warehouse.types";
import { createCrudService } from "./crudServiceFactory";

export const warehouseService = createCrudService<
  Warehouse,
  CreateWarehouseDto,
  UpdateWarehouseDto
>("/warehouses");
