<template>
  <div class="base-table-client">
<DataTable
  :value="paginatedData"
  :lazy="true"
  :paginator="true"
  :rows="rows"
  :first="first"
  :totalRecords="props.data.length"
  @page="onPage"
  @sort="onSort"
  :stripedRows="true"
  responsiveLayout="scroll"
  class="w-full"
>

      <Column
        v-for="(col, index) in columns"
        :key="col.field || index"
        :field="col.field"
        :header="col.header"
        :sortable="col.sortable"
      />
    </DataTable>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'

const props = defineProps({
  data: {
    type: Array,
    required: true
  },
  columns: {
    type: Array,
    required: true
  },
  rows: {
    type: Number,
    default: 10
  }
})

const first = ref(0)
const sortField = ref(null)
const sortOrder = ref(null)

watch(() => props.data, () => {
  first.value = 0
})

const paginatedData = computed(() => {
  let sorted = [...props.data]

  if (sortField.value) {
    sorted.sort((a, b) => {
      const valA = a[sortField.value]
      const valB = b[sortField.value]
      const result = valA > valB ? 1 : valA < valB ? -1 : 0
      return sortOrder.value === 1 ? result : -result
    })
  }

  return sorted.slice(first.value, first.value + props.rows)
})


function onPage(event) {
    first.value = event.first
}

function onSort(event) {
  sortField.value = event.sortField
  sortOrder.value = event.sortOrder
  first.value = 0
}
</script>

<style scoped>
.base-table-client {
  width: 100%;
}
</style>