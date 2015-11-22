///<reference path="typings/tsd.d.ts" />

import HttpServer from './HttpServer';

import {debug, error} from './logger';

var server = new HttpServer();
try {
  debug('Bootstrapping test runner');

  server.start();

  throw new Error('Not implemented');
}
catch (e) {
  error('Fatal error while bootstrapping test runner; cannot continue', e);
}
finally {
  debug('Stopping HTTP server');

  server.stop();
}
