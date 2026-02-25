import axiosInstance from './axiosConfig';
import type { LoginRequest, LoginResponse } from '../types/auth.types';

export const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await axiosInstance.post<LoginResponse>('/auth/login', credentials);
    return response.data;
  },

  register: async (credentials: LoginRequest): Promise<void> => {
    await axiosInstance.post('/auth/register', credentials);
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },
};
