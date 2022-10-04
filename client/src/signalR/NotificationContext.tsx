import React from "react";
import { ConnextionManager } from "./ConnectionManager";

export const NotificationContext = React.createContext<ConnextionManager>({} as any);

export interface NotificationContextProviderProps {
	hubUrl: string;
	children: React.ReactNode;
}

export const NotificationContextProvider: React.FC<NotificationContextProviderProps> = ({
	hubUrl,
	children,
}) => {
	const [connectionManager, setConnectionManager] = React.useState<ConnextionManager | undefined>();
	React.useEffect(() => {
		const manager = new ConnextionManager(hubUrl);
		manager.start()
			.then(() => setConnectionManager(manager))
		return () => {
			if (manager) {
				manager.stop();
			}
		};
	}, [hubUrl]);

	return connectionManager ? (
		<NotificationContext.Provider value={connectionManager}>
			{children}
		</NotificationContext.Provider>
	) : null;
};
