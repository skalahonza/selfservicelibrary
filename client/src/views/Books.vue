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
      <template slot="action" slot-scope="data">
        <b-button v-on:click="shouldRemove(data.item.email)">Remove</b-button>
      </template>
    </b-table>
  </div>
</template>

<script>
export default {
  name: "Books",
  data() {
    return {
      fields: [
        {
          key: "name",
          label: "Book Name",
          sortable: true
        },
        {
          key: "isbn",
          label: "ISBN",
          sortable: true
        },
        {
          key: "quantity",
          label: "Quantity",
          sortable: true
        },
        {
          key: "issued",
          label: "Issued",
          sortable: true
        }
      ],
      items: [
        {
          name: "My first Book",
          isbn: "978-3-16-148410-0",
          quantity: 20,
          issued: 10
        }
      ],
      currentPage: 0,
      perPage: 100,
      pageSizes: [100],
      emptyText: "No data",
      sortBy: "",
      isBusy: false,
      search: "",
      selectMode: "single"
    };
  }
};
</script>
