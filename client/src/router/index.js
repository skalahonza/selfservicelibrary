import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import Books from "../views/Books.vue";
import MyIssues from "../views/MyIssues.vue";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home
  },
  {
    path: "/books",
    name: "Books",
    component: Books
  },
  {
    path: "/my-issues",
    name: "My issues",
    component: MyIssues
  }
];

const router = new VueRouter({
  routes
});

export default router;
