import React from "react"
import { api } from "../appStore/api"
import { useAppDispatch, useAppSelector } from "../appStore/hooks"
import { AppDispatch, RootState } from "../appStore/store"
import { setMainBoardItems, updateMainBoardItem } from "../appStore/rootReducer"
import { IMainBoardItem } from "../appStore/model"

const loadMainBoardThunk = () => 
    async (dispatch: AppDispatch, getState: () => RootState) => {
        const { usename } = getState();
        if (!usename) return;

        const mainBoard = await api.users.getMainBoard(usename);
        if (mainBoard) {
            dispatch(setMainBoardItems(mainBoard))
        }
    }

export const useMainBoard = () => {
    const dispatch = useAppDispatch();
    const mainBoard = useAppSelector(state => state.mainBoardItems);
    const loadMainBoard = React.useCallback(() => {
            dispatch(loadMainBoardThunk())
    }, [dispatch])

    const dispatchUpdateItem = React.useCallback((item: IMainBoardItem) => {
        dispatch(updateMainBoardItem(item))
    }, [dispatch, updateMainBoardItem])

    return {
        mainBoard,
        loadMainBoard,
        updateItem: dispatchUpdateItem
    }
}