<template>
  <div>
    <!-- Upload trigger -->
    <BaseButton
      :label="label"
      :icon="icon"
      :severity="severity"
      :disabled="disabled"
      :loading="loading"
      :tooltip="tooltip || defaultTooltip"
      :tooltipOptions="{ position: 'top' }"
      @click="triggerFile"
    />

    <!-- Hidden input -->
    <input
      ref="fileInput"
      type="file"
      class="hidden"
      :accept="accept"
      :multiple="multiple"
      @change="onFileChange"
    />

    <!-- Single file preview -->
    <div v-if="files.length === 1" class="mt-3">
      <div v-if="isImage(files[0])">
        <img
          :src="filePreviewUrls[0]"
          class="w-32 h-auto border rounded shadow"
        />
      </div>
      <div v-else class="flex items-center gap-2">
        <i :class="getIcon(files[0])"></i>
        <span>{{ files[0].name }}</span>
      </div>
    </div>

    <!-- Multiple file preview -->
    <DataTable
      v-if="files.length > 1"
      :value="files"
      class="mt-4"
      responsiveLayout="scroll"
      stripedRows
    >
      <Column field="name" header="Nama File" />
      <Column header="Preview">
        <template #body="{ data, index }">
          <img
            v-if="isImage(data)"
            :src="filePreviewUrls[index]"
            class="w-12 h-auto border rounded"
          />
          <i v-else :class="getIcon(data)" style="font-size: 1.2rem;"></i>
        </template>
      </Column>
      <Column header="Aksi" style="width: 80px;">
        <template #body="{ index }">
          <Button
            icon="pi pi-trash"
            severity="danger"
            text
            @click="removeFile(index)"
          />
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<script setup>
import { ref, computed, defineProps, defineEmits } from 'vue'
import BaseButton from '../BaseButton.vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'

const props = defineProps({
  label: {
    type: String,
    default: 'Upload'
  },
  icon: {
    type: String,
    default: 'pi pi-upload'
  },
  severity: {
    type: String,
    default: 'info'
  },
  accept: {
    type: String,
    default: '*'
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
const files = ref([])
const filePreviewUrls = ref([])

const defaultTooltip = computed(() =>
  props.multiple ? 'Pilih beberapa file untuk diunggah' : 'Pilih file untuk diunggah'
)

const triggerFile = () => {
  if (!props.disabled) {
    fileInput.value?.click()
  }
}

const onFileChange = (event) => {
  const selected = Array.from(event.target.files)
  files.value = selected
  filePreviewUrls.value = selected.map((file) =>
    isImage(file) ? URL.createObjectURL(file) : null
  )
  emit('select', selected)
  event.target.value = ''
}

const isImage = (file) => file.type.startsWith('image/')

const getIcon = (file) => {
  const ext = file.name.split('.').pop().toLowerCase()
  const iconMap = {
    pdf: 'pi pi-file-pdf',
    doc: 'pi pi-file-word',
    docx: 'pi pi-file-word',
    xls: 'pi pi-file-excel',
    xlsx: 'pi pi-file-excel',
    ppt: 'pi pi-slideshare',
    pptx: 'pi pi-slideshare',
    csv: 'pi pi-file',
    json: 'pi pi-code',
    rtf: 'pi pi-file',
    txt: 'pi pi-file',
    zip: 'pi pi-file',
  }
  return iconMap[ext] || 'pi pi-file'
}

const removeFile = (index) => {
  files.value.splice(index, 1)
  filePreviewUrls.value.splice(index, 1)
  emit('select', [...files.value]) // emit ulang
}
</script>

<style scoped>
img {
  object-fit: contain;
}
</style>