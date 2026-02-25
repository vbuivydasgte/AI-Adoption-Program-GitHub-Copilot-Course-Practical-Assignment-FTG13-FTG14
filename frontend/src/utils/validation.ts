/**
 * Frontend validation utilities matching backend DTO validation rules
 */

export interface ValidationResult {
  isValid: boolean;
  errors: Record<string, string>;
}

/**
 * Validates product creation/update data
 */
export const validateProduct = (data: {
  name: string;
  sku: string;
  description?: string;
}): ValidationResult => {
  const errors: Record<string, string> = {};

  if (!data.name || data.name.trim().length === 0) {
    errors.name = "Product name is required";
  } else if (data.name.length > 100) {
    errors.name = "Product name cannot exceed 100 characters";
  }

  if (!data.sku || data.sku.trim().length === 0) {
    errors.sku = "SKU is required";
  } else if (data.sku.length > 50) {
    errors.sku = "SKU cannot exceed 50 characters";
  }

  if (data.description && data.description.length > 500) {
    errors.description = "Description cannot exceed 500 characters";
  }

  return {
    isValid: Object.keys(errors).length === 0,
    errors,
  };
};

/**
 * Validates warehouse creation/update data
 */
export const validateWarehouse = (data: {
  name: string;
  location: string;
}): ValidationResult => {
  const errors: Record<string, string> = {};

  if (!data.name || data.name.trim().length === 0) {
    errors.name = "Warehouse name is required";
  } else if (data.name.length > 100) {
    errors.name = "Warehouse name cannot exceed 100 characters";
  }

  if (!data.location || data.location.trim().length === 0) {
    errors.location = "Location is required";
  } else if (data.location.length > 200) {
    errors.location = "Location cannot exceed 200 characters";
  }

  return {
    isValid: Object.keys(errors).length === 0,
    errors,
  };
};

/**
 * Validates stock adjustment data
 */
export const validateStockAdjustment = (data: {
  productId: number;
  warehouseId: number;
  quantityChange: number;
  reason: string;
  currentStock?: number;
}): ValidationResult => {
  const errors: Record<string, string> = {};

  if (!data.productId || data.productId <= 0) {
    errors.productId = "Please select a product";
  }

  if (!data.warehouseId || data.warehouseId <= 0) {
    errors.warehouseId = "Please select a warehouse";
  }

  if (data.quantityChange === 0) {
    errors.quantityChange = "Quantity change cannot be zero";
  } else if (data.quantityChange < -10000 || data.quantityChange > 10000) {
    errors.quantityChange = "Quantity change must be between -10000 and 10000";
  }

  if (
    data.currentStock !== undefined &&
    data.currentStock + data.quantityChange < 0
  ) {
    errors.quantityChange = `Cannot reduce stock below zero (current: ${data.currentStock})`;
  }

  if (!data.reason || data.reason.trim().length === 0) {
    errors.reason = "Reason is required";
  } else if (data.reason.length > 500) {
    errors.reason = "Reason cannot exceed 500 characters";
  }

  return {
    isValid: Object.keys(errors).length === 0,
    errors,
  };
};

/**
 * Validates login credentials
 */
export const validateLogin = (data: {
  username: string;
  password: string;
}): ValidationResult => {
  const errors: Record<string, string> = {};

  if (!data.username || data.username.trim().length === 0) {
    errors.username = "Username is required";
  } else if (data.username.length < 3) {
    errors.username = "Username must be at least 3 characters";
  } else if (data.username.length > 50) {
    errors.username = "Username cannot exceed 50 characters";
  }

  if (!data.password || data.password.length === 0) {
    errors.password = "Password is required";
  } else if (data.password.length < 6) {
    errors.password = "Password must be at least 6 characters";
  } else if (data.password.length > 100) {
    errors.password = "Password cannot exceed 100 characters";
  }

  return {
    isValid: Object.keys(errors).length === 0,
    errors,
  };
};
