import axios from "axios";

export const IDENTITY = import.meta.env.VITE_IDENTITY_API || "";

let accessToken = localStorage.getItem("access_token") || null;
export const setAccessToken = (t) => {
    accessToken = t;
    if (t) localStorage.setItem("access_token", t); else localStorage.removeItem("access_token");
};

export const httpAuthed = axios.create(); // used by feature APIs to attach token

httpAuthed.interceptors.request.use(cfg => {
    if (accessToken) cfg.headers.Authorization = `Bearer ${accessToken}`;
    return cfg;
});

// 401 -> try refresh once then retry
let refreshing = null;
httpAuthed.interceptors.response.use(
    r => r,
    async err => {
        if (err.response?.status !== 401) throw err;
        if (!refreshing) {
            refreshing = fetch(`${IDENTITY}/auth/refresh`, { method: "POST", credentials: "include" })
                .then(r => r.ok ? r.json() : Promise.reject(err))
                .then(d => { setAccessToken(d.access_token); })
                .finally(() => { refreshing = null; });
        }
        await refreshing;
        err.config.headers.Authorization = `Bearer ${accessToken}`;
        return axios(err.config);
    }
);

async err => {
    if (err.response?.status !== 401) throw err;
    if (!refreshing) {
        refreshing = fetch(`${IDENTITY}/auth/refresh`, { method: "POST", credentials: "include" })
            .then(r => r.okZ ? r.json() : Promise.reject(err))
            .then(d => { setAccessToken(d.access_token); })
            .catch(() => {
                // refresh failed -> nuke token so guards kick user to /login
                setAccessToken(null);
                // optional: window.location.assign("/login");
            })
            .finally(() => { refreshing = null; });
    }
    await refreshing;
    if (!localStorage.getItem("access_token")) throw err; // refresh failed
    err.config.headers.Authorization = `Bearer ${localStorage.getItem("access_token")}`;
    return axios(err.config);
}

