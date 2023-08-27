import { createApp } from "vue";
import App from "./App.vue";
import { List, Breadcrumb, message, Upload, Button } from "ant-design-vue";
import "./scripts/iconfont.js";
import "./style/base.css";
import "ant-design-vue/dist/antd.css";

const app = createApp(App);
app.use(List).use(Breadcrumb).use(Upload).use(Button).mount("#app");
app.config.globalProperties.$message = message;
