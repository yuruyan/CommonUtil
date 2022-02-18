import express from 'express';
import { Server } from 'http';
import { formatCode } from './src/code-formating';
import { getAvailablePort } from './util/CommonUtil';
import config from './config';
import fs from 'fs';

const App = express()
let AppServer: Server
let lastCheckTime: number = new Date().getTime() // 上次访问时间

App.use(express.json());
App.use(express.urlencoded({ extended: true }));

App.post('/codeformating', formatCode)
// 心跳机制
App.get('/heartbeat', (req, resp) => {
  lastCheckTime = new Date().getTime()
  // console.log(`heartbeat ${lastCheckTime}`)
  resp.json({
    code: 200,
    message: 'ok'
  })
});

// 启动服务
(async function () {
  let port = await getAvailablePort()
  AppServer = App.listen(port, () => {
    console.log('start listening ' + port)
  })
  // 将 port 写入文件
  for (const val of process.argv) {
    if (val.startsWith('path=')) {
      let path = val.substring('path='.length)
      fs.writeFileSync(path, port.toString())
      break;
    }
  }
  checkConnection()
})()

// 检查客户端是否断开连接
function checkConnection() {
  // 超过此时长未收到 heartbeat 连接，则退出程序
  const ExpireDuration = 30 * 1000
  setInterval(() => {
    if (new Date().getTime() - lastCheckTime > ExpireDuration) {
      App.emit('close')
      AppServer?.close()
      process.exit(1)
    }
    // console.log(`app will exit in ${ExpireDuration - (new Date().getTime() - lastCheckTime)} milliseconds`)
  }, 1000)
}