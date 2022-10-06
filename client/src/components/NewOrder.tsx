import React from "react";
import { IMainBoardItem, OrderSide } from "../appStore/model";
import { useNewOrder } from "../hooks/useNewOrder";
import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogContentText from "@mui/material/DialogContentText";
import DialogTitle from "@mui/material/DialogTitle";
import Slide from "@mui/material/Slide";
import { TransitionProps } from "@mui/material/transitions";
import { Box, Stack, TextField, ToggleButton, ToggleButtonGroup, Typography } from "@mui/material";

const Transition = React.forwardRef(function Transition(
	props: TransitionProps & {
		children: React.ReactElement<any, any>;
	},
	ref: React.Ref<unknown>
) {
	return <Slide direction="up" ref={ref} {...props} />;
});

interface NewOrderProps {
	mainBoardItem?: IMainBoardItem;
	onClose: () => void;
}

export const NewOrder: React.FC<NewOrderProps> = ({ mainBoardItem, onClose }) => {
	const { loading, newOrder } = useNewOrder();
	const [quantity, setQuantity] = React.useState<number>(0);
	const [price, setPrice] = React.useState<number>(0);
	const [side, setSide] = React.useState<OrderSide>(OrderSide.Sell);

	const sendNewOrder = async () => {
		if (mainBoardItem) {
			await newOrder(mainBoardItem.securityId, {
				price,
				quantity,
				side,
			});
		}
		onClose();
	};
	return (
		<Dialog
			open={!!mainBoardItem}
			TransitionComponent={Transition}
			keepMounted={false}
			onClose={onClose}
			aria-describedby="alert-dialog-slide-description"
		>
			<DialogTitle>{`New order`}</DialogTitle>
			<DialogContent>
				<Stack spacing={4}>
					<Typography>
						Symbol: <b>{mainBoardItem?.symbol}</b>
					</Typography>
						<ToggleButtonGroup
							color="primary"
							size="small"
							value={side}
							exclusive
							onChange={(e, newValue) => setSide(newValue)}
							aria-label="Platform"
						>
							<ToggleButton value={OrderSide.Buy}>Buy</ToggleButton>
							<ToggleButton value={OrderSide.Sell}>Sell</ToggleButton>
						</ToggleButtonGroup>
					<TextField
						label="Amount"
						type="number"
						InputLabelProps={{
							shrink: true,
						}}
						variant="standard"
						onChange={e => setQuantity(parseInt(e.target.value))}
					/>
					<TextField
						label="Price"
						type="number"
						InputLabelProps={{
							shrink: true,
						}}
						variant="standard"
						onChange={e => setPrice(parseInt(e.target.value))}
					/>
				</Stack>
			</DialogContent>
			<DialogActions>
				<Button onClick={onClose}>Cancel</Button>
				<Button variant="contained" disabled={loading} onClick={sendNewOrder}>
					Ok
				</Button>
			</DialogActions>
		</Dialog>
	);
};
