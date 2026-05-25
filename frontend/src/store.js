import { configureStore } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query';
import { portfolioApi } from './portfolioApi';

export const store = configureStore({
    reducer: {
        [portfolioApi.reducerPath]: portfolioApi.reducer,
    },
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware().concat(portfolioApi.middleware),
});

// Enables refetchOnFocus / refetchOnReconnect on the hooks that opt in.
setupListeners(store.dispatch);
