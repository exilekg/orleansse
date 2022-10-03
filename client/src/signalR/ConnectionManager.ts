import * as signalR from "@microsoft/signalr";

const reconnectTimeout = 10 * 1000;

class RetryPolicy implements signalR.IRetryPolicy {
	private readonly backoff: number[] = [0, 500, 1500, 1500, 3000];
	private shouldReconnect: boolean = true;

	public nextRetryDelayInMilliseconds(retryContext: signalR.RetryContext) {
		console.log("SignalR reconnect context", retryContext);
		if (!this.shouldReconnect) {
			return null;
		}
		const selected = this.backoff[retryContext.previousRetryCount];
		return selected !== undefined ? selected : reconnectTimeout;
	}

	public stopReconnect = () => {
		this.shouldReconnect = false;
	};
}

export class ConnextionManager {
	private readonly hubUrl: string;
	private connection?: signalR.HubConnection;
	private readonly retryPolicy = new RetryPolicy();

	constructor(hubUrl: string) {
		this.hubUrl = hubUrl;
		this.createConnection();
	}

	private createConnection = () => {
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl(this.hubUrl)
			.configureLogging(signalR.LogLevel.Information)
			.withAutomaticReconnect(this.retryPolicy)
			.build();

		this.connection.onreconnected(connectionId => {
			console.log("SignalR reconnected: ", connectionId);
		});

		this.connection.onclose(async error => {
			console.log("SignalR closed", error);
		});
	};

	public start = async () => {
		try {
			await this.connection?.start();
			console.log("SignalR connected: ", this.connection?.connectionId);
		} catch (err) {
			console.log("SignalR connection error", err);
		}
	};

	public registerHandler = <T>(type: string, handler: (e: T) => void) => {
		console.log("Subscribing", type);
		this.connection?.on(type, handler);
		return () => this.connection?.off(type);
	};

	public stop = async () => {
		this.retryPolicy.stopReconnect();
		console.log("stopping SignalR: ", this.connection?.connectionId);
		await this.connection?.stop();
	};

	public reconnect = async () => {
		await this.connection?.stop();
		this.createConnection();
		await this.start();
	};
}
