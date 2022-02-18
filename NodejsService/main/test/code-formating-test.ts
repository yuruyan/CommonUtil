import got from 'got';

(async function test() {
    let resp = await got.post('http://localhost:4000/codeformating', {
        json: {
            'code': "\tconsole.log ( 'hello world')",
            'lang': 'js'
        }
    })
    console.log(resp.body)
})()