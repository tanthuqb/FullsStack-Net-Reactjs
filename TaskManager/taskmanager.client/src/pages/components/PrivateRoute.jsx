import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "../../hooks/useAuth";

const PrivateRoute = () => {
  const user = useAuth();
  const location = useLocation();

  if (user === undefined) return <div>Loading...</div>;
  if (!user || user.user === null) return <Navigate to="/login" state={{ from: location }} replace />;

  return <Outlet />;
};

export default PrivateRoute;
