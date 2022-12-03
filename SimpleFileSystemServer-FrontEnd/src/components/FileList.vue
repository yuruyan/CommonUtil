<script setup lang="ts">
import { FileVO } from "../model/index";
import FileListItem from "./FileListItem.vue";
const props = defineProps({
  files: Array<FileVO>,
});
const emits = defineEmits(["onItemClick"]);

// 点击 item 事件
const onItemClick = (item: FileVO) => emits("onItemClick", item);
</script>

<template>
  <a-list item-layout="horizontal" :data-source="files">
    <template #renderItem="{ item }">
      <a-list-item @click="onItemClick(item)" class="file-list-item-parent">
        <FileListItem class="file-list-item" :file-item="item" />
      </a-list-item>
    </template>
  </a-list>
</template>

<style scoped>
.file-list-item {
  width: 100%;
  padding: 0 1em;
}

.file-list-item-parent:hover {
  background-color: gainsboro;
  cursor: pointer;
}
</style>
