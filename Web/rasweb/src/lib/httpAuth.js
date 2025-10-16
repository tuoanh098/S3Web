// src/lib/httpAuth.js
// Exports:
//  - IDENTITY: Identity base URL (from VITE_IDENTITY_URL)
//  - GATEWAY: Gateway base URL (from VITE_GATEWAY_URL)
//  - setAccessToken / getAccessToken
//  - httpAuthed: axios instance with Authorization header attached (baseURL = GATEWAY by default)

import axios from "axios";

export const IDENTITY = import.meta.env.VITE_IDENTITY_URL || "";
export const GATEWAY = import.meta.env.VITE_GATEWAY_URL || "";
export const TIMEOUT = Number(import.meta.env.VITE_API_TIMEOUT_MS || 15000);

export const setAccessToken = (t) => {
    if (t) localStorage.setItem("access_token", t);
    else localStorage.removeItem("access_token");
};

export const getAccessToken = () => localStorage.getItem("access_token");

// axios instance used for authenticated requests
export const httpAuthed = axios.create({
    baseURL: GATEWAY || IDENTITY || "",
    timeout: TIMEOUT,
});

// attach bearer header if available
httpAuthed.interceptors.request.use(cfg => {
    const token = getAccessToken();
    if (token) {
        cfg.headers = {
            ...cfg.headers,
            Authorization: `Bearer ${token}`,
        };
    }
    return cfg;
});

// refresh-on-401: try refresh once (via gateway refresh or identity refresh), then retry original
let refreshingPromise = null;

httpAuthed.interceptors.response.use(
    res => res,
    async err => {
        const originalReq = err.config;
        if (!err.response || err.response.status !== 401) {
            throw err;
        }

        // avoid infinite loop
        if (originalReq && originalReq._retry) {
            throw err;
        }

        // if a refresh is already in progress, wait for it
        if (!refreshingPromise) {
            refreshingPromise = (async () => {
                try {
                    const refreshUrl = GATEWAY ? `${GATEWAY}/gateway/refresh` : `${IDENTITY}/auth/refresh`;
                    const resp = await fetch(refreshUrl, {
                        method: "POST",
                        credentials: "include", // if refresh uses http-only cookie
                        headers: { "Content-Type": "application/json" },
                    });

                    if (!resp.ok) {
                        setAccessToken(null);
                        throw new Error("refresh_failed");
                    }

                    const data = await resp.json();
                    if (!data?.access_token) {
                        setAccessToken(null);
                        throw new Error("refresh_no_token");
                    }

                    setAccessToken(data.access_token);
                    return data;
                } finally {
                    // nothing here
                }
            })().finally(() => { refreshingPromise = null; });
        }

        await refreshingPromise;

        const newToken = getAccessToken();
        if (!newToken) throw err;

        originalReq._retry = true;
        originalReq.headers = {
            ...originalReq.headers,
            Authorization: `Bearer ${newToken}`,
        };

        return axios(originalReq);
    }
);
