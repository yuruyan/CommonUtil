// 执行编译和打包
// note: 在开始前需要将 config.ts 的 env 修改为 'production'
const childProcess = require('child_process')
const path = require('path')
// 设置工作目录
const cwd = path.resolve(process.cwd(), './../')

console.log('开始编译...')
console.log(childProcess.execSync('yarn run compile', { cwd: cwd, }).toString())
console.log('开始打包...')
console.log(childProcess.execSync('yarn run pack', { cwd: cwd, }).toString())