import './assets/main.css'
import 'xterm/css/xterm.css'

import { createApp } from 'vue'
import App from './App.vue'

const app = createApp(App)

import { createPinia } from 'pinia'
import piniaPluginPersistedState from 'pinia-plugin-persistedstate'

const pinia = createPinia()
pinia.use(piniaPluginPersistedState)
app.use(pinia)

app.mount('#app')
