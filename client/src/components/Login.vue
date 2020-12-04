<template>
  <div>
    <b-jumbotron>
      <template #header>Self Service Library</template>

      <template #lead>
        Online Self service library for university departments.
      </template>

      <hr class="my-4" />

      <div v-if="authorized">
        <p>Signed in as {{ user.fullName }}</p>
        <b-button @click="$router.push('books')" pill variant="primary"
          >View books</b-button
        >
        <b-button @click="$router.push('my-issues')" pill variant="success"
          >View issues</b-button
        >
      </div>

      <b-form @submit="onSubmit" v-else>
        <p>
          You can begin by logging in with your university credentials. We use
          official
          <b-link href="https://auth.fit.cvut.cz/" target="_blank"
            >CVUT authorization server</b-link
          >. We do not store your password or user info anywhere.
        </p>
        <b-overlay
          :show="has_code && !authorized"
          rounded="sm"
          bg-color="#e9ecef"
          spinner-variant="primary"
          spinner-type="grow"
          spinner-small
        >
          <b-button type="submit" variant="primary" pill
            >Login with CVUT</b-button
          >
        </b-overlay>
      </b-form>
    </b-jumbotron>
  </div>
</template>

<script>
import { signIn, isAuthorized } from "@/services/auth.js";
import { parseCode, removeCode } from "@/services/utils.js";

export default {
  computed: {
    authorized: function () {
      return isAuthorized();
    },
    user: function () {
      return {
        preferredEmail: localStorage.getItem("preferredEmail"),
        fullName: localStorage.getItem("fullName"),
      };
    },
    code: function () {
      // HACK parse query parameter even from github pages
      return parseCode() || this.$route.query.code;
    },
    has_code: function () {
      // HACK parse query parameter even from github pages
      const code = this.code;
      // string to bool
      if (code) return true;
      else return false;
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
      if (this.has_code && !this.authorized) {
        await signIn(this.code);
      }
      // HACK remove code query parameter that is misplaced due to non history mode of router
      removeCode();
    });
  },
};
</script>
