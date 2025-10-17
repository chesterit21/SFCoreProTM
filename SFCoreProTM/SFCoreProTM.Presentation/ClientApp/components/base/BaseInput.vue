<template>
  <div class="flex flex-col gap-1" :class="{ 'w-full': fullWidth }">
    <label v-if="label" :for="inputId" class="font-medium text-sm text-gray-700">
      {{ label }}
    </label>

    <InputText
      v-bind="$attrs"
      v-model="modelValueProxy"
      :type="type"
      :id="inputId"
      :class="[
        'p-inputtext-sm',m-2,
        { 'p-invalid': invalid },
        { 'w-full': fullWidth }
      ]"
      :disabled="disabled"
      :readonly="readonly"
      :placeholder="placeholder"
    />

    <slot name="error">
      <small v-if="invalid && errorMessage" class="text-red-500 text-xs">
        {{ errorMessage }}
      </small>
    </slot>
  </div>
</template>

<script setup>
import { computed, useAttrs } from 'vue'
import InputText from 'primevue/inputtext'

const props = defineProps({
  modelValue: [String, Number],
  label: String,
  type: {
    type: String,
    default: 'text',
  },
  inputId: {
    type: String,
    default: () => `input-${Math.random().toString(36).substr(2, 9)}`
  },
  placeholder: String,
  disabled: Boolean,
  readonly: Boolean,
  fullWidth: {
    type: Boolean,
    default: false,
  },
  invalid: Boolean,
  errorMessage: String
})

const emits = defineEmits(['update:modelValue'])

const modelValueProxy = computed({
  get: () => props.modelValue,
  set: (val) => emits('update:modelValue', val)
})
</script>