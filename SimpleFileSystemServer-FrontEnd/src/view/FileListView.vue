<script setup lang="ts">
import { ref } from "vue";
import FileList from "./../components/FileList.vue";
import { FileVO, JsonResponse } from "./../model/index";
import axios from "axios";
import { Store } from "./../store/Index";
import { message } from "ant-design-vue";

const files = ref<FileVO[]>();
const currentDir = ref("/");
// 每一项都包含全路径
const currentDirItems = ref(["/"]);

// 获取文件列表数据
const getFileListData = (path: string): Promise<FileVO[]> => {
  return new Promise(async (resolve, reject) => {
    try {
      path = path.replace(/^\/+/, "");
      const data = (
        await axios.get(`${Store.GlobalUrl}/list`, {
          params: {
            dir: path,
          },
        })
      ).data as JsonResponse<FileVO[]>;
      if (data.code == 200) {
        return resolve(data.data);
      }
      return reject(null);
    } catch (error) {
      console.error(error);
    }
  });
};
/**
 * 改变当前目录
 * @param path 路径
 * @param loadIfEqual 如果 path 和当前目录相同，是否加载
 */
const changeCurrentDirectory = (path: string, loadIfEqual = false) => {
  if (!loadIfEqual && currentDir.value == path) {
    return;
  }
  getFileListData(path)
    .then((data) => {
      files.value = data;
      currentDir.value = path;
      const dirs = currentDir.value
        .split("/")
        .filter((val) => val.trim() != "");
      const dirFullPaths = dirs.length > 0 ? [dirs[0]] : [];
      for (let i = 1; i < dirs.length; i++) {
        dirFullPaths[i] = `${dirFullPaths[i - 1]}/${dirs[i]}`;
      }
      currentDirItems.value = dirFullPaths;
    })
    .catch((err) => {
      message.error("加载数据失败");
    });
};
// 改变文件夹
const onItemClick = (item: FileVO) => {
  const targetPath = `${item.parentPath}/${item.name}`.replace(
    new RegExp("//+"),
    "/"
  );
  // 点击文件下载
  if (!item.isDir) {
    window.open(`${Store.GlobalUrl}/download?path=${encodeURIComponent(targetPath)}`)
    return;
  }
  changeCurrentDirectory(targetPath);
};
// currentDirItemsConvert，获取最后一个目录
const currentDirItemsConvert = (path: string) => {
  const arr = path.split("/");
  return arr.length > 0 ? arr[arr.length - 1] : "";
};
// 加载初始数据
changeCurrentDirectory("/", true);
</script>

<template>
  <div class="file-list-view-root">
    <a-breadcrumb class="dir-navigation-root">
      <a-breadcrumb-item @click="changeCurrentDirectory('/')">Home</a-breadcrumb-item>
      <a-breadcrumb-item @click="changeCurrentDirectory(item)" v-for="item in currentDirItems">{{
          currentDirItemsConvert(item)
      }}</a-breadcrumb-item>
    </a-breadcrumb>
    <FileList class="file-list" @on-item-click="onItemClick" :files="files" />
    <div class="file-list-description">共 {{ files?.length }} 个文件</div>
  </div>
</template>

<style scoped>
.file-list-description {
  padding: 10px 2em;
}

.file-list-view-root {
  height: 100%;
  width: 100%;
  /* padding: 0 2em; */
  padding-bottom: 0;
  display: flex;
  flex-direction: column;
}

.dir-navigation-root {
  padding: 2em;
}

.dir-navigation-root>*:hover {
  color: goldenrod;
  cursor: pointer;
}

.file-list {
  width: 100%;
  height: 100%;
  overflow: scroll;
  overflow-x: hidden;
}
</style>
