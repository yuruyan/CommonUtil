import log4js from 'log4js';
import express from 'express';
import { UserInfo } from './../service/ftp-server';
import ftpServer from './../service/ftp-server';
import { isUndefinedOrNull } from '../util/CommonUtil';
import fs from 'fs';

const logger = log4js.getLogger("cheese");

export function startServer(req: express.Request, resp: express.Response) {
  let root = '', anonymous = false, userInfos = [] as UserInfo[]
  try {
    root = req.body['root'] as string
    anonymous = req.body['anonymous'] as boolean
    if (isUndefinedOrNull(root)) {
      throw new Error("parameter root is missing");
    }
    // 检查访问权限
    fs.accessSync(root, fs.constants.F_OK)
    if (isUndefinedOrNull(anonymous)) {
      anonymous = false
    }
    for (const userInfo of req.body['userlist']) {
      userInfos.push(userInfo)
    }
  } catch (error) {
    logger.info(error)
    resp.json({
      code: 400,
      message: error
    })
  }

  try {
    ftpServer.startServer({
      root: root,
      anonymous: anonymous,
      userlist: userInfos
    })
    resp.json({
      code: 200,
      message: 'success'
    })
  } catch (error) {
    logger.info(error)
    resp.json({
      code: 400,
      message: error
    })
  }

}

export function stopServer(req: express.Request, resp: express.Response) {
  ftpServer.stopServer()
  resp.json({
    code: 200,
    message: 'success'
  })
}