<script setup lang="ts">
import { onMounted } from 'vue'
import { useDark, useToggle } from '@vueuse/core'

const isDark = useDark({ disableTransition: false })
const toggleDark = useToggle(isDark)

onMounted(() => {
  document.body.style.transition = 'color 0.5s, background-color 0.5s'
})
</script>

<template>
  <div>
    <input @change="toggleDark()" id="checkbox" type="checkbox" />
    <label for="checkbox">
      <span><i class="pi pi-moon"></i></span>
      <span><i class="pi pi-sun"></i></span>
      <div class="toggle" :class="{ checked: isDark }"></div>
    </label>
  </div>
</template>

<style>
:root {
  --element-size: 4rem;
}
</style>

<style scoped>
.pi {
  color: var(--color-accent);
}

input[type='checkbox'] {
  display: none;
}

label {
  display: flex;
  align-items: center;
  justify-content: space-between;

  user-select: none;

  position: relative;
  padding: calc(var(--element-size) * 0.1);
  width: var(--element-size);

  border: 1px solid var(--color-accent);
  border-radius: var(--element-size);
  background-color: var(--color-background);
  cursor: pointer;

  transition:
    background-color 0.5s ease,
    border-color 0.5s ease;
}

.toggle {
  position: absolute;
  height: calc(var(--element-size) * 0.4);
  width: calc(var(--element-size) * 0.4);

  background-color: var(--color-accent);
  border-radius: 50%;

  transform: translateX(0);
  transition:
    transform 0.3s ease,
    background-color 0.5s ease;
}

.checked {
  transform: translateX(calc(var(--element-size) * 0.37));
}
</style>
