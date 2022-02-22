import FtpSrv from 'ftp-srv';

const Port = 21
let ftpServer: FtpSrv

export type Permission = 'r' | 'w'

export interface UserInfo {
  username: string,
  password: string,
  permission: Permission
}

export interface FtpServerConfig {
  root: string,
  anonymous: boolean,
  userlist: UserInfo[],
}

/**
 * 获取禁止的操作
 * @param permission 权限
 */
function getBlacklist(permission: Permission): string[] {
  if (permission == 'r') {
    return ['DELE', 'MKD', 'XMKD', 'RMD', 'XRMD', 'RNFR', 'RNTO', 'STOR', 'STOU']
  } else if (permission == 'w') {
    return []
  } else {
    return getBlacklist('r')
  }
}

/**
 * 启动 ftp server
 * @param config config
 */
export function startServer(config: FtpServerConfig) {
  ftpServer = new FtpSrv({
    url: "ftp://127.0.0.1:" + Port,
    anonymous: config.anonymous
  })

  // 登录操作
  ftpServer.on('login', (data, resolve, reject) => {
    for (const info of config.userlist) {
      if (data.username == info.username && data.password == info.password) {
        return resolve({
          root: config.root,
          blacklist: getBlacklist(info.permission)
        })
      }
    }
    // 匿名用户只读
    if (config.anonymous) {
      return resolve({
        root: config.root,
        blacklist: getBlacklist('r')
      })
    }
    // 登录失败
    reject(new Error('invalid username or password'))
  })
}

/**
 * 关闭 ftp server
 */
export function stopServer() {
  ftpServer?.close()
}

export default {
  startServer,
  stopServer,
}
