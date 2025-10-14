import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "./AuthContext";

export default function RoleRoute({ roles = [] }) {
    const { user } = useAuth();  // user = decoded JWT
    const loc = useLocation();
    const claim = (user?.role || user?.roles || user?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]);
    const userRoles = Array.isArray(claim) ? claim : (claim ? [claim] : []);
    const ok = user && (roles.length ? roles.some(r => userRoles.includes(r)) : true);
    return ok ? <Outlet /> : <Navigate to="/login" replace state={{ from: loc }} />;
}
