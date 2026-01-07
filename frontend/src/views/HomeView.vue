<template>
  <v-container
    class="fill-height d-flex flex-column justify-center align-center text-center py-16 px-4"
  >
    <v-row justify="center">
      <v-col cols="12">
        <h1 class="text-h2 font-weight-bold mb-2">WiFi Radar</h1>
        <p class="text-subtitle-1 mb-6">
          Explore and discover free public Wi-Fi hotspots near you.
        </p>
        <div class="cta-stack">
          <template v-if="!loggedIn">
            <v-btn color="primary" size="large" block to="/login"> Login </v-btn>
            <v-btn color="primary" variant="outlined" size="large" block to="/register">
              Register
            </v-btn>
          </template>
          <template v-else>
            <v-btn color="primary" size="large" block @click="goToMap"> Start Exploring </v-btn>
          </template>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { isLoggedIn } from '@/services/auth'

const router = useRouter()
const loggedIn = isLoggedIn

const goToMap = () => {
  router.push('/map')
}
</script>

<style scoped>
h1 {
  color: #2c3e50;
}
p {
  color: #666;
}
.cta-stack {
  width: 100%;
  max-width: 320px;
  margin: 0 auto;
  display: grid;
  gap: 12px;
}
</style>
