import axios, { AxiosRequestConfig } from "axios";
import { config } from "../config";
import { IMainBoardItem, IMarketDepthLevel, IOrder } from "./model";

const instance = axios.create({
    baseURL: config.baseUrl
});

const getResult = async <T>(request: AxiosRequestConfig) => {
    const response = await instance.request<T>(request)

    if (response.status !== 200)
        return undefined;

    return response.data;
}

export const api = {
    securities: {
        getDepth: (id: string) =>
            getResult<IMarketDepthLevel[]>({
                url: `api/securities/${id}/depth`,
                method: "GET",
            }),
        newOrder: (id: string, data: IOrder) =>
            getResult<boolean>({
                url: `api/securities/${id}/orders`,
                method: "POST",
                data,
            })
    },
    users: {
        getMainBoard: (username: string) =>
            getResult<IMainBoardItem[]>({
                url: `api/users/${username}/mainboard`,
                method: "GET",
            }),
    }
}