<script setup lang="ts">
import { useDark, useToggle } from '@vueuse/core'

const isDark = useDark({ disableTransition: false })
const toggleDark = useToggle(isDark)
</script>

<template>
  <div>
    <input @change="toggleDark()" id="checkbox" type="checkbox" />
    <label for="checkbox">
      <span>🌙</span>
      <span>☀️</span>
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
  cursor: pointer;

  transition: border-color 0.5s ease;
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
