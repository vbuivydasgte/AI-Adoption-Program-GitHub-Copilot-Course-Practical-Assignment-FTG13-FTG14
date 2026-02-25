import { useState, useMemo, useCallback } from "react";
import type { ReactNode } from "react";
import type { AuthUser, LoginRequest } from "../types/auth.types";
import { authService } from "../api/authService";
import { AuthContext } from "./authContext.config";

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider = ({ children }: AuthProviderProps) => {
  const [user, setUser] = useState<AuthUser | null>(() => {
    const token = localStorage.getItem("token");
    const userData = localStorage.getItem("user");

    if (token && userData) {
      try {
        const parsedUser = JSON.parse(userData);
        return { ...parsedUser, token };
      } catch (error) {
        console.error("Failed to parse user data:", error);
        localStorage.removeItem("token");
        localStorage.removeItem("user");
      }
    }
    return null;
  });
  const [loading] = useState(false);

  const login = useCallback(async (credentials: LoginRequest) => {
    const response = await authService.login(credentials);

    const authUser: AuthUser = {
      username: response.username,
      role: response.role,
      token: response.token,
    };

    localStorage.setItem("token", response.token);
    localStorage.setItem(
      "user",
      JSON.stringify({
        username: response.username,
        role: response.role,
      }),
    );

    setUser(authUser);
  }, []);

  const logout = useCallback(() => {
    authService.logout();
    setUser(null);
  }, []);

  const isAdmin = user?.role === "Admin";

  const value = useMemo(
    () => ({ user, loading, login, logout, isAdmin }),
    [user, loading, login, logout, isAdmin],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
