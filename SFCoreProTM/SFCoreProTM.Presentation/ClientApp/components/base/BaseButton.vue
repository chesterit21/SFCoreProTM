<!-- ClientApp/components/base/BaseButton.vue -->
<script setup>
import { computed, useAttrs } from 'vue'
import Button from 'primevue/button'

// Props utama
defineProps({
  label: String,
  icon: String,
  iconPos: {
    type: String,
    default: 'left'
  },
  loading: Boolean,
  disabled: Boolean,
  severity: String,
  rounded: Boolean,
  outlined: Boolean,
  text: Boolean
})

const computedClass = computed(() => {
  return [
    'p-button',  // class dasar PrimeVue
    !label ? 'p-button-icon-only' : '', // class khusus untuk tombol icon-only
  ]
})

// Ambil semua atribut & event listener dari parent
const attrs = useAttrs()
</script>

<template>
  <Button
    v-bind="attrs"
    :label="label"
    :icon="icon"
    :iconPos="iconPos"
    :loading="loading"
    :disabled="disabled"
    :severity="severity"
    :rounded="rounded"
    :outlined="outlined"
    :text="text"
    :class= "mx-3"
  />
</template>
<style scoped>
.p-button  {
  margin: 6px 3px;     /* ✅ tambahkan jarak antar tombol */
  padding: 2px 5px;    /* ✅ jika ingin ada padding juga */
  display: inline-block; /* ✅ supaya tidak full width */
}
</style>