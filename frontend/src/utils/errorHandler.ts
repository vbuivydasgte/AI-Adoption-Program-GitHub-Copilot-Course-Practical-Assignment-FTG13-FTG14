import axios from "axios";

/**
 * Extracts error message from API response
 * @param error - The error object from axios or other source
 * @param fallbackMessage - Default message if specific error cannot be extracted
 * @returns The error message to display
 */
export const getErrorMessage = (
  error: unknown,
  fallbackMessage: string = "An error occurred",
): string => {
  if (axios.isAxiosError(error)) {
    // Extract message from backend response (e.g., { message: "Stock quantity cannot be negative." })
    return error.response?.data?.message || fallbackMessage;
  }

  if (error instanceof Error) {
    return error.message;
  }

  return fallbackMessage;
};
