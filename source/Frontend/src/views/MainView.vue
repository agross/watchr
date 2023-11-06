<script setup lang="ts">
import { reactive } from 'vue'
import { ReplaySubject } from 'rxjs'
import ThemeButton from '@/components/ThemeButton.vue'
import ConnectionIndicator from '@/components/ConnectionIndicator.vue'
import WelcomeHero from '@/components/WelcomeHero.vue'
import TerminalSession from '@/components/TerminalSession.vue'
import { useSignalR } from '@/composables/signalr'
import type { TextReceived } from '@/model/TextReceived'

const { on } = useSignalR()

const subject = new ReplaySubject<TextReceived>(10, 5000)

const sessions = reactive<string[]>([])

on('text', async (text: TextReceived) => {
  console.log(`Text: ${JSON.stringify(text)}`)

  const existing = sessions.find((x) => x === text.sessionId)
  if (!existing) {
    sessions.push(text.sessionId)
  }

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
    <section class="sessions">
      <TerminalSession
        v-for="sessionId in sessions"
        :key="sessionId"
        :sessionId="sessionId"
        :textReceived="subject"
      />
    </section>
  </main>
</template>

<style scoped>
header {
  position: absolute;
  right: 1rem;

  display: flex;
  gap: 1rem;
  align-items: center;

  /* Float above xterm.js */
  z-index: 1000;
}

.sessions {
  display: flex;
  gap: 1rem;
}

@media all and (max-width: 800px) {
  .sessions {
    flex-direction: column;
  }
}
</style>
