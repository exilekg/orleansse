import { Add } from "@mui/icons-material";
import { Box, Card, CardContent, IconButton, Tooltip } from "@mui/material";
import MaterialReactTable, { MRT_ColumnDef } from "material-react-table";
import React from "react";
import { IMainBoardItem } from "../appStore/model";
import { useMainBoard } from "../hooks/useMainBoard";
import { useNotificationHandler } from "../signalR/hooks";
import MainAppBar from "./MainAppBar";
import { MarketDepth } from "./MarketDepth";
import { NewOrder } from "./NewOrder";

const columns: MRT_ColumnDef<IMainBoardItem>[] = [
	{
		accessorKey: "symbol",
		header: "Symbol",
		size: 10,
	},
	{
		accessorKey: "price",
		header: "Price",
		size: 10,
	},
	{
		accessorKey: "bidAmount",
		header: "Bid Amount",
		size: 10,
	},
	{
		accessorKey: "bidPrice",
		header: "Bid Price",
		size: 10,
	},
	{
		accessorKey: "askAmount",
		header: "Ask Amount",
		size: 10,
	},
	{
		accessorKey: "askPrice",
		header: "Ask Price",
		size: 10,
	},
	{
		accessorKey: "volume",
		header: "Volume",
		size: 10,
	},
	{
		accessorKey: "vwap",
		header: "VWAP",
		size: 10,
	},
	{
		accessorKey: "high",
		header: "High",
		size: 10,
	},
	{
		accessorKey: "low",
		header: "Low",
		size: 10,
	},
	{
		accessorKey: "change",
		header: "Change",
		size: 10,
	},
	{
		accessorKey: "changePercent",
		header: "Change %",
		size: 10,
	},
];

export const MainBoard: React.FC = () => {
	const { mainBoard, loadMainBoard, updateItem } = useMainBoard();
	const [newOrderItem, setNewOrderItem] = React.useState<IMainBoardItem | undefined>();
	useNotificationHandler("mainBoardChange", updateItem, []);

	React.useEffect(() => {
		if (!mainBoard) {
			loadMainBoard();
		}
	}, [loadMainBoard, mainBoard]);

	return (
		<>
		<MainAppBar />
		<Card sx={{ minWidth: 275 }}>
			<CardContent>
				{mainBoard ? (
					<MaterialReactTable
						columns={columns}
						data={mainBoard}
						enableEditing
						enableColumnActions={false}
						enableColumnFilters={false}
						renderDetailPanel={({ row }) => <MarketDepth securityId={row.original.securityId} />}
						renderRowActions={({ row, table }) => (
							<Box sx={{ display: "flex", gap: "1rem" }}>
								<Tooltip arrow placement="left" title="New order">
									<IconButton onClick={() => setNewOrderItem(row.original)}>
										<Add />
									</IconButton>
								</Tooltip>
							</Box>
						)}
						enablePagination={false}
					/>
				) : null}
				<NewOrder onClose={() => setNewOrderItem(undefined)} mainBoardItem={newOrderItem} />
			</CardContent>
		</Card>
		</>
	);
};
