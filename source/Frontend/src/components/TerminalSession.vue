<script setup lang="ts">
const props = defineProps<{
  sessionId: string
}>()
</script>

<template>
{{ props.sessionId }}
</template>
