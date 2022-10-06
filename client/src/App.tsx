import React from "react";
import "./App.css";
import { NotificationContextProvider } from "./signalR/NotificationContext";
import { LogIn } from "./components/Login";
import { createTheme, ThemeProvider } from "@mui/material/styles";
import { Provider } from "react-redux";
import store from "./appStore/store";
import { useUser } from "./hooks/useUser";
import { config } from "./config";
import { MainBoard } from "./components/MainBoard";

const theme = createTheme();

function App() {
	return (
		<Provider store={store}>
			<ThemeProvider theme={theme}>
				{/* <NotificationContextProvider hubUrl="https://localhost:7037/notificationsHub"> */}
				<NotificationContextProvider hubUrl={config.notificationsUrl}>
					<MainComponent />
				</NotificationContextProvider>
			</ThemeProvider>
		</Provider>
	);
}

function MainComponent() {
	const { setUserName, username } = useUser();
	return (<>
		{ !username ? <LogIn setUsername={setUserName}/> : <MainBoard /> }
	</>)
}

export default App;
