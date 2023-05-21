/**
 * 创建 MenuItemIconDll
 * 参数：
 *   MenuItemId
 *   MenuItemIconPath
 */

import { fileURLToPath } from 'url'
import * as path from "path"
import { execFileSync } from "child_process"
import { existsSync } from "fs"

const Args = process.argv;
const MenuItemId = Args[2]
const MenuItemIconPath = Args[3]

// Check args
if (Args.length != 4) {
    throw new Error("Arguments error")
}
if (!existsSync(MenuItemIconPath)) {
    throw new Error(`Path '${MenuItemIconPath}' doesn't exist'`)
}

const ThisFileDir = path.dirname(fileURLToPath(import.meta.url))
const SolutionDir = path.join(ThisFileDir, "./../")
const ResourceHackerFile = path.join(SolutionDir, "Tools/ResourceHacker.exe")
const EmptyIconFile = path.join(SolutionDir, "CommonUtil.Data/Resources/EmptyIcon.dll")
const MenuItemsDir = path.join(EmptyIconFile, "./../MenuItems")

console.log("Start executing CreateMenuItemDll")
execFileSync(ResourceHackerFile, [
    "-open",
    EmptyIconFile,
    "-save",
    path.join(MenuItemsDir, `${MenuItemId}.dll`),
    "-resource",
    MenuItemIconPath,
    "-action",
    "add",
    "-mask",
    "ICON,main"
])
console.log("Over")
