import React from "react";
import ReactDOM from "react-dom/client";
import "./index.css";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import { AuthProvider } from "./auth/AuthContext";
import ProtectedRoute from "./auth/ProtectedRoute";
import AppLayout from "./ui/AppLayout";

// 1. Import your new AuthLayout component
import AuthLayout from "./pages/AuthLayout";

import Login from "./pages/Login";
import Forgot from "./pages/Forgot";
import Logout from "./pages/Logout";
import ResetPassword from "./pages/ResetPassword";
import Dashboard from "./pages/Dashboard";

// 2. Update the router configuration
const router = createBrowserRouter([
    {
        element: <AuthLayout />,
        children: [
            // These routes will now render inside the AuthLayout's <Outlet />
            { path: "/login", element: <Login /> },
            { path: "/forgot", element: <Forgot /> },
            { path: "/reset-password", element: <ResetPassword /> },
        ]
    },
    {
        path: "/logout",
        element: <Logout />,
    },
    {
        element: <ProtectedRoute />,
        children: [{
            element: <AppLayout />,
            children: [
                { path: "/", element: <Dashboard /> },
            ]
        }]
    }
]);

ReactDOM.createRoot(document.getElementById("root")).render(
    <React.StrictMode>
        <AuthProvider><RouterProvider router={router} /></AuthProvider>
    </React.StrictMode>
);