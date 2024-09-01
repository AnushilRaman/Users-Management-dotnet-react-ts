import useAuth from "../../hooks/useAuth.hook";
import Header from "./Header";
import { Outlet, useLocation } from "react-router-dom";
import SideBar from "./SideBar";

const Layout = () => {
    const { isAuthenticated } = useAuth();
    const { pathname } = useLocation();

    const sideBarRenderer = () => {
        if (isAuthenticated && pathname.toLowerCase().startsWith('/dashboard')) {
            return <SideBar />
        }
        return null;
    }

    return (
        <div>
            <Header />
            {/* Using Outlet, we render all routes that are inside of this Layout  */}
            <div className="flex">
                {sideBarRenderer()}
                <Outlet />
            </div>
        </div>
    )
}

export default Layout
