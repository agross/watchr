<script setup lang="ts">
import { computed, onMounted, ref, watchEffect } from 'vue'
import { Observable } from 'rxjs'
import { filter, tap } from 'rxjs/operators'
import { useSubscription } from '@vueuse/rxjs'
import type { TextReceived } from '@/model/TextReceived'
import { BufferedTerminal } from '@/model/BufferedTerminal'
import { useDark, useResizeObserver } from '@vueuse/core'
import type { ITheme } from 'xterm'
import { FitAddon } from '@xterm/addon-fit'
import { WebLinksAddon } from '@xterm/addon-web-links'
import { ConnectionState, useSignalR } from '@/composables/signalr'

const props = defineProps<{
  sessionId: string
  textReceived: Observable<TextReceived>
}>()

const isDark = useDark({ disableTransition: false })

const theme = computed<ITheme>(() => {
  if (isDark.value) {
    return {
      foreground: '#fff',
      background: '#282828',
      cursor: '#fff',
      cursorAccent: '#fff',
      selectionForeground: '#fff',
      selectionBackground: '#0073cf',
      brightYellow: '#c4a000'
    }
  }

  return {
    foreground: '#000',
    background: '#fff',
    cursor: '#000',
    cursorAccent: '#000',
    selectionForeground: '#fff',
    selectionBackground: '#003478',
    brightYellow: '#c4a000'
  }
})

const bufferedTerminal = new BufferedTerminal({
  scrollback: 20000,
  cursorBlink: false,
  cursorStyle: 'block',
  fontFamily: 'Noto Sans Mono',
  fontSize: 13,
  disableStdin: true,
  theme: theme.value
})

watchEffect(() => {
  bufferedTerminal.options.theme = theme.value
})

const { connectionState } = useSignalR()
const disconnected = computed<boolean>(() => connectionState.value !== ConnectionState.Connected)

const buffering = ref<boolean>(false)

useSubscription(
  props.textReceived
    .pipe(
      filter((x) => x.sessionId === props.sessionId),
      tap((x) => console.log(`Received: ${JSON.stringify(x)}`)),
      tap((x) => (buffering.value = bufferedTerminal.writeBuffered(x).buffering))
    )
    .subscribe()
)

const terminal = ref<HTMLDivElement | null>(null)

const fitAddon = new FitAddon()
bufferedTerminal.loadAddon(fitAddon)

useResizeObserver(terminal, (_entries) => {
  fitAddon.fit()
})

bufferedTerminal.loadAddon(new WebLinksAddon())

onMounted(() => {
  bufferedTerminal.open(terminal.value!)
  fitAddon.fit()
})
</script>

<template>
  <section>
    <header>
      <span class="status" :class="{ buffering: buffering, disconnected: disconnected }"></span>
      <span>{{ props.sessionId }}</span>
    </header>
    <div class="term" ref="terminal"></div>
  </section>
</template>

<style>
/* Handle theme change. */
.term * {
  transition: background-color 0.5s;
}
</style>

<style scoped>
section {
  border: 1px solid var(--color-accent);
  border-radius: 5px;

  display: flex;
  flex: 1;
  flex-direction: column;
  overflow: hidden;
}

header {
  background-color: var(--color-term-header);

  transition: background-color 0.5s;

  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.3rem;
}

header .status {
  display: block;
  border-radius: 50%;

  height: 0.3rem;
  width: 0.3rem;

  background-color: var(--color-ok);
  transition: background-color 0.5s ease;
}

header .status.buffering {
  background-color: var(--color-warning);
}

header .status.disconnected {
  background-color: var(--color-error);
}

.term {
  background: var(--color-background);
  transition: background-color 0.5s;

  padding: 0.2rem;
  height: 100%;
  overflow: auto;
}
</style>
