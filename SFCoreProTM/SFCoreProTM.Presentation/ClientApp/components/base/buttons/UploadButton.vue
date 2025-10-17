<!-- ClientApp/components/base/buttons/UploadButton.vue -->
<template>
  <div class="relative inline-block">
    <BaseButton
      :label="label"
      :icon="icon"
      :type="type"
      :loading="loading"
      :disabled="disabled"
      :severity="severity"
      :tooltip="tooltip || 'Pilih file untuk diunggah'"
      :tooltipOptions="{ position: 'top' }"
      @click="triggerFileSelect"
    />
    <input
      ref="fileInput"
      type="file"
      :accept="accept"
      :multiple="multiple"
      class="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
      @change="handleFileChange"
    />
  </div>
</template>

<script setup>
import { ref, defineProps, defineEmits } from 'vue'
import BaseButton from '../BaseButton.vue'

const props = defineProps({
  label: {
    type: String,
    default: 'Upload'
  },
  icon: {
    type: String,
    default: 'pi pi-upload'
  },
  type: {
    type: String,
    default: 'button'
  },
  severity: {
    type: String,
    default: 'info'
  },
  accept: {
    type: String,
    default: '*' // bisa di-set 'image/*', '.pdf', dll
  },
  multiple: {
    type: Boolean,
    default: false
  },
  tooltip: String,
  loading: Boolean,
  disabled: Boolean
})

const emit = defineEmits(['select'])

const fileInput = ref(null)

const triggerFileSelect = () => {
  if (!props.disabled) {
    fileInput.value?.click()
  }
}

const handleFileChange = (e) => {
  const files = e.target.files
  if (files && files.length > 0) {
    emit('select', props.multiple ? [...files] : files[0])
    e.target.value = '' // reset agar bisa pilih file sama dua kali
  }
}
</script>