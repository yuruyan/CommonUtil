import express from 'express';
import { formatCode } from './src/code-formating';

const router = express.Router()

router.post('/codeformating', formatCode)

export default {
  router
}