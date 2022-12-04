/**
 * 拷贝前端打包的 dist 文件夹，本脚本应由 visual studio 构建前自动执行
 * */

import * as fs from 'fs'
import * as path from 'path'
import { fileURLToPath } from 'url'

const thisFilename = path.basename(fileURLToPath(import.meta.url))
console.log(`-----------------------------${thisFilename}-----------------------------`)
const serverRootFolder = path.join(fileURLToPath(import.meta.url), '../../')
const distFolder = path.join(serverRootFolder, `../SimpleFileSystemServer-FrontEnd/dist`)

console.log(`serverRootFolder: ${serverRootFolder}`)
console.log(`distFolder: ${distFolder}`)

// 复制 index.html 为 index.cshtml
fs.copyFileSync(`${distFolder}/index.html`, path.join(serverRootFolder, 'views/index.cshtml'))
console.log('copy "index.html" done')
// 复制 assets 到项目源代码目录
const copyAssetsToServerSourceDir = () => {
    // 先删除 wwwroot/assets
    fs.rmSync(path.join(serverRootFolder, `wwwroot/assets`), { recursive: true, force: true })
    // 创建 assets 文件夹
    fs.mkdirSync(path.join(serverRootFolder, 'wwwroot/assets'), { recursive: true })
    // 复制 assets 文件夹到 wwwroot
    fs.cpSync(`${distFolder}/assets`, path.join(serverRootFolder, 'wwwroot/assets'), { recursive: true })
    console.log('copy "assets" to sourceDir "wwwroot" done')
}
copyAssetsToServerSourceDir();
