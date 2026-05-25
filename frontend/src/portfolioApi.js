import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const portfolioApi = createApi({
    reducerPath: 'portfolioApi',
    baseQuery: fetchBaseQuery({
        baseUrl: import.meta.env.VITE_API_URL ?? 'http://localhost:5282/api/',
    }),
    tagTypes: ['Holdings', 'Prices'],
    endpoints: (builder) => ({
        listHoldings: builder.query({
            query: () => 'holdings',
            providesTags: ['Holdings'],
        }),
        addHolding: builder.mutation({
            query: (body) => ({
                url: 'holdings',
                method: 'POST',
                body,
            }),
            // A new holding can add a ticker to the trend chart, so refresh prices too.
            invalidatesTags: ['Holdings', 'Prices'],
        }),
        deleteHolding: builder.mutation({
            query: (id) => ({
                url: `holdings/${id}`,
                method: 'DELETE',
            }),
            invalidatesTags: ['Holdings', 'Prices'],
        }),
        listPrices: builder.query({
            query: () => 'prices',
            providesTags: ['Prices'],
        }),
        listPriceHistory: builder.query({
            query: () => 'prices/history',
            providesTags: ['Prices'],
        }),
        refreshPrices: builder.mutation({
            query: () => ({
                url: 'prices/refresh',
                method: 'POST',
            }),
            // New prices change holding market values, so refresh both.
            invalidatesTags: ['Prices', 'Holdings'],
        }),
        resetSeed: builder.mutation({
            query: () => ({
                url: 'seed/reset',
                method: 'POST',
            }),
            invalidatesTags: ['Prices', 'Holdings'],
        }),
    }),
});

export const {
    useListHoldingsQuery,
    useAddHoldingMutation,
    useDeleteHoldingMutation,
    useListPricesQuery,
    useListPriceHistoryQuery,
    useRefreshPricesMutation,
    useResetSeedMutation,
} = portfolioApi;
