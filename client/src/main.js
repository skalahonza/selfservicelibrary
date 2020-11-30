import Vue from "vue";
import {
  BootstrapVue,
  IconsPlugin
} from "bootstrap-vue";
import "bootstrap/dist/css/bootstrap.css";
import "bootstrap-vue/dist/bootstrap-vue.css";
import App from "./App.vue";
import router from "./router";
import api from "./services/api";
import {
  isExpired,
  refresh
} from "./services/auth";

Vue.config.productionTip = false;
Vue.prototype.$http = api;

// Install BootstrapVue
Vue.use(BootstrapVue);
// Install the BootstrapVue icon components plugin
Vue.use(IconsPlugin);

// attach Bearer token
api.interceptors.request.use(
  config => {
    const token = localStorage.getItem("access_token");
    if (token && token != undefined) {
      config.headers.common["Authorization"] = "Bearer " + token;
    }
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

// token refresher
api.interceptors.request.use(
  async config => {
    if (isExpired()) {
      if (!await refresh()) {
        router.replace({
          path: "/"
        });
      }
    }
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

new Vue({
  router,
  render: h => h(App)
}).$mount("#app");