import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    Legend,
    ResponsiveContainer,
} from 'recharts';
import { useListPriceHistoryQuery } from '../portfolioApi';
import { currency, SLICE_COLORS } from '../portfolio';

function pivot(series) {
    const byTime = {};
    for (const s of series) {
        for (const p of s.points) {
            const row = (byTime[p.asOf] ??= {
                asOf: p.asOf,
                time: new Date(p.asOf).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
            });
            row[s.ticker] = p.value;
        }
    }
    return Object.values(byTime).sort((a, b) => new Date(a.asOf) - new Date(b.asOf));
}

export default function PriceTrendChart() {
    const { data: series = [], isLoading, isError } = useListPriceHistoryQuery(undefined, {
        pollingInterval: 10000,
    });

    if (isLoading) return <p className="chart-empty">Loading price history...</p>;
    if (isError) return <p className="error">Failed to load price history.</p>;
    if (series.length === 0) {
        return <p className="chart-empty">Add a holding to see its price trend.</p>;
    }

    const data = pivot(series);

    return (
        <ResponsiveContainer width="100%" height={260}>
            <LineChart data={data} margin={{ top: 8, right: 16, bottom: 0, left: 8 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                <XAxis dataKey="time" tick={{ fontSize: 12 }} minTickGap={24} />
                <YAxis tick={{ fontSize: 12 }} width={70} tickFormatter={(v) => currency.format(v)} />
                <Tooltip formatter={(value) => currency.format(value)} />
                <Legend />
                {series.map((s, i) => (
                    <Line
                        key={s.ticker}
                        type="monotone"
                        dataKey={s.ticker}
                        stroke={SLICE_COLORS[i % SLICE_COLORS.length]}
                        strokeWidth={2}
                        dot={false}
                        isAnimationActive={false}
                    />
                ))}
            </LineChart>
        </ResponsiveContainer>
    );
}
