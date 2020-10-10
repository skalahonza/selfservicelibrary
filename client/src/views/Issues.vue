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
  name: "Issues",
  data() {
    return {
      fields: [
        {
          key: "book",
          label: "Book Name",
          sortable: true
        },
        {
          key: "isbn",
          label: "ISBN",
          sortable: true
        },
        {
          key: "issueTo",
          label: "Issue To",
          sortable: true
        },
        {
          key: "issueDate",
          label: "Issue date",
          sortable: true
        },
        {
          key: "expiryDate",
          label: "Expiry Date",
          sortable: true
        },
        {
          key: "returnDate",
          label: "Return Date",
          sortable: true
        },
        {
          key: "status",
          label: "Status",
          sortable: true
        }
      ],
      items: [
        {
          book: "My first Book",
          isbn: "978-3-16-148410-0",
          issueTo: "John Doe",
          issueDate: "2019-06-06",
          expiryDate: "2020-06-06",
          returnDate: "2020-05-05",
          status: "Returned"
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
