import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import Books from "../views/Books.vue";
import Issues from "../views/Issues.vue";
import MyIssues from "../views/MyIssues.vue";
import Login from "../components/Login.vue";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home
  },
  {
    path: "/login",
    name: "Login",
    component: Login
  },
  {
    path: "/books",
    name: "Books",
    component: Books
  },
  {
    path: "/issues",
    name: "All Issues",
    component: Issues
  },
  {
    path: "/my-issues",
    name: "My issues",
    component: MyIssues
  }
];

const router = new VueRouter({
  mode: "history",
  routes: routes
});

export default router;
