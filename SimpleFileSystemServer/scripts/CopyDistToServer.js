/**
 * 拷贝前端打包的 dist 文件夹
 * */

import * as fs from 'fs'

const distFolder = './../../SimpleFileSystemServer-FrontEnd/dist'
// 复制 index.html 为 index.cshtml
fs.copyFileSync(`${distFolder}/index.html`, './../views/index.cshtml')
console.log('copy "index.html" done')
// 复制 assets 文件夹到 wwwroot
fs.rmSync(`./../wwwroot/assets`, { recursive: true })
fs.cpSync(`${distFolder}/assets`, './../wwwroot/assets', { recursive: true })
console.log('copy "assets" to "wwwroot" done')
