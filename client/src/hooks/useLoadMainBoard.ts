import React from "react"
import { api } from "../appStore/api"
import { useAppDispatch, useAppSelector } from "../appStore/hooks"
import { AppDispatch, RootState } from "../appStore/store"
import { setMainBoardItems } from "../appStore/rootReducer"
import { useUser } from "./useUser"

const loadMainBoardThunk = (username: string) => 
    async (dispatch: AppDispatch, getState: () => RootState) => {
        const mainBoard = await api.users.getMainBoard(username);
        if (mainBoard) {
            dispatch(setMainBoardItems(mainBoard))
        }
    }

export const useLoadMainBoard = () => {
    const { username } = useUser()
    const dispatch = useAppDispatch();
    React.useEffect(() => {
        if (username) {
            dispatch(loadMainBoardThunk(username))
        }
    }, [username])
}