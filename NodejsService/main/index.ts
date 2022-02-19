import express from 'express';
import { Server } from 'http';
import { getAvailablePort } from './util/CommonUtil';
import fs from 'fs';
import path from 'path';
import log4js from 'log4js';
import config from './config';
import router from './router';

// 配置日志
log4js.configure({
  appenders: { cheese: { type: "file", filename: "cheese.log" } },
  categories: {
    default: {
      appenders: ["cheese"], level: config.env == 'development' ? "info" : "warn"
    }
  }
})

const logger = log4js.getLogger("cheese");
const App = express()
let AppServer: Server
let lastCheckTime: number = new Date().getTime() // 上次访问时间

App.use(express.json({ limit: "8192kb" })); // 限制上传
App.use(express.urlencoded({ extended: true }));
App.use('', router.router)

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
    logger.info('start listening ' + port)
  })
  // 将 port 写入文件
  for (const val of process.argv) {
    if (val.startsWith('path=')) {
      let portPath = val.substring('path='.length)
      let portDir = path.resolve(portPath, '..')
      // 检查文件夹是否存在，不存在则创建
      fs.access(portDir, fs.constants.F_OK, err => {
        if (err) {
          fs.mkdirSync(portDir, { recursive: true })
        }
        fs.writeFileSync(portPath, port.toString())
      })
      break;
    }
  }
  // 开发环境则不检查
  if (config.env == 'production') {
    checkConnection()
  }
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