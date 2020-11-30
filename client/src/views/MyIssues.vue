<template>
  <div>
    <b-table
      hover
      selectable
      :current-page="currentPage"
      :per-page="perPage"
      :sort-by.sync="sortBy"
      :fields="fields"
      :items="items"
      :busy="isBusy"
      :select-mode="selectMode"
    >
      <div slot="table-busy" class="text-center text-primary my-2">
        <b-spinner class="align-middle"></b-spinner>
        <strong>Loading...</strong>
      </div>
      <template #cell(isReturned)="row">
        <b-check v-model="row.isReturned" disabled />
      </template>
    </b-table>
  </div>
</template>

<script>
import { getMine } from "@/services/issues";
import moment from 'moment';
export default {
  name: "MyIssues",
  async created() {
    this.isBusy = true;
    this.items = await getMine();
    this.isBusy = false;
  },
  methods: {
    formatDateAssigned(value) {
      if (value) {
        return moment(value).format('LL');
      }
      return value;
    },
  },
  data() {
    return {
      fields: [
        {
          key: "bookName",
          label: "Book Name",
          sortable: true,
        },
        {
          key: "isbn",
          label: "ISBN",
          sortable: true,
        },
        {
          key: "issueDate",
          label: "Issue date",
          sortable: true,
          formatter: "formatDateAssigned",
        },
        {
          key: "expiryDate",
          label: "Expiry Date",
          sortable: true,
          formatter: "formatDateAssigned",
        },
        {
          key: "returnDate",
          label: "Return Date",
          sortable: true,
          formatter: "formatDateAssigned",
        },
        {
          key: "isReturned",
          label: "Reutrned",
          sortable: true,
        },
      ],
      items: [],
      currentPage: 0,
      perPage: 100,
      pageSizes: [100],
      emptyText: "No data",
      sortBy: "",
      isBusy: false,
      search: "",
      selectMode: "single",
    };
  },
};
</script>
