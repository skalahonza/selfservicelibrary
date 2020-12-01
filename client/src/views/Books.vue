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
      <template #cell(isAvailable)="row">
        <b-check v-model="row.item.isAvailable" disabled />
      </template>
      <template #cell(action)="row">
        <b-button variant="success" v-on:click="borrow(row.item)"
          >Borrow</b-button
        >
      </template>
    </b-table>
  </div>
</template>

<script>
import { getAll, borrowBook } from "@/services/books";

export default {
  name: "Books",
  created() {
    this.reload();
  },
  methods: {
    async reload() {
      this.isBusy = true;
      this.items = await getAll();
      this.isBusy = false;
    },
    borrow(item) {
      borrowBook(item.id)
        .then(() => {
          this.$bvToast.toast("Book successfully borrowed.", {
            title: "Success",
            variant: "success",
            solid: true,
          });
          this.reload();
        })
        .catch((error) => {
          this.$bvToast.toast(error.response.data.title, {
            title: "Chyba",
            variant: "danger",
            solid: true,
          });
        });
    },
  },
  data() {
    return {
      fields: [
        {
          key: "name",
          label: "Book Name",
          sortable: true,
        },
        {
          key: "isbn",
          label: "ISBN",
          sortable: true,
        },
        {
          key: "quantity",
          label: "Quantity",
          sortable: true,
        },
        {
          key: "issued",
          label: "Issued",
          sortable: true,
        },
        {
          key: "isAvailable",
          label: "Is Available",
          sortable: true,
        },
        { key: "action" },
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
