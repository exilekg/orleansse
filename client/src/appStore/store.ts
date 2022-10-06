import { configureStore } from '@reduxjs/toolkit'
import rootReducer from './rootReducer'

export type RootState = ReturnType<typeof rootReducer>
const store = configureStore({
  reducer: rootReducer,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware()
})

export type AppDispatch = typeof store.dispatch

export default store