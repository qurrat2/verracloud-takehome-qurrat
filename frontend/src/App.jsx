import HoldingsTable from './components/HoldingsTable';
import './App.css';

export default function App() {
    return (
        <div className="app">
            <header className="app-header">
                <h1>Portfolio Dashboard</h1>
            </header>
            <main>
                <section>
                    <h2>Holdings</h2>
                    <HoldingsTable />
                </section>
            </main>
        </div>
    );
}
