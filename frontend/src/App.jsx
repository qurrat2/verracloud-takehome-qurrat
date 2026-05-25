import { useListHoldingsQuery, useResetSeedMutation } from './portfolioApi';
import { computeTotals } from './portfolio';
import SummaryCards from './components/SummaryCards';
import AllocationChart from './components/AllocationChart';
import PriceTrendChart from './components/PriceTrendChart';
import PricesTable from './components/PricesTable';
import AddHoldingForm from './components/AddHoldingForm';
import HoldingsTable from './components/HoldingsTable';
import './App.css';

export default function App() {
    const {
        data: holdings = [],
        isLoading,
        isError,
        error,
    } = useListHoldingsQuery(undefined, { pollingInterval: 5000 });
    const [resetSeed, { isLoading: isResetting }] = useResetSeedMutation();

    const totals = computeTotals(holdings);

    // Only treat it as an error state when we have no data to fall back on;
    // a failed background poll should keep showing the last good values.
    const showError = isError && holdings.length === 0;
    const errorMessage = error?.data?.detail ?? error?.error ?? 'Unknown error';

    const onReset = async () => {
        if (window.confirm('Reset all data to the seed defaults? This removes any holdings you added.')) {
            await resetSeed();
        }
    };

    return (
        <div className="app">
            <header className="app-header">
                <h1>Portfolio Holdings Dashboard</h1>
                <div className="header-actions">
                    <button onClick={() => window.location.reload()}>Refresh</button>
                    <button className="btn-secondary" onClick={onReset} disabled={isResetting}>
                        {isResetting ? 'Resetting...' : 'Reset to seed data'}
                    </button>
                </div>
            </header>

            {isLoading ? (
                <p className="chart-empty">Loading portfolio...</p>
            ) : showError ? (
                <p className="error">Failed to load portfolio: {errorMessage}</p>
            ) : (
                <SummaryCards totals={totals} />
            )}

            <div className="grid-2">
                <section className="panel">
                    <h2>Allocation</h2>
                    {isLoading ? (
                        <p className="chart-empty">Loading allocation...</p>
                    ) : showError ? (
                        <p className="error">Failed to load allocation: {errorMessage}</p>
                    ) : (
                        <AllocationChart allocation={totals.allocation} />
                    )}
                </section>
                <section className="panel">
                    <h2>Price Trends (your holdings)</h2>
                    <PriceTrendChart />
                </section>
            </div>

            <section className="panel">
                <h2>Market Prices</h2>
                <PricesTable />
            </section>

            <section className="panel">
                <h2>Add Holding</h2>
                <AddHoldingForm />
            </section>

            <section className="panel">
                <h2>Holdings</h2>
                <HoldingsTable />
            </section>
        </div>
    );
}
