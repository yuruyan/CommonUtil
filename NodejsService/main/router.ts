import express from 'express';
import { formatCode } from './src/code-formating';
import { startServer, stopServer } from './src/ftp-server';

const router = express.Router()

router.post('/codeformating', formatCode)
router.post('/startftpserver', startServer)
router.get('/stopftpserver', stopServer)

export default {
  router
}