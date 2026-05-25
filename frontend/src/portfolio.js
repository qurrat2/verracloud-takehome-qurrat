export const currency = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
});

export const percent = new Intl.NumberFormat('en-US', {
    style: 'percent',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
});

// Color palette for allocation slices, indexed by position.
export const SLICE_COLORS = [
    '#3b82f6',
    '#10b981',
    '#f59e0b',
    '#8b5cf6',
    '#ef4444',
    '#06b6d4',
    '#ec4899',
    '#84cc16',
];

export function computeTotals(holdings) {
    let invested = 0;
    let marketValue = 0;

    const byTicker = new Map();

    for (const h of holdings) {
        invested += h.quantity * h.purchasePrice;
        marketValue += h.marketValue;
        byTicker.set(h.ticker, (byTicker.get(h.ticker) ?? 0) + h.marketValue);
    }

    const pnl = marketValue - invested;
    const pnlPct = invested > 0 ? pnl / invested : 0;

    const allocation = [...byTicker.entries()]
        .map(([name, value]) => ({ name, value }))
        .sort((a, b) => b.value - a.value);

    return {
        invested,
        marketValue,
        pnl,
        pnlPct,
        positions: holdings.length,
        allocation,
    };
}
