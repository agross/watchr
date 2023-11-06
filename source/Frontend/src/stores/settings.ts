import { defineStore } from 'pinia'

export const useSettingsStore = defineStore('settings', {
  state: () => ({
    theme: ''
  }),
  persist: {
    paths: ['theme']
  }
})
