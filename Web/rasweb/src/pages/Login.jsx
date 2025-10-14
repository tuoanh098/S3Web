import { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import BrandLogo from "../ui/BrandLogo";

export default function Login() {
    const [userOrEmail, setUserOrEmail] = useState("");
    const [password, setPassword] = useState("");
    const [showPwd, setShowPwd] = useState(false);
    const [err, setErr] = useState("");
    const { login } = useAuth();
    const nav = useNavigate();
    const from = useLocation().state?.from?.pathname || "/";

    const submit = async (e) => {
        e.preventDefault();
        setErr("");
        try { await login(userOrEmail, password); nav(from, { replace: true }); }
        catch (ex) { setErr(ex.message || "Login failed"); }
    };

    return (
        <div className="w-full max-w-lg sm:max-w-xl bg-white rounded-3xl shadow-[0_16px_40px_-12px_rgba(31,31,77,.18)] p-6 sm:p-8">
            <div className="flex justify-center mb-6">
                <BrandLogo className="h-22" />
            </div>

            <h1 className="text-center font-display text-3xl sm:text-4xl font-semibold text-ras-blue-800 leading-tight mb-8">
                <span className="text-ras-blue-900">RAS Student</span>
            </h1>

            <form onSubmit={submit} className="space-y-4">
                {err && (
                    <div className="text-sm text-ras-coral-700 bg-ras-coral-50 border border-ras-coral-200 rounded-xl p-2.5">
                        {err}
                    </div>
                )}

                <label className="block">
                    <span className="text-sm text-ras-blue-800">Username or Email</span>
                    <input
                        className="mt-1 w-full h-12 rounded-xl border border-ras-blue-200 px-4 outline-none focus:ring-2 focus:ring-ras-blue-400 focus:border-ras-blue-400 text-[15px]"
                        placeholder="you@example.com"
                        value={userOrEmail}
                        onChange={(e) => setUserOrEmail(e.target.value)}
                        autoComplete="username"
                    />
                </label>

                <label className="block">
                    <span className="text-sm text-ras-blue-800">Password</span>
                    <div className="mt-1 relative">
                        <input
                            type={showPwd ? "text" : "password"}
                            className="w-full h-12 rounded-xl border border-ras-blue-200 px-4 pr-12 outline-none focus:ring-2 focus:ring-ras-blue-400 focus:border-ras-blue-400 text-[15px]"
                            placeholder="••••••••"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            autoComplete="current-password"
                        />
                        <button
                            type="button"
                            onClick={() => setShowPwd(v => !v)}
                            aria-label={showPwd ? "Hide password" : "Show password"}
                            className="absolute inset-y-0 right-2 grid place-items-center px-2 text-ras-blue-600 hover:text-ras-blue-800"
                        >
                            {showPwd ? (
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                    <path strokeWidth="1.8" d="M3 3l18 18M10.6 10.6A3 3 0 0012 15a3 3 0 002.4-4.4M6.2 6.2C4.1 7.5 3 9.2 3 12c0 0 3.5 7 9 7 1.2 0 2.3-.2 3.3-.6M21 12s-4-7-9-7c-.9 0-1.8.1-2.7.4" />
                                </svg>
                            ) : (
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                    <path strokeWidth="1.8" d="M1 12s4-7 11-7 11 7 11 7-4 7-11 7S1 12 1 12z" />
                                    <circle cx="12" cy="12" r="3" strokeWidth="1.8" />
                                </svg>
                            )}
                        </button>
                    </div>
                </label>

                <button type="submit" className="w-full h-12 rounded-xl bg-ras-blue-800 text-white font-medium hover:bg-ras-blue-700 focus:ring-2 focus:ring-ras-blue-400 transition">
                    Log in
                </button>
            </form>

            <div className="mt-5 text-center">
                <Link to="/forgot" className="text-sm text-ras-coral-600 hover:text-ras-coral-700 hover:underline">
                    Forgot username / password?
                </Link>
            </div>

            <div className="mt-3 text-center text-sm text-ras-blue-800/80">
                Not a RAS Student? <a href="#" className="text-ras-blue-800 hover:underline">Learn more</a>
            </div>

            <div className="mt-8 text-center text-[11px] text-ras-blue-800/70 space-x-3">
                <a href="#" className="hover:underline">Privacy Policy</a><span></span>
                <a href="#" className="hover:underline">Cookie Policy</a><span></span>
                <a href="#" className="hover:underline">Terms of Use</a>
                <div className="mt-1"> {new Date().getFullYear()} RAS Music &amp; Art Studio</div>
            </div>
        </div>
    );
}
