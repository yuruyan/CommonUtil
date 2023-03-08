/**
 * FileVO
 */
export class FileVO {
  /**
   * 文件大小，以字节为单位
   */
  fileSize!: number;
  /**
   * 是否是文件夹
   */
  isDir!: boolean;
  /**
   * 文件名
   */
  name!: string;
  /**
   * 父目录，以 '/' 开头
   */
  parentPath!: string;
}

/**
 * JsonResponse
 */
export interface JsonResponse<T> {
  /**
   * 响应码
   */
  code: number;
  /**
   * 数据
   */
  data: T;
  /**
   * 响应信息
   */
  message: string;
}
