<template>
  <div>
    <b-form @submit.prevent="onSubmit">
      <b-form-group label="Book name:">
        <b-form-input
          v-model="book.name"
          required
          placeholder="Enter book name"
          invalid-feedback="Name is required"
        ></b-form-input>
      </b-form-group>

      <b-form-group label="Author name:">
        <b-form-input
          v-model="book.author"
          required
          placeholder="Enter name"
          invalid-feedback="Author is required"
        ></b-form-input>
      </b-form-group>

      <b-form-group label="ISBN:">
        <b-form-input
          v-model="book.isbn"
          required
          placeholder="978-80-01-03775-1"
          invalid-feedback="ISBN is required"
        ></b-form-input>
      </b-form-group>

      <b-form-group label="Quantity:">
        <b-form-input
          type="number"          
          v-model="book.quantity"
        ></b-form-input>
      </b-form-group>

      <b-button type="submit" block variant="success">Submit</b-button>
    </b-form>
  </div>
</template>

<script>
import { addBook } from "@/services/books";
export default {
  props: {
    book: Object,
    onSuccess: Function,
  },
  methods: {
    onSubmit() {
      addBook(this.book)
        .then(() => {
          this.$bvToast.toast("Book successfully added.", {
            title: "Success",
            variant: "success",
            solid: true,
          });
          if (this.onSuccess) this.onSuccess();
        })
        .catch((error) => {
          this.$bvToast.toast(
            error.response.data.title || error.response.data,
            {
              title: "Error",
              variant: "danger",
              solid: true,
            }
          );
        });
    },
  },
  data() {
    return {};
  },
};
</script>