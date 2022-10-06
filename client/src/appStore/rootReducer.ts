import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'
import { IMainBoardItem, IMarketDepth, ISecurity } from './model'

interface IAppState {
    mainBoardItems?: IMainBoardItem[];
    depths: {
        [securityId: string]: IMarketDepth;
    };
    usename?: string;
    securities?: ISecurity[];
    loadingNewOrder?: boolean
}


const initialState = { depths: {} } as IAppState

const appSlice = createSlice({
  name: 'appSlice',
  initialState,
  reducers: {
    setUserName: (state, action: PayloadAction<string>) => {
        state.usename = action.payload;
    },
    setMainBoardItems: (state, action: PayloadAction<IMainBoardItem[]>) => {
        state.mainBoardItems = action.payload;
    },
    updateMainBoardItem: (state, action: PayloadAction<IMainBoardItem>) => {
        if (!state.mainBoardItems) 
            return;

        const item = state.mainBoardItems.find(mbi => mbi.securityId === action.payload.securityId);
        if (!item)
            return;

        const index = state.mainBoardItems.indexOf(item);
        state.mainBoardItems[index] = action.payload;

        const depth = state.depths[action.payload.securityId]
        if (depth && !depth.invalidated) {
            depth.invalidated = true;
        }
    },
    setMarketDepth: (state, action: PayloadAction<IMarketDepth>) => {
        state.depths[action.payload.securityId] = action.payload;
    },
    setSecurities: (state, action: PayloadAction<ISecurity[]>) => {
        state.securities = action.payload;
    },
    setNewOrderLoading: (state, action: PayloadAction<boolean | undefined>) => {
        state.loadingNewOrder = action.payload;
    },
  },
})

export const { setMainBoardItems, setMarketDepth, setSecurities, setUserName, updateMainBoardItem, setNewOrderLoading } = appSlice.actions
export default appSlice.reducer