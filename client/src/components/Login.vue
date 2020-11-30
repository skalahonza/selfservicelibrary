<template>
  <div>
    <div>Code: {{ code }}</div>
    <b-form @submit="onSubmit" v-if="!code && hasExpired" >
      <b-button type="submit" variant="primary">Login with CVUT</b-button>
    </b-form>
  </div>
</template>

<script>
import { signIn, isExpired } from "@/services/auth.js";

export default {
  computed: {
    hasExpired: function () {
      return isExpired();
    },
    code: function () {
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
  mounted: function () {
    this.$nextTick(async function () {
      if (this.code && this.hasExpired) {
        await signIn(this.code);        
      }
      // HACK remove code query parameter that is misplaced due to non history mode of router
      const regex = /\??code=(\w*)(?=(&|#)?)/gm;
      window.location.href = window.location.href.replace(regex, "");
    });
  },
};
</script>
