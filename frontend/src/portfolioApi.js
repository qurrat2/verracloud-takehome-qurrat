import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const portfolioApi = createApi({
    reducerPath: 'portfolioApi',
    baseQuery: fetchBaseQuery({ baseUrl: 'http://localhost:5282/api/' }),
    tagTypes: ['Holdings'],
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
            invalidatesTags: ['Holdings'],
        }),
        deleteHolding: builder.mutation({
            query: (id) => ({
                url: `holdings/${id}`,
                method: 'DELETE',
            }),
            invalidatesTags: ['Holdings'],
        }),
    }),
});

export const {
    useListHoldingsQuery,
    useAddHoldingMutation,
    useDeleteHoldingMutation,
} = portfolioApi;
