import { Link, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function AppLayout() {
    const { user, logout } = useAuth();
    const nav = useNavigate();

    const onLogout = async () => {
        await logout();
        nav("/login", { replace: true });
    };

    return (
        <div className="min-h-screen grid grid-rows-[auto_1fr]">
            <header className="p-4 shadow flex gap-4 items-center">
                <Link to="/" className="font-bold">RAS</Link>
                {/*<Link to="/students">Students</Link>*/}
                <Link to="/">Dashboard</Link>
                <div className="ml-auto flex items-center gap-3">
                    {user && <span className="text-sm opacity-70">{user.email || user.name}</span>}
                    <button className="px-3 py-1 border rounded" onClick={onLogout}>Logout</button>
                </div>
            </header>
            <main className="p-6"><Outlet /></main>
        </div>
    );
}
