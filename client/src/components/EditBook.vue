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

      <b-form-group label="Quantity:">
        <b-form-input
          type="number"          
          v-model="book.quantity"
        ></b-form-input>
      </b-form-group>

      <b-button type="submit" block variant="success">Save</b-button>
    </b-form>
  </div>
</template>

<script>
import { editBook } from "@/services/books";
export default {
  props: {
    book: Object,
    onSuccess: Function,
  },
  methods: {
    onSubmit() {
      editBook(this.book)
        .then(() => {
          this.$bvToast.toast("Book successfully edited.", {
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