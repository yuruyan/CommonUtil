/**
 * 拷贝前端打包的 dist 文件夹，本脚本应由 visual studio 构建后自动执行
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

// 复制 assets 到项目输出目录
const copyAssetsToServerOutputDir = () => {
    fs.cpSync(`${distFolder}/assets`, path.join(serverOutputFolder, 'wwwroot/assets'), { recursive: true, force: true })
    console.log('copy "assets" to outputDir "wwwroot" done')
}
copyAssetsToServerOutputDir();