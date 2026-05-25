import { currency, percent } from '../portfolio';

export default function SummaryCards({ totals }) {
    const { invested, marketValue, pnl, pnlPct, positions } = totals;
    const pnlClass = pnl >= 0 ? 'pnl-positive' : 'pnl-negative';

    return (
        <div className="cards">
            <div className="card">
                <span className="card-label">Total Market Value</span>
                <span className="card-value">{currency.format(marketValue)}</span>
            </div>
            <div className="card">
                <span className="card-label">Total Invested</span>
                <span className="card-value">{currency.format(invested)}</span>
            </div>
            <div className="card">
                <span className="card-label">Unrealized P&amp;L</span>
                <span className={`card-value ${pnlClass}`}>
                    {currency.format(pnl)}
                    <span className="card-sub">
                        {pnl >= 0 ? '+' : ''}
                        {percent.format(pnlPct)}
                    </span>
                </span>
            </div>
            <div className="card">
                <span className="card-label">Positions</span>
                <span className="card-value">{positions}</span>
            </div>
        </div>
    );
}
