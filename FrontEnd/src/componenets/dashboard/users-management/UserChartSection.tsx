import { IAuthUser, RolesEnum } from "../../../types/auth.types";
import { Chart as ChartJs, CategoryScale, LinearScale, PointElement, LineElement, Tooltip, Legend } from 'chart.js';
import { Line } from "react-chartjs-2";

ChartJs.register(CategoryScale, LinearScale, PointElement, LineElement, Tooltip, Legend);

interface IProps {
    usersList: IAuthUser[],
}

const UserChartSection = ({ usersList }: IProps) => {
    const chartLabels = [RolesEnum.OWNER, RolesEnum.ADMIN, RolesEnum.MANAGER, RolesEnum.USER];
    const chartValues = [];

    const ownersCount = usersList.filter((x) => x.roles.includes(RolesEnum.OWNER)).length;
    chartValues.push(ownersCount);

    const adminsCount = usersList.filter((x) => x.roles.includes(RolesEnum.ADMIN)).length;
    chartValues.push(adminsCount);

    const managersCount = usersList.filter((x) => x.roles.includes(RolesEnum.MANAGER)).length;
    chartValues.push(managersCount);

    const usersCount = usersList.filter((x) => x.roles.includes(RolesEnum.USER)).length;
    chartValues.push(usersCount);

    const chartOptions = {
        respnsive: true,
        scales: {
            x: {
                grid: { display: false },
            },
            y: {
                ticks: { stepSize: 5 },
            }
        }
    }

    const chartData = {
        labels: chartLabels,
        datasets: [
            {
                label: 'count',
                data: chartValues,
                borderColor: '#7564eb475',
                pointBorderColor: '#754eb4',
                tension: 0.25,
            },
        ],
    }

    return (
        <div className="col-span-1 lg:col-span-3 bg-white p-2 rounded-md">
            <h1 className="text-xl font-bold mb-2">Users Chart</h1>
            <Line options={chartOptions} data={chartData} className="bg-white p-2 rounded-md" />
        </div>
    )
}

export default UserChartSection
