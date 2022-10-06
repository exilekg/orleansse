import { useAppDispatch, useAppSelector } from "../appStore/hooks"
import { setUserName } from "../appStore/rootReducer"

export const useUser = () => {
    const username = useAppSelector(state => state.usename)
    const dispatch = useAppDispatch();
    const dispatchSetUserName = (username: string) => dispatch(setUserName(username));
    return {
        username,
        setUserName: dispatchSetUserName,
    }
}