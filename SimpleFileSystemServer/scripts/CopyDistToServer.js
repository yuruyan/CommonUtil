/**
 * 拷贝前端打包的 dist 文件夹，本脚本应由 visual studio 构建时自动执行
 * */

import * as fs from 'fs'
import * as path from 'path'
import { fileURLToPath } from 'url'

if (typeof (process.argv[2]) == 'undefined') {
    console.error('this file should be executed by visual studio automatically')
    process.exit(0)
}

const thisFilename = path.basename(fileURLToPath(import.meta.url))
console.log(`-----------------------------${thisFilename}-----------------------------`)
const serverRootFolder = path.join(fileURLToPath(import.meta.url), '../../')
const serverOutputFolder = path.join(serverRootFolder, process.argv[2])
const distFolder = path.join(serverRootFolder, `../SimpleFileSystemServer-FrontEnd/dist`)

console.log(`serverRootFolder: ${serverRootFolder}`)
console.log(`serverOutputFolder: ${serverOutputFolder}`)
console.log(`distFolder: ${distFolder}`)

// 复制 index.html 为 index.cshtml
fs.copyFileSync(`${distFolder}/index.html`, path.join(serverRootFolder, 'views/index.cshtml'))
console.log('copy "index.html" done')
// 复制到项目源代码目录
const copyToServerSourceDir = () => {
    // 先删除 wwwroot/assets
    fs.rmSync(path.join(serverRootFolder, `wwwroot/assets`), { recursive: true, force: true })
    // 创建 assets 文件夹
    fs.mkdirSync(path.join(serverRootFolder, 'wwwroot/assets'), { recursive: true })
    // 复制 assets 文件夹到 wwwroot
    fs.cpSync(`${distFolder}/assets`, path.join(serverRootFolder, 'wwwroot/assets'), { recursive: true })
    console.log('copy "assets" to sourceDir "wwwroot" done')
}
// 复制到项目输出目录
const copyToServerOutputDir = () => {
    fs.cpSync(`${distFolder}/assets`, path.join(serverOutputFolder, 'wwwroot/assets'), { recursive: true, force: true })
    console.log('copy "assets" to outputDir "wwwroot" done')
}
copyToServerSourceDir();
copyToServerOutputDir();