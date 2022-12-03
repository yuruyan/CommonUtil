import { createApp } from "vue";
import App from "./App.vue";
import { List, Breadcrumb, message } from "ant-design-vue";
import { Store } from "./store/Index";
import "./scripts/iconfont.js";
import "./style/base.css";
import "ant-design-vue/dist/antd.css";

// 配置 Store
Store.GlobalUrl = document.getElementById("config")!.innerText;

const app = createApp(App);
app.use(List).use(Breadcrumb).mount("#app");
app.config.globalProperties.$message = message;
