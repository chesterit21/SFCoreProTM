<template>
  <div class="flex flex-col gap-1 w-full">
    <label v-if="label" :for="id" class="font-medium text-sm text-gray-700">{{ label }}</label>
    <MultiSelect
      :id="id"
      v-model="modelValueProxy"
      :options="options"
      :option-label="optionLabel"
      :option-value="optionValue"
      :placeholder="placeholder"
      :disabled="disabled"
      :filter="filter"
      class="w-full"
      :class="{ 'p-invalid': invalid }"/>
    <small v-if="invalid" class="text-red-500 text-xs">
      <slot name="error">{{ errorMessage }}</slot>
    </small>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import MultiSelect from 'primevue/multiselect'

const props = defineProps({
  modelValue: Array,
  options: {
    type: Array,
    default: () => []
  },
  optionLabel: {
    type: String,
    default: 'label'
  },
  optionValue: {
    type: String,
    default: 'value'
  },
  placeholder: {
    type: String,
    default: 'Pilih item'
  },
  label: String,
  errorMessage: String,
  invalid: Boolean,
  disabled: Boolean,
  filter: Boolean,
  id: {
    type: String,
    default: () => `multi_${Math.random().toString(36).substring(2, 9)}`
  }
})

const emit = defineEmits(['update:modelValue'])
const modelValueProxy = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})
</script>

<style scoped>
.p-invalid {
  border-color: #f87171 !important;
  box-shadow: 0 0 0 1px #f87171 !important;
}
</style>