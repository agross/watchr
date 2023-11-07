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

  height: calc(var(--element-size) * 0.2);
  width: calc(var(--element-size) * 0.2);

  transition: background-color 0.5s ease;
}

.connected {
  background-color: var(--color-ok);
}

.connecting {
  background-color: var(--color-warning);
}

.disconnected {
  background-color: var(--color-error);
}
</style>
