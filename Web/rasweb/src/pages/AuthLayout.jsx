import { Outlet } from "react-router-dom";

export default function AuthLayout() {
    return (
        <main className="grid md:grid-cols-2 min-h-screen">
            <section className="hidden md:block">
                <img
                    src="/assets/login-hero.jpg"
                    alt="RAS campus"
                    className="w-full h-full object-cover"
                />
            </section>
            <section className="bg-gradient-to-b from-ras-blue-50 to-white flex items-center justify-center p-4 sm:p-6 lg:p-8">
                <Outlet />
            </section>
        </main>
    );
}