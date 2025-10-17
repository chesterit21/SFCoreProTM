<!-- ClientApp/components/base/buttons/SaveButton.vue -->
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
    default: 'Simpan'
  },
  icon: {
    type: String,
    default: 'pi pi-save'
  },
  type: {
    type: String,
    default: 'submit'
  },
  severity: {
    type: String,
    default: 'primary'
  },
  loading: Boolean,
  disabled: Boolean
})

defineEmits(['click'])
</script>