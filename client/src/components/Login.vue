<template>
  <div>
    <div>Code: {{ code }}</div>
    <b-form @submit="onSubmit" v-if="!code">
      <b-button type="submit" variant="primary">Login with CVUT</b-button>
    </b-form>
  </div>
</template>

<script>
export default {
  computed: {
    code: function() {
      // HACK parse query parameter even from github pages
      const regex = /(?<=code=)(\w*)(?=(&|#)?)/gm;
      const found = window.location.href.match(regex);
      if (found && found.length == 1) {
        return found[0];
      }
      return this.$route.query.code;
    },
  },
  methods: {
    onSubmit(evt) {
      evt.preventDefault();
      const redirect = location.href.replace("/#/", "");
      const clientId = process.env.VUE_APP_CLIENT_ID;
      window.location.href = `https://auth.fit.cvut.cz/oauth/authorize?response_type=code&client_id=${clientId}&redirect_uri=${redirect}`;
    },
  },
  mounted: function() {
    alert(this.code());
    if (this.code()) {
      // TODO Get Token from API
      alert(this.code());
    }
  },
};
</script>
