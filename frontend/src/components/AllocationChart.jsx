import { PieChart, Pie, Cell, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { currency, SLICE_COLORS } from '../portfolio';

export default function AllocationChart({ allocation }) {
    if (allocation.length === 0) {
        return <p className="chart-empty">No holdings to allocate yet.</p>;
    }

    return (
        <ResponsiveContainer width="100%" height={260}>
            <PieChart>
                <Pie
                    data={allocation}
                    dataKey="value"
                    nameKey="name"
                    innerRadius={60}
                    outerRadius={95}
                    paddingAngle={2}
                >
                    {allocation.map((slice, i) => (
                        <Cell key={slice.name} fill={SLICE_COLORS[i % SLICE_COLORS.length]} />
                    ))}
                </Pie>
                <Tooltip formatter={(value) => currency.format(value)} />
                <Legend verticalAlign="bottom" height={36} />
            </PieChart>
        </ResponsiveContainer>
    );
}
