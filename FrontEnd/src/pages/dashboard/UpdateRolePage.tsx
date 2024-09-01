import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "react-hot-toast";
import Spinner from "../../componenets/general/Spinner";
import { IAuthUser, IUpdateRoleDto } from "../../types/auth.types";
import axiosInstance from "../../utils/axiosInstance";
import { UPDATE_ROLE_URL, USERS_LIST_URL } from "../../utils/globalConfig";
import useAuth from "../../hooks/useAuth.hook";
import { allowedRolesForUpdateArray, isAuthorizedForUpdateRole } from "../../auth/auth.utils";
import Button from "../../componenets/general/Button";
import { PATH_DASHBOARD } from "../../routes/paths";


const UpdateRolePage = () => {
  const { user: loggedInUser }: any = useAuth();
  const { userName } = useParams();
  const [user, setUser] = useState<IAuthUser>();
  const [role, setRole] = useState<string>();
  const [loading, setLoading] = useState<boolean>(false);
  const [postLoading, setPostLoading] = useState<boolean>(false);
  const navigate = useNavigate();

  const getUserByUserName = async () => {
    try {
      setLoading(true);
      const response = await axiosInstance.get<IAuthUser>(`${USERS_LIST_URL}/${userName}`);
      const { data }: any = response;
      if (!isAuthorizedForUpdateRole(loggedInUser?.roles[0], data?.roles[0])) {
        setLoading(false);
        toast.error("You are not allowed to change the role of this user");
        navigate(PATH_DASHBOARD.dashboard);
      }
      else {
        setUser(data);
        setRole(data?.roles[0]);
        setLoading(false);
      }
    }
    catch (error) {
      setLoading(false);
      const err = error as { data: string, status: number }
      const { status } = err;
      if (status === 404) {
        toast.error("User name not found!!!!!!");
      }
      else toast.error("An Error happened. Please Contact admins");
    }
  }

  const UpdateRole = async () => {
    try {
      if (!role || !userName) return;
      setPostLoading(true);
      const updateData: IUpdateRoleDto = {
        newRole: role,
        userName,
      }
      await axiosInstance.post(UPDATE_ROLE_URL, updateData);
      toast.success("Role updated Successfully");
      navigate(PATH_DASHBOARD.dashboard);
    }
    catch (error) {
      setLoading(false);
      const err = error as { data: string, status: number }
      const { status } = err;
      if (status === 404) {
        toast.error("You are not allowed to change role of this user");
      }
      else toast.error("An Error happened. Please Contact admins");
      navigate(PATH_DASHBOARD.dashboard);
    }
  }

  useEffect(() => {
    getUserByUserName();
  }, [])

  if (loading) {
    return <div className="w-full">
      <Spinner />
    </div>
  }

  return (
    <div className="p-4 w-2/4 mx-auto flex flex-col gap-4">
      <div className="bg-white p-2 rounded-md flex flex-col gap-4">
        <h1 className="text-2xl font-bold">Update Role</h1>
        <div className="border border-dashed border-purple-300 rounded-md">
          <h4 className="text-xl">
            UserName:
            <span className="text-2xl font-bold ml-2 px-2 py-1 text-purple-600 rounded-md">{userName}</span>
          </h4>
          <h4 className="text-xl">
            Current role:
            <span className="text-2xl font-bold ml-2 px-2 py-1 text-purple-600 rounded-md">{user?.roles[0]}</span>
          </h4>
          <h4 className="text-xl">
            New Role :
          </h4>
          <select className="w-80" value={role} onChange={(evt) => setRole(evt.target.value)}>
            {allowedRolesForUpdateArray(loggedInUser).map((item) => (
              <option key={item} value={item}>{item}</option>
            ))}
          </select>
          <div className="grid grid-cols-2 gap-4 mt-12">
            <Button
              label="Cancel"
              onClick={() => navigate(PATH_DASHBOARD.dashboard)}
              type="button"
              variant="secondary"
            />
            <Button
              label="Update"
              onClick={() => UpdateRole()}
              type="button"
              variant="primary"
              loading={postLoading}
            />
          </div>
        </div>
      </div>
    </div>
  )
}

export default UpdateRolePage
