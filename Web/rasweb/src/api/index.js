import { IDENTITY, GATEWAY, setAccessToken, httpAuthed } from "../lib/httpAuth";

async function jsonFetch(url, options = {}) {
    const headers = { ...(options.headers || {}) };
    if (options.body) headers["Content-Type"] = "application/json";

    const res = await fetch(url, {
        method: options.method || "GET",
        headers,
        body: options.body ? JSON.stringify(options.body) : undefined,
        credentials: options.withCreds ? "include" : "omit",
    });

    if (!res.ok) {
        const text = await res.text();
        let parsed;
        try { parsed = JSON.parse(text); } catch { }
        const errMsg = parsed?.error || text || res.statusText;
        const err = new Error(errMsg);
        err.status = res.status;
        throw err;
    }

    if (res.status === 204) return undefined;
    const txt = await res.text();
    if (!txt) return undefined;
    try { return JSON.parse(txt); } catch { return txt; }
}


export const AuthApi = {
    register: (email, password) => {
        const base = GATEWAY || IDENTITY;
        return jsonFetch(`${base}/auth/register`, { method: "POST", body: { email, password } });
    },

    async login(email, password) {
        const payload = { username: email, password };
        const endpoint = GATEWAY ? `${GATEWAY}/gateway/login` : `${IDENTITY}/auth/login`;
        const data = await jsonFetch(endpoint, { method: "POST", body: payload, withCreds: true });

        if (data?.access_token) setAccessToken(data.access_token);
        if (data?.refresh_token) {
            try { localStorage.setItem("refresh_token", data.refresh_token); } catch { }
        }
        if (data?.user) {
            try { localStorage.setItem("identity", JSON.stringify(data.user)); } catch { }
        }
        if (data?.profile) {
            try { localStorage.setItem("profile", JSON.stringify(data.profile)); } catch { }
        }

        return data;
    },

    async refresh() {
        const refreshToken = localStorage.getItem("refresh_token");
        if (!refreshToken) throw new Error("no_refresh_token");

        const endpoint = GATEWAY ? `${GATEWAY}/gateway/refresh` : `${IDENTITY}/auth/refresh`;
        const data = await jsonFetch(endpoint, { method: "POST", withCreds: true, body: { RefreshToken: refreshToken } });

        if (data?.access_token) setAccessToken(data.access_token);
        if (data?.refresh_token) {
            // rotate refresh token if server returned a new one
            localStorage.setItem("refresh_token", data.refresh_token);
        }

        // optionally update identity/profile if returned
        if (data?.user) {
            localStorage.setItem("identity", JSON.stringify(data.user));
        }
        if (data?.profile) {
            localStorage.setItem("profile", JSON.stringify(data.profile));
        }

        return data;
    },

    async logout() {
        const refreshToken = localStorage.getItem("refresh_token");
        const endpoint = GATEWAY ? `${GATEWAY}/gateway/logout` : `${IDENTITY}/auth/logout`;

        if (refreshToken) {
            try {
                // send refresh token to logout endpoint (server will revoke)
                await jsonFetch(endpoint, { method: "POST", withCreds: true, body: { RefreshToken: refreshToken } });
            } catch (err) {
                // ignore errors from remote, still clear local
            }
        }

        // Clear client state
        setAccessToken(null);
        localStorage.removeItem("refresh_token");
        localStorage.removeItem("identity");
        localStorage.removeItem("profile");
    },
};