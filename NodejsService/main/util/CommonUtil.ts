import http from 'http';
/**
 * 判断对象是否为 Undefined 或 Null
 * @param obj 
 * @returns 
 */
export function isUndefinedOrNull(obj: any): boolean {
  return typeof obj == 'undefined' || obj == null
}

/**
 * 判断 string 是否为空
 * @param obj 
 * @returns 
 */
export function isStringEmpty(obj: string): boolean {
  return obj.trim() == ''
}


/**
 * 获取可用端口
 */
export async function getAvailablePort(): Promise<number> {
  let port = 3001
  // 排除的 port
  let exclusivePorts = new Set([3024, 3128, 3129, 3150, 3210, 3333, 3389, 3700, 3996, 4000, 4060, 4092, 4092, 4321, 4590, 5400, 5401, 5402, 5550, 5569, 5632, 5742, 6267, 6400, 6670, 6671, 6883, 6969, 6970, 7000, 7323, 7626, 7789])
  // 获取可用 port
  for (let p = 3001; p < 8000; p++) {
    if (exclusivePorts.has(p)) {
      continue
    }
    let result = await isPortOccupied(p).catch((e) => { })
    if (result) {
      console.log(`port ${port} is ready`)
      return port
    }
  }
  throw new Error("cannot find an available port");
}

/**
 * 检查端口是否可用
 * @param port port
 * @returns 
 */
function isPortOccupied(port: number): Promise<number | undefined> {
  return new Promise((resolve, reject) => {
    let server = http.createServer().listen(port)
    server.on('listening', () => {
      server.close()
      resolve(port)
    })
    server.on('error', (err) => {
      reject()
    })
  })
}

export default {
  isUndefinedOrNull,
  isStringEmpty,
  getAvailablePort,
}
