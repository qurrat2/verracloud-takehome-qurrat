import { useListPricesQuery, useRefreshPricesMutation } from '../portfolioApi';
import { currency } from '../portfolio';

export default function PricesTable() {
    const { data: prices = [], isLoading, isError, error } = useListPricesQuery(undefined, {
        pollingInterval: 5000,
    });
    const [refreshPrices, { isLoading: isRefreshing }] = useRefreshPricesMutation();

    if (isLoading) return <p>Loading prices...</p>;
    if (isError) {
        const message = error?.data?.detail ?? error?.error ?? 'Unknown error';
        return <p className="error">Failed to load prices: {message}</p>;
    }

    return (
        <div>
            <div className="panel-actions">
                <button onClick={() => refreshPrices()} disabled={isRefreshing}>
                    {isRefreshing ? 'Refreshing...' : 'Refresh prices'}
                </button>
            </div>
            <table className="data-table">
                <thead>
                    <tr>
                        <th>Ticker</th>
                        <th>Name</th>
                        <th className="num">Current Price</th>
                        <th>Last Updated</th>
                    </tr>
                </thead>
                <tbody>
                    {prices.map((p) => (
                        <tr key={p.ticker}>
                            <td className="ticker-cell">{p.ticker}</td>
                            <td>{p.name}</td>
                            <td className="num">{currency.format(p.currentPrice)}</td>
                            <td>{new Date(p.lastUpdatedAt).toLocaleTimeString()}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
