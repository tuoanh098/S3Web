import React, { createContext, useContext, useEffect, useMemo, useState } from "react";
import { AuthApi } from "../api";
import { setAccessToken } from "../lib/httpAuth";
import jwtDecode from "jwt-decode";

const Ctx = createContext(null);

export function AuthProvider({ children }) {
    // 1. Initialize token state from localStorage
    const [token, setToken] = useState(localStorage.getItem("access_token"));

    // 2. Decode the token to get user info
    const user = useMemo(() => {
        if (!token) return null;
        try {
            return jwtDecode(token);
        } catch {
            return null;
        }
    }, [token]);

    // 3. CRITICAL: This effect syncs the token with the HTTP client and localStorage
    useEffect(() => {
        if (token) {
            // If token exists, set it for all future API requests
            setAccessToken(token);
            localStorage.setItem("access_token", token);
        } else {
            // If token is null (on logout), clear it
            setAccessToken(null);
            localStorage.removeItem("access_token");
        }
    }, [token]); // This effect runs whenever the token changes

    const login = async (email, password) => {
        const data = await AuthApi.login(email, password);
        // This will trigger the useEffect hook above
        setToken(data.access_token);
    };

    const logout = async () => {
        try {
            await AuthApi.logout();
        } catch { }
        // This will also trigger the useEffect hook
        setToken(null);
    };

    return <Ctx.Provider value={{ user, token, login, logout }}>{children}</Ctx.Provider>;
}

export const useAuth = () => useContext(Ctx);