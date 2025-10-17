<template>
  <div class="p-card surface-card p-4 border-round">
    <DataTable
      :value="items"
      :lazy="true"
      :loading="loading"
      :totalRecords="totalRecords"
      :rows="rowsPerPage"
      :first="first"
      :sortField="sortField"
      :sortOrder="sortOrder"
      paginator
      responsiveLayout="scroll"
      class="p-datatable-gridlines p-datatable-hoverable-rows"
      @page="onPageChange"
      @sort="onSortChange"
      :size="size.value" 
      rowHover
    >
      <!-- Dynamic Columns -->
      <Column
        v-for="col in columns"
        :key="col.field"
        :field="col.field"
        :header="col.header"
        :sortable="col.sortable ?? true"
      />

      <!-- Action Buttons -->
      <Column header="Actions" headerStyle="text-align:center;" bodyStyle="text-align:center;">
        <template #body="{ data }">
          <Button
            icon="pi pi-pencil"
            class="p-button-sm p-button-text p-button-info"
            @click="emit('edit', data.id)"
          />
          <Button
            icon="pi pi-trash"
            class="p-button-sm p-button-text p-button-danger"
            @click="emit('delete', data.id)"
          />
        </template>
      </Column>
    </DataTable>
    <!-- Info Baris Ditampilkan -->
    <div class="mt-2 text-sm text-gray-600 text-right">
      Menampilkan
      <b>{{ formatNumberID(startRow) }}</b>
      –
      <b>{{ formatNumberID(endRow) }}</b>
      dari
      <b>{{ formatNumberID(totalRecords) }}</b>
      data
    </div>
</div>
</template>

<script setup>
import { ref, computed } from 'vue';
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Paginator from 'primevue/paginator'
import Button from 'primevue/button'
import ColumnGroup from 'primevue/columngroup';   // optional
import Row from 'primevue/row';                   // optional

const props = defineProps({
  columns: Array,
  items: Array,
  totalRecords: { type: Number, default: 0 },
  loading: Boolean,
  rowsPerPage: { type: Number, default: 10 },
  first: { type: Number, default: 0 },
  sortField: String,
  sortOrder: Number,
  rows:{ type: Number, default: 0 },
})

const emit = defineEmits(['edit', 'delete', 'page-change', 'sort-change'])
const size = ref({ label: 'Small', value: 'small' });

const startRow = computed(() => props.first + 1);
const endRow = computed(() => Math.min(props.first + props.rowsPerPage, props.totalRecords));

function onPageChange(event) {
  emit('page-change', {
    page: event.page,
    rows: event.rows
  })
}

function onSortChange(event) {
  emit('sort-change', {
    sortField: event.sortField,
    sortOrder: event.sortOrder
  })
}
function formatNumberID(num) {
  return num.toLocaleString('id-ID'); // gunakan 'id-ID' biar pakai titik
}

</script>

<style scoped>
.p-card {
  box-shadow: 0 1px 6px rgba(0, 0, 0, 0.05);
}

/* Optional hover style */
.p-datatable-hoverable-rows .p-selectable-row:hover, .p-datatable-tbody > tr :hover .p-datatable-tbody > tr >td :hover{
  background-color: #f0f9ff; /* light blue hover */
  cursor: pointer;
}
/* Khusus baris yang bisa dihover */
.p-datatable-hoverable-rows .p-datatable-tbody > tr:hover {
  background-color: #f0f9ff;
  cursor: pointer;
}
</style>