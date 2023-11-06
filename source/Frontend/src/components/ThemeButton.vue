<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useSettingsStore } from '@/stores/settings'

const settings = useSettingsStore()

const userTheme = computed(() => settings.theme || mediaPreference())

function setTheme(theme: string) {
  settings.theme = theme
  document.documentElement.className = theme
}

function toggleTheme() {
  const activeTheme = userTheme.value

  if (activeTheme === 'light-theme') {
    setTheme('dark-theme')
  } else {
    setTheme('light-theme')
  }
}

function mediaPreference() {
  const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches

  if (prefersDark) {
    return 'dark-theme'
  } else {
    return 'light-theme'
  }
}

onMounted(() => {
  setTheme(userTheme.value)
})
</script>

<template>
  <div class="theme-selector">
    <input @change="toggleTheme" id="checkbox" type="checkbox" />
    <label for="checkbox">
      <span>🌙</span>
      <span>☀️</span>
      <div class="toggle" :class="{ checked: userTheme === 'dark-theme' }"></div>
    </label>
  </div>
</template>

<style>
:root {
  --element-size: 4rem;
}
</style>

<style scoped>
.theme-selector{
  margin: 1rem;
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
