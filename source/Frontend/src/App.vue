<script setup lang="ts">
import ThemeButton from './components/ThemeButton.vue'
import WelcomeHero from './components/WelcomeHero.vue'
</script>

<template>
  <header>
    <ThemeButton />
  </header>

  <main>
    <WelcomeHero />
  </main>
</template>

<style scoped>
header {
  position: absolute;
  right: 0;
}
</style>
