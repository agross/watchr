import { readonly, ref } from 'vue'
import * as signalR from '@microsoft/signalr'

export enum ConnectionState {
  Connecting = 'connecting',
  Connected = 'connected',
  Disconnected = 'disconnected'
}

const api = import.meta.env.VITE_API_URL
const reconnectDelay = 5000

class AlwaysRetryPolicy implements signalR.IRetryPolicy {
  nextRetryDelayInMilliseconds(_retryContext: signalR.RetryContext): number {
    return reconnectDelay
  }
}

const builder = new signalR.HubConnectionBuilder()
  .withUrl(`${api}/shell`)
  .withAutomaticReconnect(new AlwaysRetryPolicy())
  .configureLogging(signalR.LogLevel.Information)
  .build()

builder.onclose(() => {
  console.info('SignalR closed')
  connectionState.value = ConnectionState.Disconnected
})

builder.onreconnecting((err) => {
  console.warn(`SignalR reconnecting: ${err}`)
  connectionState.value = ConnectionState.Connecting
})

builder.onreconnected(() => {
  console.info('SignalR reconnected')
  connectionState.value = ConnectionState.Connected
})

const connectionState = ref(ConnectionState.Disconnected)

const connect = async () => {
  try {
    connectionState.value = ConnectionState.Connecting

    if (builder.state === signalR.HubConnectionState.Disconnected) {
      await builder.start()
    }

    console.info('SignalR connected')
    connectionState.value = ConnectionState.Connected
  } catch (err) {
    console.error(`SignalR connection failed: ${err}`)
    setTimeout(connect, reconnectDelay)
  }
}

const on = (methodName: string, newMethod: (...args: any[]) => any) => {
  builder.on(methodName, newMethod)
}

export function useSignalR() {
  return {
    on: on,
    connect: connect,
    connectionState: readonly(connectionState)
  }
}
