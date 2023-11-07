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

const props = defineProps<{
  sessionId: string
  textReceived: Observable<TextReceived>
}>()

const isDark = useDark({ disableTransition: false })

const theme = computed<ITheme>(() => {
  if (isDark.value) {
    return {
      foreground: '#fff',
      background: '#303030',
      cursor: '#fff',
      cursorAccent: '#fff',
      selection: 'rgba(0, 52, 120, 0.25)',
      brightYellow: '#c4a000'
    }
  }

  return {
    foreground: '#000',
    background: '#fff',
    cursor: '#000',
    cursorAccent: '#000',
    selection: 'rgba(0, 52, 120, 0.25)',
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

useSubscription(
  props.textReceived
    .pipe(
      filter((x) => x.sessionId === props.sessionId),
      tap((x) => console.log(`Received: ${JSON.stringify(x)}`)),
      tap((x) => bufferedTerminal.writeBuffered(x))
    )
    .subscribe()
)

const terminal = ref<HTMLDivElement | null>(null)

const fitAddon = new FitAddon()
bufferedTerminal.loadAddon(fitAddon)

useResizeObserver(terminal, (_entries) => {
  fitAddon.fit()
})

bufferedTerminal.loadAddon(new WebLinksAddon());

onMounted(() => {
  bufferedTerminal.open(terminal.value!)
  fitAddon.fit()
})
</script>

<template>
  <section>
    <header>
      {{ props.sessionId }}
    </header>
    <div ref="terminal"></div>
  </section>
</template>

<style>
.terminal {
  padding: 0.2rem;
  height: 100%;
  overflow: auto;
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
  text-align: center;
  background-color: var(--color-term-header);
}
</style>
