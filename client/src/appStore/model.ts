export interface IMainBoardItem {
	securityId: string;
	symbol: string;
	bidAmount: number;
	bidPrice: number;
	askAmount: number;
	askPrice: number;
	price: number;
	open: number;
	high: number;
	low: number;
	change: number;
	changePercent: number;
	volume: number;
	vwap: number;
}

export interface IMarketDepthLevel {
	securityId: string;
	index: number;
	bidPrice: number;
	bidAmount: number;
	askPrice: number;
	askAmount: number;
}

export interface IMarketDepth {
	securityId: string;
	depth: IMarketDepthLevel[];
	invalidated?: boolean;
}

export interface ISecurity {
	id: string;
	symbol: string;
}

export interface IOrder {
	quantity: number;
	price: number;
	side: OrderSide;
}

export enum OrderSide {
    Buy = 0,
    Sell = 1,
}
