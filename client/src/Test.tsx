import React from "react";
import { useNotificationHandler } from "./signalR/hooks";

export const TestCmp: React.FC = () => {

    useNotificationHandler("mainBoardChange", console.log, []);
    return <div>Test</div>
}