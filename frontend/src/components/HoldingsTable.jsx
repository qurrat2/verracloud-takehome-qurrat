import { useState } from 'react';
import { useListHoldingsQuery, useDeleteHoldingMutation, useListPricesQuery } from '../portfolioApi';
import { currency } from '../portfolio';

const PAGE_SIZE = 5;

export default function HoldingsTable() {
    const {
        data: holdings = [],
        isLoading,
        isError,
        error,
        fulfilledTimeStamp,
    } = useListHoldingsQuery(undefined, { pollingInterval: 5000 });
    const { data: tickers = [] } = useListPricesQuery();
    const [deleteHolding, { isLoading: isDeleting }] = useDeleteHoldingMutation();

    const [filter, setFilter] = useState('');
    const [page, setPage] = useState(0);

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

    const filtered = filter ? holdings.filter((h) => h.ticker === filter) : holdings;
    const totalPages = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE));
    const safePage = Math.min(page, totalPages - 1);
    const pageItems = filtered.slice(safePage * PAGE_SIZE, safePage * PAGE_SIZE + PAGE_SIZE);

    const onFilterChange = (e) => {
        setFilter(e.target.value);
        setPage(0);
    };

    return (
        <div>
            <div className="table-toolbar">
                <label className="filter">
                    Filter
                    <select value={filter} onChange={onFilterChange}>
                        <option value="">All tickers</option>
                        {tickers.map((t) => (
                            <option key={t.tickerId} value={t.ticker}>
                                {t.ticker}
                            </option>
                        ))}
                    </select>
                </label>
            </div>

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
                    {pageItems.map((h) => {
                        const noPrice = h.currentPrice === 0;
                        return (
                            <tr key={h.id}>
                                <td className="ticker-cell">{h.ticker}</td>
                                <td className="num">{h.quantity}</td>
                                <td className="num">{currency.format(h.purchasePrice)}</td>
                                <td className="num">
                                    {noPrice ? <span className="muted">Not priced yet</span> : currency.format(h.currentPrice)}
                                </td>
                                <td className="num">{noPrice ? 'N/A' : currency.format(h.marketValue)}</td>
                                <td className={noPrice ? 'num muted' : `num ${h.unrealizedPnL >= 0 ? 'pnl-positive' : 'pnl-negative'}`}>
                                    {noPrice ? 'N/A' : currency.format(h.unrealizedPnL)}
                                </td>
                                <td>
                                    <button className="link-danger" onClick={() => onDelete(h)} disabled={isDeleting}>
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        );
                    })}
                </tbody>
            </table>

            <div className="pagination">
                <button className="btn-secondary" onClick={() => setPage(safePage - 1)} disabled={safePage === 0}>
                    Prev
                </button>
                <span>
                    Page {safePage + 1} of {totalPages} ({filtered.length} holding{filtered.length === 1 ? '' : 's'})
                </span>
                <button className="btn-secondary" onClick={() => setPage(safePage + 1)} disabled={safePage >= totalPages - 1}>
                    Next
                </button>
            </div>

            {fulfilledTimeStamp && (
                <p className="last-updated">
                    Last updated: {new Date(fulfilledTimeStamp).toLocaleTimeString()}
                </p>
            )}
        </div>
    );
}
