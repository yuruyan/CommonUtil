import express from 'express';
import { format, Lang } from './../service/code-formating';
import { isUndefinedOrNull } from './../util/CommonUtil';

/**
 * 解析字符串为 {Lang}
 * @param s 
 * @returns 
 */
function parseLang(s: string): Lang | null {
  for (const key in Lang) {
    if (isNaN(key as any) && key.toLowerCase() == s.toLowerCase()) {
      return key as any as Lang
    }
  }
  return null
}

export function formatCode(req: express.Request, resp: express.Response) {
  let code = ''
  let lang: Lang | null = null
  // 解析参数
  try {
    code = req.body['code'] as string
    let langParam = req.body['lang'] as string
    if (isUndefinedOrNull(code)) {
      throw new Error("parameter code is missing");
    }
    if (isUndefinedOrNull(langParam)) {
      throw new Error("parameter lang is missing");
    }
    lang = parseLang(langParam)
    if (lang == null) {
      throw new Error(`language ${langParam} is not supported`);
    }
  } catch (error) {
    console.error(error);
    // 错误返回
    resp.json({
      code: 400,
      message: error
    })
    return
  }

  // 格式化
  try {
    resp.json({
      code: 200,
      message: 'success',
      data: format(code, lang)
    })
  } catch (error) {
    console.error(error);
    resp.json({
      code: 401,
      message: '格式化错误'
    })
  }

}
