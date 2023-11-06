<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { Observable } from 'rxjs'
import { filter, tap } from 'rxjs/operators'
import { useSubscription, useObservable } from '@vueuse/rxjs'
import type { TextReceived } from '@/model/TextReceived'
import { BufferedTerminal } from '@/model/BufferedTerminal'

const props = defineProps<{
  sessionId: string
  textReceived: Observable<TextReceived>
}>()

const terminal = ref<HTMLDivElement | null>(null)

const bufferedTerminal = new BufferedTerminal({
  scrollback: 20000,
  cursorBlink: false,
  cursorStyle: 'block',
  fontFamily: 'Noto Sans Mono',
  fontSize: 13,
  disableStdin: true,
  theme: {
    foreground: '#000',
    background: '#fff',
    cursor: '#000',
    cursorAccent: '#000',
    selection: 'rgba(0, 52, 120, 0.25)',
    brightYellow: '#c4a000'
  }
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

onMounted(() => {
  bufferedTerminal.open(terminal.value!)
})
</script>

<template>
  {{ props.sessionId }}
  <div ref="terminal"></div>
</template>
