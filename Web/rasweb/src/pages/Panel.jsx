import { useNavigate } from "react-router-dom";

export default function Panel() {
    const nav = useNavigate();
    return (
        <div className="max-w-lg mx-auto mt-16 space-y-6 text-center">
            <h1 className="text-2xl font-semibold">Welcome to RAS Student</h1>
            <p className="text-slate-600">Choose where you want to go:</p>
            <div className="flex items-center justify-center gap-4">
                <button className="px-4 py-2 border rounded" onClick={() => nav("/dashboard")}>
                    Go to Dashboard
                </button>
                <button className="px-4 py-2 border rounded" onClick={() => nav("/students")}>
                    Go to Students
                </button>
            </div>
        </div>
    );
}
