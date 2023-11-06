<script setup lang="ts">
import ThemeButton from '@/components/ThemeButton.vue'
import ConnectionIndicator from '@/components/ConnectionIndicator.vue'
import WelcomeHero from '@/components/WelcomeHero.vue'
</script>

<template>
  <header>
    <ThemeButton />
    <ConnectionIndicator />
  </header>

  <main>
    <WelcomeHero />
  </main>
</template>

<style scoped>
header {
  position: absolute;
  right: 0;

  display: flex;
  align-items: center;

  margin-top: 1rem;
}

header > * {
  margin-right: 1rem;
}
</style>
