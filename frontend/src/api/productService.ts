import type {
  Product,
  CreateProductDto,
  UpdateProductDto,
} from "../types/product.types";
import { createCrudService } from "./crudServiceFactory";

export const productService = createCrudService<
  Product,
  CreateProductDto,
  UpdateProductDto
>("/products");
