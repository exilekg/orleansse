import React from "react";
import "./App.css";
import { TestCmp } from "./Test";
import { NotificationContextProvider } from "./signalR/NotificationContext";
import { LogIn } from "./Login";
import { createTheme, ThemeProvider } from "@mui/material/styles";

const theme = createTheme();

function App() {
	return (
		<ThemeProvider theme={theme}>
			{/* <NotificationContextProvider hubUrl="https://localhost:7037/notificationsHub"> */}
			<NotificationContextProvider hubUrl="/notificationsHub">
				<LogIn />
        <TestCmp />
			</NotificationContextProvider>
		</ThemeProvider>
	);
}

export default App;
