import { createContext, useContext, useMemo, useState } from "react";
import { AuthApi } from "../api";
import { setAccessToken } from "../lib/httpAuth";
import jwtDecode from "jwt-decode";

const Ctx = createContext(null);

export function AuthProvider({ children }) {
    const [token, setToken] = useState(localStorage.getItem("access_token"));

    const user = useMemo(() => {
        if (!token) return null;
        try { return jwtDecode(token); } catch { }
    }, [token]);

    const login = async (email, password) => {
        const data = await AuthApi.login(email, password);
        setToken(data.access_token);
    };

    const logout = async () => {
        try { await AuthApi.logout(); } catch { }
        setToken(null);
    };

    return <Ctx.Provider value={{ user, token, login, logout }}>{children}</Ctx.Provider>;
}
export const useAuth = () => useContext(Ctx);