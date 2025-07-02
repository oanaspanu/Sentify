import React, { createContext, useContext, useState, useEffect } from "react";
import { api } from "../service/api";

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);

  useEffect(() => {
    const storedUser = localStorage.getItem("user");
    const token = localStorage.getItem("token");

    if (storedUser && token) {
      setUser(JSON.parse(storedUser));
      api.defaults.headers["Authorization"] = `Bearer ${token}`;
    }
  }, []);

  const login = async (username, password) => {
    try {
      const res = await api.post("/auth/login", { username, password });

      const userData = {
        id: res.data.id,
        username: res.data.username,
      };

      localStorage.setItem("token", res.data.token);
      localStorage.setItem("user", JSON.stringify(userData));

      setUser(userData);
      api.defaults.headers["Authorization"] = `Bearer ${res.data.token}`;

      console.log("User logged in:", userData.username);
    } catch (err) {
      console.error("Login error:", err);
      throw new Error(err.response?.data?.message || "Login failed");
    }
  };

  const register = async (userData) => {
    await api.post("/auth/register", userData);
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
    delete api.defaults.headers["Authorization"];
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, register }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
