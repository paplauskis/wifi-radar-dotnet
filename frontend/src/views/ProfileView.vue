<template>
  <v-container class="py-12">
    <v-row justify="center">
      <v-col cols="12" md="8">
        <v-card class="pa-6" elevation="4">
          <v-card-title class="font-weight-bold text-h5 mb-4">My Profile</v-card-title>

          <v-card-text>
            <div class="profile-header">
              <v-avatar size="96" class="profile-avatar">
                <v-img v-if="photoUrl" :src="photoUrl" cover />
                <v-icon v-else size="72">mdi-account-circle</v-icon>
              </v-avatar>

              <div class="profile-meta">
                <div class="text-h6 font-weight-bold">{{ profile.name }}</div>
                <div class="text-subtitle-2 text-grey">{{ profile.email }}</div>

                <div class="profile-actions">
                  <v-file-input
                    density="compact"
                    variant="outlined"
                    prepend-inner-icon="mdi-camera"
                    accept="image/*"
                    label="Update photo"
                    @update:modelValue="onPhotoSelected"
                  />
                  <v-btn
                    variant="text"
                    color="error"
                    class="mt-2"
                    :disabled="!photoUrl"
                    @click="removePhoto"
                  >
                    Remove photo
                  </v-btn>
                </div>
              </div>
            </div>

            <v-divider class="my-6" />

            <section>
              <h3 class="text-subtitle-1 font-weight-bold mb-3">Change Password</h3>
              <v-form @submit.prevent="updatePassword">
                <v-text-field
                  v-model="passwordForm.current"
                  label="Current password"
                  type="password"
                  prepend-inner-icon="mdi-lock"
                  variant="outlined"
                  density="compact"
                />
                <v-text-field
                  v-model="passwordForm.next"
                  label="New password"
                  type="password"
                  prepend-inner-icon="mdi-lock-reset"
                  variant="outlined"
                  density="compact"
                />
                <v-text-field
                  v-model="passwordForm.confirm"
                  label="Confirm new password"
                  type="password"
                  prepend-inner-icon="mdi-lock-check"
                  variant="outlined"
                  density="compact"
                />
                <v-btn color="primary" type="submit" :disabled="!canUpdatePassword">
                  Update password
                </v-btn>
                <v-alert
                  v-if="passwordMessage"
                  class="mt-4"
                  :type="passwordMessageType"
                  variant="tonal"
                  density="compact"
                >
                  {{ passwordMessage }}
                </v-alert>
              </v-form>
            </section>

            <v-divider class="my-6" />

            <section>
              <h3 class="text-subtitle-1 font-weight-bold mb-3">Favorites</h3>
              <v-list density="comfortable">
                <v-list-item v-for="favorite in favorites" :key="favorite.id">
                  <v-list-item-title>{{ favorite.name }}</v-list-item-title>
                  <v-list-item-subtitle class="text-caption text-grey">
                    {{ favorite.location }}
                  </v-list-item-subtitle>
                  <template #append>
                    <v-btn
                      icon
                      variant="text"
                      color="error"
                      @click="removeFavorite(favorite.id)"
                    >
                      <v-icon>mdi-trash-can-outline</v-icon>
                    </v-btn>
                  </template>
                </v-list-item>
                <v-list-item v-if="!favorites.length">
                  <v-list-item-title class="text-grey">No favorites yet</v-list-item-title>
                </v-list-item>
              </v-list>
            </section>

            <v-divider class="my-6" />

            <section>
              <h3 class="text-subtitle-1 font-weight-bold mb-3">Shared Passwords</h3>
              <v-list density="comfortable">
                <v-list-item v-for="entry in sharedPasswords" :key="entry.id">
                  <v-list-item-title>{{ entry.wifiName }}</v-list-item-title>
                  <v-list-item-subtitle class="text-caption text-grey">
                    {{ entry.password }}
                  </v-list-item-subtitle>
                  <template #append>
                    <v-btn icon variant="text" color="error" @click="removeSharedPassword(entry.id)">
                      <v-icon>mdi-trash-can-outline</v-icon>
                    </v-btn>
                  </template>
                </v-list-item>
                <v-list-item v-if="!sharedPasswords.length">
                  <v-list-item-title class="text-grey">No shared passwords yet</v-list-item-title>
                </v-list-item>
              </v-list>
            </section>

            <v-divider class="my-6" />

            <section>
              <h3 class="text-subtitle-1 font-weight-bold mb-3">Your Reviews</h3>
              <v-list density="comfortable">
                <v-list-item v-for="review in reviews" :key="review.id">
                  <div class="review-row">
                    <div>
                      <div class="font-weight-medium">{{ review.wifiName }}</div>
                      <v-rating v-model="review.rating" readonly color="amber" size="18" />
                      <div class="text-body-2">{{ review.comment }}</div>
                      <div class="text-caption text-grey">{{ formatDate(review.date) }}</div>
                    </div>
                    <v-btn icon variant="text" color="error" @click="removeReview(review.id)">
                      <v-icon>mdi-trash-can-outline</v-icon>
                    </v-btn>
                  </div>
                </v-list-item>
                <v-list-item v-if="!reviews.length">
                  <v-list-item-title class="text-grey">No reviews yet</v-list-item-title>
                </v-list-item>
              </v-list>
            </section>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { computed, onBeforeUnmount, ref } from 'vue'

const profile = {
  name: 'Jonas Varnas',
  email: 'jonas@gmail.com',
}

const photoUrl = ref(null)
let photoObjectUrl = null

const favorites = ref([
  { id: 'wifi001', name: 'City Hotels Algirdas', location: 'Kaunas, Algirdo g. 24' },
  { id: 'wifi789', name: 'Tech Park', location: 'Vilnius, Antakalnio g. 12' },
  { id: 'wifi456', name: 'Central Library', location: 'Kaunas, Laisves al. 57' },
])

const sharedPasswords = ref([
  { id: 'sp-1', wifiName: 'Cafe Street', password: 'cafe123' },
  { id: 'sp-2', wifiName: 'Public Square', password: 'public456' },
])

const reviews = ref([
  {
    id: 'rv-1',
    wifiName: 'City Hotels Algirdas',
    rating: 4,
    comment: 'Fast and stable connection.',
    date: new Date().toISOString(),
  },
  {
    id: 'rv-2',
    wifiName: 'Tech Park',
    rating: 3,
    comment: 'Crowded, but okay.',
    date: new Date(Date.now() - 86400000).toISOString(),
  },
])

const passwordForm = ref({
  current: '',
  next: '',
  confirm: '',
})

const passwordMessage = ref('')
const passwordMessageType = ref('success')

const canUpdatePassword = computed(() => {
  return (
    passwordForm.value.current.trim() !== '' &&
    passwordForm.value.next.trim() !== '' &&
    passwordForm.value.next === passwordForm.value.confirm
  )
})

const onPhotoSelected = (files) => {
  const file = Array.isArray(files) ? files[0] : files
  if (!file) return

  if (photoObjectUrl) {
    URL.revokeObjectURL(photoObjectUrl)
  }

  photoObjectUrl = URL.createObjectURL(file)
  photoUrl.value = photoObjectUrl
}

const removePhoto = () => {
  if (photoObjectUrl) {
    URL.revokeObjectURL(photoObjectUrl)
    photoObjectUrl = null
  }
  photoUrl.value = null
}

const updatePassword = () => {
  if (!canUpdatePassword.value) {
    passwordMessageType.value = 'error'
    passwordMessage.value = 'Please fill in all fields and confirm the new password.'
    return
  }

  passwordMessageType.value = 'success'
  passwordMessage.value = 'Password updated locally. Connect this to the API when ready.'
  passwordForm.value = { current: '', next: '', confirm: '' }
}

const removeFavorite = (id) => {
  favorites.value = favorites.value.filter((item) => item.id !== id)
}

const removeSharedPassword = (id) => {
  sharedPasswords.value = sharedPasswords.value.filter((item) => item.id !== id)
}

const removeReview = (id) => {
  reviews.value = reviews.value.filter((item) => item.id !== id)
}

const formatDate = (iso) =>
  new Date(iso).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })

onBeforeUnmount(() => {
  if (photoObjectUrl) {
    URL.revokeObjectURL(photoObjectUrl)
  }
})
</script>

<style scoped>
.profile-header {
  display: flex;
  flex-wrap: wrap;
  gap: 24px;
  align-items: center;
}

.profile-avatar {
  background: rgba(63, 81, 181, 0.12);
}

.profile-meta {
  flex: 1;
  min-width: 240px;
}

.profile-actions {
  margin-top: 16px;
  max-width: 360px;
}

.review-row {
  width: 100%;
  display: flex;
  justify-content: space-between;
  gap: 16px;
}
</style>
