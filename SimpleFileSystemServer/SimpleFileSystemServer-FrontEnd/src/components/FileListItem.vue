<script setup lang="ts">
import { computed } from "vue";
import { FileVO } from "../model/index";
const props = defineProps({
  fileItem: FileVO,
});

// 文件大小格式转换
const fileSizeConvert = computed(() => {
  const size = props.fileItem!.fileSize;
  if (size < 1024) {
    return `${size} b`;
  }
  if (size < 1024 * 1024) {
    return `${parseInt((size / 1024).toString())} kb`;
  }
  if (size < 1024 * 1024 * 1024) {
    return `${parseInt((size / 1024 / 1024).toString())} mb`;
  }
  return `${parseInt((size / 1024 / 1024 / 1024).toString())} gb`;
});
const fileIconConvert = computed(() => {
  return props.fileItem!.isDir ? '#icon-open' : '#icon-file'
})
</script>

<template>
  <div class="file-item-root">
    <div class="filename-icon">
      <svg class="icon" aria-hidden="true">
        <use :xlink:href="fileIconConvert"></use>
      </svg>
      <span>{{ fileItem!.name }}</span>
    </div>
    <div v-show="!fileItem!.isDir">{{ fileSizeConvert }}</div>
  </div>
</template>

<style scoped>
.filename-icon {
  display: flex;
  align-items: center;
}

.filename-icon span {
  margin-left: 10px;
}

.file-item-root {
  display: flex;
  margin: 0 1em;
  justify-content: space-between;
}
</style>
