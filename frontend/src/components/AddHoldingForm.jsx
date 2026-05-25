import { useState } from 'react';
import { useAddHoldingMutation, useListHoldingsQuery, useListPricesQuery } from '../portfolioApi';

const initialForm = { tickerCode: '', quantity: '', purchasePrice: '' };
const DUPLICATE_WINDOW_MS = 2 * 60 * 1000;

export default function AddHoldingForm() {
    const [form, setForm] = useState(initialForm);
    const [addHolding, { isLoading, error, reset }] = useAddHoldingMutation();
    const { data: holdings = [] } = useListHoldingsQuery();
    const { data: tickers = [] } = useListPricesQuery();

    const onChange = (e) => {
        setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
        if (error) reset();
    };

    const onSubmit = async (e) => {
        e.preventDefault();

        const tickerCode = form.tickerCode.trim().toUpperCase();
        const quantity = Number(form.quantity);
        const purchasePrice = Number(form.purchasePrice);

        const recentDuplicate = holdings.some(
            (h) =>
                h.ticker === tickerCode &&
                h.quantity === quantity &&
                h.purchasePrice === purchasePrice &&
                Date.now() - new Date(h.createdAt).getTime() <= DUPLICATE_WINDOW_MS
        );

        const message = recentDuplicate
            ? `You added an identical ${tickerCode} holding less than 2 minutes ago. Add it again?`
            : `Add ${quantity} ${tickerCode} @ $${purchasePrice}?`;

        if (!window.confirm(message)) return;

        try {
            await addHolding({ tickerCode, quantity, purchasePrice }).unwrap();
            setForm(initialForm);
        } catch {
            // error already exposed via the `error` state from the hook
        }
    };

    const errorMessage = error?.data?.detail ?? error?.error;

    return (
        <form className="add-holding-form" onSubmit={onSubmit}>
            <div className="form-row">
                <label>
                    Ticker
                    <select name="tickerCode" value={form.tickerCode} onChange={onChange} required>
                        <option value="" disabled>
                            Select a ticker
                        </option>
                        {tickers.map((t) => (
                            <option key={t.tickerId} value={t.ticker}>
                                {t.ticker} - {t.name}
                            </option>
                        ))}
                    </select>
                </label>
                <label>
                    Quantity
                    <input
                        name="quantity"
                        type="number"
                        value={form.quantity}
                        onChange={onChange}
                        min="0.0001"
                        step="0.0001"
                        required
                    />
                </label>
                <label>
                    Purchase Price
                    <input
                        name="purchasePrice"
                        type="number"
                        value={form.purchasePrice}
                        onChange={onChange}
                        min="0"
                        step="0.01"
                        required
                    />
                </label>
                <button type="submit" disabled={isLoading}>
                    {isLoading ? 'Adding...' : 'Add Holding'}
                </button>
            </div>
            {errorMessage && <p className="error">{errorMessage}</p>}
        </form>
    );
}
