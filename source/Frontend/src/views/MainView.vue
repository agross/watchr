<script setup lang="ts">
import { reactive, ref } from 'vue'
import ThemeButton from '@/components/ThemeButton.vue'
import ConnectionIndicator from '@/components/ConnectionIndicator.vue'
import WelcomeHero from '@/components/WelcomeHero.vue'
import TerminalSession from '@/components/TerminalSession.vue'
import { useSignalR } from '@/composables/signalr'
import type { TextReceived } from '@/model/TextReceived'

const { on } = useSignalR()

const sessions = reactive<string[]>([])

on('text', async (text: TextReceived) => {
  console.log(`Text: ${JSON.stringify(text)}`)

  const existing = sessions.find((x) => x === text.sessionId)
  if (!existing) {
    sessions.push(text.sessionId)
  }
})

on('terminate', (sessionId: string) => {
  console.log(`Terminate session ${sessionId}`)
})
</script>

<template>
  <header>
    <ThemeButton />
    <ConnectionIndicator />
  </header>

  <main>
    <WelcomeHero v-if="sessions.length === 0" />
    <TerminalSession v-for="sessionId in sessions" :key="sessionId" :sessionId="sessionId" />
  </main>
</template>

<style scoped>
header {
  position: absolute;
  right: 0;

  display: flex;
  align-items: center;

  margin-top: 1rem;
}

header > * {
  margin-right: 1rem;
}
</style>
