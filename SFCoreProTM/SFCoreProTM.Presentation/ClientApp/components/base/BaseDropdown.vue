<template>
  <div class="flex flex-col gap-1 w-full">
    <label v-if="label" :for="id" class="font-medium text-sm text-gray-700">
      {{ label }}
    </label>

    <Select 
      :id="id"
      v-model="modelValueProxy"
      :options="options"
      :optionLabel="optionLabel"
      :optionValue="optionValue"
      :placeholder="placeholder"
      :disabled="disabled"
      :filter="filter"
      :class="[{ 'p-invalid': invalid }, dropdownClass]"
      class="w-full"
      :data-testid="testid"
    />

    <!-- Error Slot -->
    <slot name="error" />

  </div>
</template>

<script setup>
import { computed, defineProps, defineEmits } from 'vue'
import Select from 'primevue/select'

const props = defineProps({
  modelValue: [String, Number, Object, null],
  options: {
    type: Array,
    required: true
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
    default: '-- Pilih --'
  },
  label: String,
  id: String,
  disabled: Boolean,
  filter: Boolean,
  invalid: Boolean,
  dropdownClass: {
    type: String,
    default: ''
  },
  testid: String
})

const emit = defineEmits(['update:modelValue'])

const modelValueProxy = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})
</script>

<style scoped>
.p-invalid {
  border: 1px solid #e24c4c;
}
</style>