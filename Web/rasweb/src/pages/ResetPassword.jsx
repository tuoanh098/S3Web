import { useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { AuthApi } from "../api";

export default function ResetPassword() {
    const [searchParams] = useSearchParams();
    const token = searchParams.get("token") || "";
    const email = searchParams.get("email") || "";
    const [pwd, setPwd] = useState("");
    const [pwd2, setPwd2] = useState("");
    const [err, setErr] = useState("");
    const [ok, setOk] = useState(false);
    const [loading, setLoading] = useState(false);
    const nav = useNavigate();

    const submit = async (e) => {
        e.preventDefault();
        setErr("");
        if (!email || !token) {
            setErr("Invalid reset link. Email or token missing.");
            return;
        }
        if (pwd.length < 8) {
            setErr("Password must be at least 8 characters.");
            return;
        }
        if (pwd !== pwd2) {
            setErr("Passwords do not match.");
            return;
        }
        setLoading(true);
        try {
            await AuthApi.resetPassword(email, token, pwd);
            setOk(true);
            setTimeout(() => nav("/login"), 1800);

        } catch (ex) {
            const msg = ex.message || "Failed to reset password.";
            setErr(msg);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="w-full max-w-md bg-white rounded-2xl shadow p-6">
            <h2 className="text-2xl font-semibold text-ras-blue-900 mb-2">Reset password</h2>
            <p className="text-sm text-ras-blue-700 mb-4">Set a new password for <b>{email || "your account"}</b></p>

                {ok ? (
                    <div className="text-center space-y-4">
                        <div className="text-green-600">Password reset successful.</div>
                        <button onClick={() => nav("/login")} className="mt-2 w-full rounded-xl bg-ras-blue-800 text-white py-2">Back to login</button>
                    </div>
                ) : (
                    <form onSubmit={submit} className="space-y-4">
                        {err && <div className="text-sm text-ras-coral-700 bg-ras-coral-50 p-2 rounded">{String(err)}</div>}

                        <div>
                            <label className="block text-sm text-ras-blue-800">New password</label>
                            <input type="password" value={pwd} onChange={e => setPwd(e.target.value)}
                                className="mt-1 w-full rounded-xl border px-3 py-2" placeholder="Enter new password" />
                        </div>

                        <div>
                            <label className="block text-sm text-ras-blue-800">Confirm password</label>
                            <input type="password" value={pwd2} onChange={e => setPwd2(e.target.value)}
                                className="mt-1 w-full rounded-xl border px-3 py-2" placeholder="Confirm new password" />
                        </div>

                        <button type="submit" disabled={loading}
                            className="w-full rounded-xl bg-ras-blue-800 text-white py-2">
                            {loading ? "Resetting..." : "Reset password"}
                        </button>

                        <div className="text-sm text-center text-ras-blue-700">
                            <a href="/login" className="hover:underline">Back to login</a>
                        </div>
                    </form>
                )}
            </div>
    );
}
