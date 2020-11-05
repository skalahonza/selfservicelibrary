<template>
  <div>
    <div>Token: {{ token }}</div>
    <b-form @submit="onSubmit" v-if="!token">
      <b-button type="submit" variant="primary">Login with CVUT</b-button>
    </b-form>
  </div>
</template>

<script>
export default {
  computed: {
    token: function() {
      return this.$route.query.code;
    }
  },
  methods: {
    onSubmit(evt) {
      evt.preventDefault();
      //const redirect = location.href;
      const redirect = "https://enjqvdake9lq.x.pipedream.net";
      const clientId = process.env.VUE_APP_CLIENT_ID;
      window.location.href = `https://auth.fit.cvut.cz/oauth/authorize?response_type=code&client_id=${clientId}&redirect_uri=${redirect}`;
    }
  },
  mounted: function() {
      if(this.$route.query.code){
          // TODO Get token from API
      }
  }
};
</script>
