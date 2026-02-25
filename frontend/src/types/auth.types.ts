export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  username: string;
  role: string;
}

export interface AuthUser {
  username: string;
  role: string;
  token: string;
}
