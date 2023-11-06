<script setup lang="ts">
import { reactive } from 'vue'
import { Subject } from 'rxjs'
import ThemeButton from '@/components/ThemeButton.vue'
import ConnectionIndicator from '@/components/ConnectionIndicator.vue'
import WelcomeHero from '@/components/WelcomeHero.vue'
import TerminalSession from '@/components/TerminalSession.vue'
import { useSignalR } from '@/composables/signalr'
import type { TextReceived } from '@/model/TextReceived'

const { on } = useSignalR()

const subject = new Subject<TextReceived>()

const sessions = reactive<string[]>([])

on('text', async (text: TextReceived) => {
  console.log(`Text: ${JSON.stringify(text)}`)

  const existing = sessions.find((x) => x === text.sessionId)
  if (!existing) {
    sessions.push(text.sessionId)
  }

  // The first notification will arrive before xterm is set up.
  subject.next(text)
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
    <TerminalSession
      v-for="sessionId in sessions"
      :key="sessionId"
      :sessionId="sessionId"
      :textReceived="subject"
    />
  </main>
</template>

<style scoped>
header {
  position: absolute;
  right: 0;

  display: flex;
  align-items: center;

  margin-top: 1rem;

  /* Float above xterm.js */
  z-index: 1000;
}

header > * {
  margin-right: 1rem;
}
</style>
