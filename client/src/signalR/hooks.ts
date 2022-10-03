import React from "react";
import { NotificationContext } from "./NotificationContext";

export const useNotificationHandler = <TNotification>(
	type: string,
	handler: (e: TNotification) => void,
	deps: React.DependencyList
) => {
	const connectionManager = React.useContext(NotificationContext);
	const handlerCallback = React.useCallback(handler, deps);
	React.useEffect(
		() => connectionManager.registerHandler(type, handlerCallback),
		[connectionManager, type, handlerCallback]
	);
};
