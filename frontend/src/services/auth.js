import { computed, ref } from 'vue'

const TOKEN_KEY = 'wifiRadarToken'
const authToken = ref(localStorage.getItem(TOKEN_KEY))

export const isLoggedIn = computed(() => Boolean(authToken.value))

export const setAuthToken = (token) => {
  authToken.value = token
  localStorage.setItem(TOKEN_KEY, token)
}

export const clearAuthToken = () => {
  authToken.value = null
  localStorage.removeItem(TOKEN_KEY)
}
