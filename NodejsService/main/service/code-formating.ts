import prettier from 'prettier';

/**
 * 支持的语言
 */
export enum Lang {
  angular,
  css,
  graphql,
  html,
  js,
  json,
  json5,
  less,
  markdown,
  mdx,
  scss,
  typescript,
  vue,
  yaml,
}

/**
 * 格式化
 * @param code 代码
 * @param lang 语言
 * @returns 
 */
export function format(code: string, lang: Lang): string {
  console.log(lang)
  return prettier.format(code, {
    // js 替换为 babel
    parser: lang.toString() == Lang[Lang.js] ? 'babel' : lang
  })
}
