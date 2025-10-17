<template>
  <BaseButton
    :label="label"
    :icon="icon"
    :type="type"
    :disabled="disabled"
    :loading="loading"
    :severity="severity"
    :tooltip="tooltip || 'Export data ke Excel'"
    :tooltipOptions="{ position: 'top' }"
    @click="$emit('click', filename)"
  />
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'
import BaseButton from '../BaseButton.vue'

const props = defineProps({
  label: {
    type: String,
    default: 'Export Excel'
  },
  icon: {
    type: String,
    default: 'pi pi-file-excel'
  },
  type: {
    type: String,
    default: 'button'
  },
  severity: {
    type: String,
    default: 'success'
  },
  filename: {
    type: String,
    default: 'data.xlsx'
  },
  tooltip: String,
  loading: Boolean,
  disabled: Boolean
})

defineEmits(['click'])
</script>