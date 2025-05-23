<template>
    <el-icon size="25" class="sidebar-btn ink-button" style="color:white" plain @click="dialog = true">
        <Plus />
    </el-icon>


    <el-dialog :before-close="clickCancel" v-model="dialog" title="上传文件" size='80%' :with-header="false">

        <div class="upload-container">
            <file-pond ref="pondRef"  name="file" label-idle="拖拽文件到这里或点击上传(文件小于1MB)" allow-multiple="false"
                accepted-file-types="file/*" :server="serverOptions" :instant-upload="true"
                @processfile="handleUploadSuccess" />
            <div v-if="fileLink != null" class="result-container">
                <el-button @click="sendFile(fileLink)" class="button">
                    {{ "发送" }}
                </el-button>
                <el-button @click="deleteFile(fileLink)" class="button">
                    {{ "撤销" }}
                </el-button>
            </div>
            <!-- <input class="link-result" v-model="fileLink" type="text" disabled /> -->
        </div>

    </el-dialog>

</template>

<script setup>
import { ref ,defineProps} from "vue";
import vueFilePond from "vue-filepond";
import "filepond/dist/filepond.min.css";
import { ElMessage, ElNotification } from "element-plus";
import baseURL from "@/utils/api/baseURL";
import signalRService from "@/services/SignalRService";



// 创建 FilePond 组件
const FilePond = vueFilePond();
const token = localStorage.getItem('userToken')
const fileLink = ref("");
const fileUrls = ref([]);
const dialog = ref(false);

const props = defineProps(["roomId"]);

const pondRef = ref(null)

// 服务器上传配置
const serverOptions = {
    process: {
        url: `${baseURL}/api/fileupload/upload`,
        method: "POST",
        timeout: 7000,
        withCredentials: true,
        headers: {
            Authorization: `Bearer ${token}`, // 如果需要认证
        },
        // onload: (response) => JSON.parse(response).data, // 解析返回的 URL
        onload: (response) => {
            console.log("response", response)
            // 返回的 URL 会赋值给 file.serverId
            const res = JSON.parse(response)
            return res.url
        },
        onerror: (response) => {
            console.error("上传失败返回值：", response)
            return "上传失败"
        }
    }
};

const sendFile = async (fileLink) => {
    await signalRService.sendMessage(props.roomId, fileLink)
    dialog.value = false
}

const deleteFile = async (fileLink) => {
    const response = await fetch(`${baseURL}/api/fileupload/delete?fileLink=${fileLink}`, {
        method: "DELETE",
    })
}

// 处理上传成功事件
const handleUploadSuccess = (error, file) => {
    if (!error) {
        ElMessage.success("上传成功");
        // ElNotification.success({
        //     message: `文件上传成功,文件URL为:${file.serverId}`,
        // });
        console.log("Url", file.serverId)
        fileLink.value = file.serverId
        fileUrls.value.push(file.serverId)

        setTimeout(() => {
            pondRef.value?.removeFile(file.id)
        }, 7000)    // 七秒之后删除小弹窗
    }
    else {
        ElMessage.error("上传失败",error);
    }
};

</script>

<style>
.upload-container {
    width: 400px;
    margin: 20px auto;
}

.result-container {
    width: auto;
    display: flex;
    justify-content: space-around;
    align-items: center;
    gap: 10%;
}

.link-result {
    background-color: #f5f5f5;
    /* color: var(--paper-white); */
    border: 2px solid black;
    padding: 8px 16px;
    border-radius: 4px;
    transition: all 0.3s ease;
    cursor: pointer;
    font-family: Arial, serif;
    font-size: 18px;
    height: auto;
}

.file-container {
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
    align-items: center;
    gap: 10px;
}

.button {
    width: 30%;
}
</style>
