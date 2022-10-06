import * as React from "react";
import Avatar from "@mui/material/Avatar";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Paper from "@mui/material/Paper";
import Box from "@mui/material/Box";
import Grid from "@mui/material/Grid";
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";
import Typography from "@mui/material/Typography";

export interface LogInProps {
	setUsername: (username: string) => void;
}

export const LogIn: React.FC<LogInProps> = ({ setUsername }) => {
	const [currentUsername, setCurrentUsername] = React.useState<string | undefined>(undefined);

	const signIn = React.useCallback(() => {
		if (currentUsername) {
			setUsername(currentUsername);
		}
	}, [currentUsername, setUsername])

	return (
		<Grid container component="main" sx={{ height: "100vh" }}>
			<CssBaseline />
			<Grid
				item
				xs={false}
				sm={4}
				md={9}
				sx={{
					backgroundImage: "url(https://source.unsplash.com/random)",
					backgroundRepeat: "no-repeat",
					backgroundColor: t =>
						t.palette.mode === "light" ? t.palette.grey[50] : t.palette.grey[900],
					backgroundSize: "cover",
					backgroundPosition: "center",
				}}
			/>
			<Grid item xs={12} sm={8} md={3} component={Paper} elevation={6} square>
				<Box
					sx={{
						my: 8,
						mx: 4,
						display: "flex",
						flexDirection: "column",
						alignItems: "center",
						padding: "20px",
					}}
				>
					<Avatar sx={{ m: 1, bgcolor: "secondary.main" }}>
						<LockOutlinedIcon />
					</Avatar>
					<Typography component="h1" variant="h5">
						Sign in
					</Typography>
					<Box sx={{ mt: 1 }}>
						<TextField
							margin="normal"
							required
							fullWidth
							id="email"
							label="User name"
							name="email"
							autoComplete="email"
							autoFocus
							variant="standard"
							onChange={e => { setCurrentUsername(e.target.value) } }
						/>
						<TextField
							margin="normal"
							required
							fullWidth
							name="password"
							label="Password"
							type="password"
							id="password"
							autoComplete="current-password"
							variant="standard"
						/>
						<Button fullWidth variant="outlined" sx={{ mt: 3, mb: 2 }} onClick={signIn} disabled={!currentUsername}>
							Sign In
						</Button>
					</Box>
				</Box>
			</Grid>
		</Grid>
	);
};
