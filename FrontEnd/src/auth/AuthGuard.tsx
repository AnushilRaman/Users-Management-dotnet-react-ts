import useAuth from '../hooks/useAuth.hook'
import AuthSpinner from '../componenets/general/authSpinner';
import { Navigate, Outlet } from 'react-router-dom';
import { PATH_PUBLIC } from '../routes/paths';


interface IProps {
    roles: string[]
}
const AuthGuard = ({ roles }: IProps) => {
    const { isAuthenticated, user, isAuthLoading } = useAuth();
    const hasAccess = isAuthenticated && user?.roles?.find((x) => roles.includes(x));
    if (isAuthLoading) {
        return <AuthSpinner />;
    }
    return hasAccess ? <Outlet /> : <Navigate to={PATH_PUBLIC.unauthorized} />;
}
export default AuthGuard
