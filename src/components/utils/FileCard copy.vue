<template>
    <template v-if="isImage(props.url)">
        <img :src="props.url" alt="image" class="chat-image" />
    </template>

    <template v-else-if="isDocument(props.url)">
        <a :href="props.url" class="file-card">
            <div class="file-info">
                <div class="file-name">{{ parseFileContent(props.fileContent).fileName }}</div>
                <div class="file-size">{{ parseFileContent(props.fileContent).fileSize }}</div>
            </div>
            <img :src="getIcon(props.url)" class="file-icon" />
        </a>
    </template>

</template>

<script setup>
import { defineProps } from 'vue';

const props = defineProps(["url", "fileContent"]);

function isImage(url) {
    return /\.(png|jpe?g|gif|bmp|webp)$/i.test(url);
}

function isDocument(url) {
    return /\.(docx?|pptx?|xlsx?|pdf)$/i.test(url);
}

function getIcon(url) {
    if (/\.docx?$/.test(url)) return new URL('@/assets/icons/doc.png', import.meta.url).href;
    if (/\.pptx?$/.test(url)) return new URL('@/assets/icons/ppt.png', import.meta.url).href;
    if (/\.xlsx?$/.test(url)) return new URL('@/assets/icons/xls.png', import.meta.url).href;
    if (/\.pdf$/.test(url)) return new URL('@/assets/icons/pdf.png', import.meta.url).href;
    return new URL('@/assets/icons/file.png', import.meta.url).href;
}

function parseFileContent(fileContent) {
  const match = fileContent.match(/^(.*)-\(([^)]+)\)$/);

  if (!match) {
    // 格式不符合预期，返回原始字符串为 fileName，fileSize 为 null
    return {
      fileName: fileContent,
      fileSize: null
    };
  }

  const fileName = match[1];
  const fileSize = match[2];

  return { fileName, fileSize };
}

</script>

<style scoped>
.chat-image {
    max-width: 200px;
    border-radius: 8px;
}

.file-card {
    display: flex;
    align-items: center;
    gap: 8px;
}

.file-icon {
    width: 32px;
    height: 32px;
}

.file-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  text-decoration: none;
  background-color: #f3f3f3;
  border-radius: 8px;
  padding: 12px;
  width: 250px;
  color: #333;
  transition: background-color 0.2s ease;
}

.file-card:hover {
  background-color: #e0e0e0;
}

.file-info {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  flex: 1;
  overflow: hidden;
}

.file-name {
  font-size: 14px;
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.file-size {
  font-size: 12px;
  color: #999;
  margin-top: 6px;
}

.file-icon {
  width: 40px;
  height: 40px;
  margin-left: 12px;
  flex-shrink: 0;
}

</style>
