import { useListHoldingsQuery, useDeleteHoldingMutation } from '../portfolioApi';

const currency = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' });

export default function HoldingsTable() {
    const {
        data: holdings = [],
        isLoading,
        isError,
        error,
        fulfilledTimeStamp,
    } = useListHoldingsQuery(undefined, { pollingInterval: 5000 });

    const [deleteHolding, { isLoading: isDeleting }] = useDeleteHoldingMutation();

    const onDelete = (h) => {
        if (window.confirm(`Delete your ${h.ticker} holding (${h.quantity} @ ${currency.format(h.purchasePrice)})?`)) {
            deleteHolding(h.id);
        }
    };

    if (isLoading) return <p>Loading holdings...</p>;
    if (isError) {
        const message = error?.data?.detail ?? error?.error ?? 'Unknown error';
        return <p className="error">Failed to load holdings: {message}</p>;
    }
    if (holdings.length === 0) return <p>No holdings yet. Add one to get started.</p>;

    return (
        <div>
            <table className="holdings-table">
                <thead>
                    <tr>
                        <th>Ticker</th>
                        <th>Quantity</th>
                        <th>Purchase Price</th>
                        <th>Current Price</th>
                        <th>Market Value</th>
                        <th>P&amp;L</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {holdings.map((h) => (
                        <tr key={h.id}>
                            <td>{h.ticker}</td>
                            <td>{h.quantity}</td>
                            <td>{currency.format(h.purchasePrice)}</td>
                            <td>{currency.format(h.currentPrice)}</td>
                            <td>{currency.format(h.marketValue)}</td>
                            <td className={h.unrealizedPnL >= 0 ? 'pnl-positive' : 'pnl-negative'}>
                                {currency.format(h.unrealizedPnL)}
                            </td>
                            <td>
                                <button onClick={() => onDelete(h)} disabled={isDeleting}>
                                    Delete
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            {fulfilledTimeStamp && (
                <p className="last-updated">
                    Last updated: {new Date(fulfilledTimeStamp).toLocaleTimeString()}
                </p>
            )}
        </div>
    );
}