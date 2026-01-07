<template>
  <v-app>
    <v-app-bar app color="primary" dark>
      <v-toolbar-title class="font-weight-bold">WiFi Radar</v-toolbar-title>

      <v-spacer />

      <v-btn to="/" text>Home</v-btn>
      <v-btn to="/map" text>Map</v-btn>

      <template v-if="!loggedIn">
        <v-btn to="/login" text>Login</v-btn>
        <v-btn to="/register" text>Register</v-btn>
      </template>

      <template v-else>
        <v-btn to="/profile" text>Profile</v-btn>
        <v-btn @click="logout" text>Log Out</v-btn>
      </template>
    </v-app-bar>

    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<script setup>
import { clearAuthToken, isLoggedIn } from '@/services/auth'

const loggedIn = isLoggedIn

const logout = () => {
  clearAuthToken()
}
</script>
