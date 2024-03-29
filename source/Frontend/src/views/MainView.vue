<script setup lang="ts">
import { ref } from 'vue'
import { ReplaySubject } from 'rxjs'
import ThemeButton from '@/components/ThemeButton.vue'
import ConnectionIndicator from '@/components/ConnectionIndicator.vue'
import WelcomeHero from '@/components/WelcomeHero.vue'
import TerminalSession from '@/components/TerminalSession.vue'
import { useSignalR } from '@/composables/signalr'
import type { TextReceived } from '@/model/TextReceived'

const { on } = useSignalR()

const subject = new ReplaySubject<TextReceived>(10, 5000)

const sessions = ref<string[]>([])

on('text', async (text: TextReceived) => {
  const existing = sessions.value.find((x) => x === text.sessionId)
  if (!existing) {
    sessions.value.push(text.sessionId)
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
        @close="sessions = sessions.filter(x => x !== sessionId)"
      />
    </section>
  </main>
</template>

<style scoped>
header {
  position: absolute;
  top: 1rem;
  right: 1rem;

  display: flex;
  gap: 1rem;
  align-items: center;

  /* Float above xterm.js */
  z-index: 1000;
}

.sessions {
  display: flex;
  /* Space between. */
  gap: 1rem;
  /* Space around. */
  padding: 1rem;
  height: 100%;
}

@media all and (max-width: 800px) {
  .sessions {
    flex-direction: column;
  }
}
</style>
