<template>
  <BaseButton
    :label="label"
    :icon="icon"
    :type="type"
    :disabled="disabled"
    :loading="loading"
    :severity="severity"
    :tooltip="tooltip || 'Export data ke PDF'"
    :tooltipOptions="{ position: 'top' }"
    @click="$emit('click', $event)"
  />
</template>

<script setup>
import BaseButton from '../BaseButton.vue'
import { defineProps, defineEmits } from 'vue'

const props = defineProps({
  label: {
    type: String,
    default: 'Export PDF'
  },
  icon: {
    type: String,
    default: 'pi pi-file-pdf'
  },
  type: {
    type: String,
    default: 'button'
  },
  severity: {
    type: String,
    default: 'danger'
  },
  tooltip: String,
  loading: Boolean,
  disabled: Boolean
})

defineEmits(['click'])
</script>