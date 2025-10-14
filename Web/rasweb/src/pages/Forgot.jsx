import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { AuthApi } from "../api";

const IDENTITY = import.meta.env.VITE_IDENTITY_API || "";

export default function Forgot() {
    const [email, setEmail] = useState("");
    const [err, setErr] = useState("");
    const [sent, setSent] = useState(false);
    const [loading, setLoading] = useState(false);
    const nav = useNavigate();

    // 2. Updated submit function
    const submit = async (e) => {
        e.preventDefault();
        setErr("");
        if (!email || !/^\S+@\S+\.\S+$/.test(email)) {
            setErr("Please enter a valid email address.");
            return;
        }
        setLoading(true);
        try {
            // This now uses your centralized API function
            await AuthApi.forgot(email);
            setSent(true);
        } catch (ex) {
            // To prevent email enumeration attacks, we show the same success message
            // even if the email doesn't exist in the system.
            setSent(true);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="w-full max-w-lg sm:max-w-xl bg-white rounded-3xl shadow-[0_16px_40px_-12px_rgba(31,31,77,.18)] p-6 sm:p-8">
        <h1 className="text-center font-display text-3xl sm:text-4xl font-semibold text-ras-blue-800 leading-tight mb-4">
            Reset password
        </h1>

        {sent ? (
            <div className="space-y-6">
                <p className="text-sm text-ras-blue-800/90 text-center">
                    Sending reset password link to <b>{email}</b>,
                    Please wait 30s and check your inbox.
                </p>
                <button
                    onClick={() => nav("/login", { replace: true })}
                    className="w-full h-12 rounded-xl bg-ras-blue-800 text-white font-medium hover:bg-ras-blue-700 focus:ring-2 focus:ring-ras-blue-400 transition"
                >
                    Back to login
                </button>
            </div>
        ) : (
        <form onSubmit={submit} className="space-y-5">
            <p className="text-sm text-ras-blue-800/90">
                Please enter your email to reset your password.
            </p>

            {err && (
                <div className="text-sm text-ras-coral-700 bg-ras-coral-50 border border-ras-coral-200 rounded-xl p-2.5">
                    {err}
                </div>
            )}

            <label className="block">
                <span className="text-sm text-ras-blue-800">Email address *</span>
                <input
                    className="mt-1 w-full h-12 rounded-xl border border-ras-blue-200 px-4 outline-none focus:ring-2 focus:ring-ras-blue-400 focus:border-ras-blue-400 text-[15px]"
                    placeholder="you@example.com"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    type="email"
                    autoComplete="email"
                />
            </label>

            <div className="flex flex-col sm:flex-row gap-3">
                <button
                    type="submit"
                    disabled={loading}
                    className="flex-1 h-12 rounded-xl bg-ras-blue-800 text-white font-medium hover:bg-ras-blue-700 focus:ring-2 focus:ring-ras-blue-400 transition disabled:opacity-60"
                >
                    {loading ? "Sending..." : "Send"}
                </button>
                <button
                    type="button"
                    onClick={() => nav("/login")}
                    className="flex-1 h-12 rounded-xl border border-ras-blue-200 text-ras-blue-900 hover:bg-ras-blue-50"
                >
                    Cancel
                </button>
            </div>
        </form>
         )}
        <div className="mt-10 text-center text-[11px] text-ras-blue-800/70 space-x-3">
            <a href="#" className="hover:underline">Privacy Policy</a><span></span>
            <a href="#" className="hover:underline">Cookie Policy</a><span></span>
            <a href="#" className="hover:underline">Terms of Use</a>
            <div className="mt-1"> {new Date().getFullYear()} RAS Music &amp; Art Studio</div>
        </div>
        </div>
    );
}
