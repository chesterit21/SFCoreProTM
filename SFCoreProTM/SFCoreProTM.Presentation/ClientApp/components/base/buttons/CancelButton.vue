<!-- ClientApp/components/base/buttons/CancelButton.vue -->
<template>
  <BaseButton
    :label="label"
    :icon="icon"
    :type="type"
    :loading="loading"
    :disabled="disabled"
    :severity="severity"
    @click="$emit('click', $event)"
  />
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'
import BaseButton from '../BaseButton.vue'

const props = defineProps({
  label: {
    type: String,
    default: 'Batal'
  },
  icon: {
    type: String,
    default: 'pi pi-times'
  },
  type: {
    type: String,
    default: 'button'
  },
  severity: {
    type: String,
    default: 'secondary'
  },
  loading: Boolean,
  disabled: Boolean
})

defineEmits(['click'])
</script>