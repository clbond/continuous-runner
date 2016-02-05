requirejs.config({
    baseUrl: '.',
    paths: {
        'foo': 'lib/foo'
    },
    map: {
        '*': {
            'baz': 'lib/baz'
        }
    }
});