import { Box, Table, TableBody, TableCell, TableHead, TableRow, Typography } from "@mui/material";
import MaterialReactTable, { MRT_ColumnDef } from "material-react-table";
import React from "react";
import { IMarketDepthLevel } from "../appStore/model";
import { useMarketDepth } from "../hooks/useMarketDepth";

interface MarketDepthProps {
	securityId: string;
}

const columns: MRT_ColumnDef<IMarketDepthLevel>[] = [
	{
		accessorKey: "index",
		header: "",
		size: 10,
	},
	{
		accessorKey: "bidPrice",
		header: "Bid Price",
		size: 10,
	},
	{
		accessorKey: "bidAmount",
		header: "Bid Amount",
		size: 10,
	},
	{
		accessorKey: "askPrice",
		header: "Bid Price",
		size: 10,
	},
	{
		accessorKey: "askAmount",
		header: "Ask Amount",
		size: 10,
	},
];

export const MarketDepth: React.FC<MarketDepthProps> = ({ securityId }) => {
	const { loadMarketDepth, marketDepth } = useMarketDepth(securityId);
	console.log(marketDepth);

	React.useEffect(() => {
		if (!marketDepth || marketDepth.invalidated) {
			console.log("reload depth");
			loadMarketDepth();
		}
	}, [loadMarketDepth, marketDepth]);

    if (!marketDepth?.depth)
        return null;

        return (
            <Box sx={{ margin: 1 }}>
              <Table size="small" aria-label="purchases">
                <TableHead>
                  <TableRow>
                    <TableCell></TableCell>
                    <TableCell>Bid Price</TableCell>
                    <TableCell>Bid Amount</TableCell>
                    <TableCell>Ask Price</TableCell>
                    <TableCell>Ask Amount</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {marketDepth.depth.map((level) => (
                    <TableRow key={level.index}>
                      <TableCell component="th" scope="row">
                        {level.index}
                      </TableCell>
                      <TableCell>{level.bidPrice}</TableCell>
                      <TableCell>{level.bidAmount}</TableCell>
                      <TableCell>{level.askPrice}</TableCell>
                      <TableCell>{level.askAmount}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </Box>
        )
	// return (
	// 	<MaterialReactTable
	// 		columns={columns}
	// 		data={marketDepth.depth}
	// 		enableColumnActions={false}
	// 		enableColumnFilters={false}
	// 		enablePagination={false}
	// 		enableSorting={false}
	// 		enableBottomToolbar={false}
	// 		enableTopToolbar={false}
	// 		muiTableBodyRowProps={{ hover: false }}
	// 	/>
	//);
};
