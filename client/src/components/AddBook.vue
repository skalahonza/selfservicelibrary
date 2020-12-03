<template>
  <div>
    <b-form @submit="onSubmit">
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
    </b-form>
  </div>
</template>

<script>
export default {
  props: {
    book: Object,
    onSuccess: Function,
  },
  methods: {
    onSubmit() {
      console.log(this.book);
      if (this.onSuccess) this.onSuccess();
    },
  },
  data() {
    return {};
  },
};
</script>