<script setup lang="ts">
import { useSignalR } from '@/composables/signalr'

const { connect, connectionState } = useSignalR()

await connect()
</script>

<template>
  <span :class="connectionState"></span>
</template>

<style scoped>
span {
  display: block;
  border-radius: 50%;

  height: calc(var(--element-size) * 0.4);
  width: calc(var(--element-size) * 0.4);

  transition: background-color 0.5s ease;
}

.connected {
  background-color: green;
}

.connecting {
  background-color: orange;
}

.disconnected {
  background-color: red;
}

@media (prefers-color-scheme: dark) {
  html.dark-theme .connected {
    background-color: greenyellow;
  }
}
</style>
