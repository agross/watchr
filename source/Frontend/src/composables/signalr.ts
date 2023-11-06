import { readonly, ref } from 'vue'
import * as signalR from '@microsoft/signalr'

export enum ConnectionState {
  Connecting = 'connecting',
  Connected = 'connected',
  Disconnected = 'disconnected'
}

const api = import.meta.env.VITE_API_URL
const reconnectDelay = 5000

const builder = new signalR.HubConnectionBuilder()
  .withUrl(`${api}/shell`)
  .withAutomaticReconnect({
    nextRetryDelayInMilliseconds: (_context) => reconnectDelay
  })
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

async function connect() {
  try {
    connectionState.value = ConnectionState.Connecting

    await builder.start()

    console.info('SignalR connected')
    connectionState.value = ConnectionState.Connected
  } catch (err) {
    console.error(`SignalR connection failed: ${err}`)
    setTimeout(connect, reconnectDelay)
  }
}

export function useSignalR() {
  return {
    builder: builder,
    connect: connect,
    connectionState: readonly(connectionState)
  }
}
