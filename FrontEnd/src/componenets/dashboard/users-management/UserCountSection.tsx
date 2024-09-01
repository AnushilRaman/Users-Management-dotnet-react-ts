import UserCountCard from "./UserCountCard";
import { IAuthUser, RolesEnum } from "../../../types/auth.types";
import { FaUser, FaUsersCog, FaUserShield, FaUserTie } from "react-icons/fa";


interface IPoprs {
    usersList: IAuthUser[];
}

const UserCountSection = ({ usersList }: IPoprs) => {
    let owners = 0;
    let admins = 0;
    let managers = 0;
    let users = 0;

    usersList.forEach((item) => {
        if (item.roles.includes(RolesEnum.OWNER)) owners++;
        else if (item.roles.includes(RolesEnum.ADMIN)) admins++;
        else if (item.roles.includes(RolesEnum.MANAGER)) managers++;
        else if (item.roles.includes(RolesEnum.USER)) users++;
    })

    const userCounterData = [
        { count: owners, role: RolesEnum.OWNER, icon: FaUsersCog, color: '#3b3549' },
        { count: admins, role: RolesEnum.ADMIN, icon: FaUserShield, color: '#9333EA' },
        { count: managers, role: RolesEnum.MANAGER, icon: FaUserTie, color: '#0B96BC' },
        { count: users, role: RolesEnum.USER, icon: FaUser, color: '#FEC223' },
    ]

    return (
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-x-4">
            {userCounterData.map((item, index) => (
                <UserCountCard key={index} count={item.count} color={item.color} role={item.role} icon={item.icon} />
            ))}
        </div>
    )
}

export default UserCountSection
