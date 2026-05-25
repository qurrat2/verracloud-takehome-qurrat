import { useListHoldingsQuery, useDeleteHoldingMutation } from '../portfolioApi';
import { currency } from '../portfolio';

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
            <table className="data-table">
                <thead>
                    <tr>
                        <th>Ticker</th>
                        <th className="num">Quantity</th>
                        <th className="num">Purchase Price</th>
                        <th className="num">Current Price</th>
                        <th className="num">Market Value</th>
                        <th className="num">P&amp;L</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {holdings.map((h) => (
                        <tr key={h.id}>
                            <td className="ticker-cell">{h.ticker}</td>
                            <td className="num">{h.quantity}</td>
                            <td className="num">{currency.format(h.purchasePrice)}</td>
                            <td className="num">{currency.format(h.currentPrice)}</td>
                            <td className="num">{currency.format(h.marketValue)}</td>
                            <td className={`num ${h.unrealizedPnL >= 0 ? 'pnl-positive' : 'pnl-negative'}`}>
                                {currency.format(h.unrealizedPnL)}
                            </td>
                            <td>
                                <button className="link-danger" onClick={() => onDelete(h)} disabled={isDeleting}>
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