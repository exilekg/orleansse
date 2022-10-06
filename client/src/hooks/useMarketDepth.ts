import React from "react"
import { api } from "../appStore/api"
import { useAppDispatch, useAppSelector } from "../appStore/hooks"
import { AppDispatch, RootState } from "../appStore/store"
import { setMarketDepth } from "../appStore/rootReducer"

const loadMarketDepthThunk = (securityId: string) => 
    async (dispatch: AppDispatch, getState: () => RootState) => {
        const marketDepthItems = await api.securities.getDepth(securityId);
        if (marketDepthItems) {
            dispatch(setMarketDepth({
                securityId,
                depth: marketDepthItems,
            }))
        }
    }

export const useMarketDepth = (securityId: string) => {
    const dispatch = useAppDispatch();
    const marketDepth = useAppSelector(state => state.depths[securityId]);
    const loadMarketDepth = React.useCallback(() => {
            dispatch(loadMarketDepthThunk(securityId))
    }, [dispatch, securityId])

    return {
        marketDepth,
        loadMarketDepth
    }
}