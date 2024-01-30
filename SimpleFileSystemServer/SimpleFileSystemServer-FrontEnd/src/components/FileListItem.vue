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
    return `${size} B`;
  }
  if (size < 1024 * 1024) {
    return `${(size / 1024).toFixed(1)} KB`;
  }
  if (size < 1024 * 1024 * 1024) {
    return `${(size / 1024 / 1024).toFixed(1)} MB`;
  }
  return `${(size / 1024 / 1024 / 1024).toFixed(1)} GB`;
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
