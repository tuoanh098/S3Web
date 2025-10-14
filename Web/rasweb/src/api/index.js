/**
 * API surface for RasStudent FE <-> BE
 */
import { IDENTITY, setAccessToken } from "../lib/httpAuth";

async function jsonFetch(url, options = {}) {
    const headers = { ...options.headers };
    if (options.body) {
        headers["Content-Type"] = "application/json";
    }

    const res = await fetch(url, {
        ...options,
        headers,
        // If the body is a JS object, stringify it
        body: options.body ? JSON.stringify(options.body) : undefined,
        // Handle credentials option
        credentials: options.withCreds ? "include" : "omit",
    });

    if (!res.ok) {
        // Try to parse error text, then fall back to status text
        const errorText = await res.text();
        throw new Error(errorText || res.statusText);
    }

    // Handle 204 No Content response
    if (res.status === 204) {
        return undefined;
    }

    return res.json();
}

/* ----------------------- AUTH SERVICE -----------------------
   Base:  VITE_IDENTITY_API (e.g., https://localhost:7171)

   POST /auth/register?email&password
   POST /auth/login?email&password -> { access_token, token_type, roles } + Set-Cookie(rt)
   POST /auth/refresh              -> { access_token, token_type } (uses cookie)
   POST /auth/logout               -> 204 (clears cookie)
   POST /auth/forgot?email         -> { reset_token } (DEV) or { sent: true }
   GET  /auth/me                   -> { email, userName, roles } (requires bearer)
---------------------------------------------------------------- */

export const AuthApi = {
    seed: () => jsonFetch(`${IDENTITY}/auth/seed`, { method: "POST" }),
    register: (email, password) =>
        jsonFetch(`${IDENTITY}/auth/register?email=${encodeURIComponent(email)}&password=${encodeURIComponent(password)}`, { method: "POST" }),

    async login(email, password) {
        const data = await jsonFetch(`${IDENTITY}/auth/login`, {
            method: "POST",
            withCreds: true,
            body: { email, password }
        });
        setAccessToken(data.access_token);
        return data;
    },

    async refresh() {
        const data = await jsonFetch(`${IDENTITY}/auth/refresh`, { method: "POST", withCreds: true });
        setAccessToken(data.access_token);
        return data;
    },

    logout: async () => {
        await jsonFetch(`${IDENTITY}/auth/logout`, { method: "POST", withCreds: true });
        setAccessToken(null);
    },

    forgot: (email) =>
        jsonFetch(`${IDENTITY}/auth/forgot`, {
            method: "POST",
            body: { email }
        }),

    register: (email, password) =>
        jsonFetch(`${IDENTITY}/auth/register`, {
            method: "POST",
            body: { email, password }
        }),

    resetPassword: (email, token, password) =>
        jsonFetch(`${IDENTITY}/auth/reset`, {
            method: "POST",
            body: { email, token, NewPassword: password } // <-- Change "password" to "NewPassword"
        }),

    me: () =>
        jsonFetch(`${IDENTITY}/auth/me`, {
            headers: { Authorization: `Bearer ${localStorage.getItem("access_token") || ""}` },
        }),
};

/* ----------------------- STUDENTS SERVICE -----------------------
   Base:  VITE_STUDENTS_API (e.g., https://localhost:7181)

   GET    /health                     -> {status, service}
   GET    /db-ping                    -> true/false (can connect)
   GET    /api/students               -> Student[]
   GET    /api/students/{id}
   POST   /api/students               -> create { fullName, email }
   PUT    /api/students/{id}
   DELETE /api/students/{id}
------------------------------------------------------------------ */

export const StudentsApi = {
    health: () => http.get(`/health`).then(r => r.data),
    dbPing: () => http.get(`/db-ping`).then(r => r.data),

    list: () => http.get(`/api/students`).then(r => r.data),
    get: (id) => http.get(`/api/students/${id}`).then(r => r.data),
    create: (dto) => http.post(`/api/students`, dto).then(r => r.data),
    update: (id, dto) => http.put(`/api/students/${id}`, dto).then(r => r.data),
    remove: (id) => http.delete(`/api/students/${id}`).then(r => r.data),
};

/* ------------------ OPTIONAL EXAMPLE USAGE ------------------
import { AuthApi, StudentsApi } from "./api";

await AuthApi.seed();
await AuthApi.login("admin@ras.local","Passw0rd!");
const me = await AuthApi.me();
const list = await StudentsApi.list();
-------------------------------------------------------------- */
