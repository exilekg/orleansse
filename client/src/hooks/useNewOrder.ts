import React from "react";
import { api } from "../appStore/api";
import { useAppDispatch, useAppSelector } from "../appStore/hooks";
import { AppDispatch } from "../appStore/store";
import { setNewOrderLoading } from "../appStore/rootReducer";
import { IOrder } from "../appStore/model";

const newOrderThunk = (securityId: string, newOrder: IOrder) => async (dispatch: AppDispatch) => {
	dispatch(setNewOrderLoading(true));
	const result = await api.securities.newOrder(securityId, newOrder);
	dispatch(setNewOrderLoading(false));
	return result;
};

export const useNewOrder = () => {
	const dispatch = useAppDispatch();
	const loading = useAppSelector(state => state.loadingNewOrder);

	const newOrder = React.useCallback(
		(securityId: string, order: IOrder) => dispatch(newOrderThunk(securityId, order)),
		[dispatch]
	);

	return {
		loading,
		newOrder,
	};
};
