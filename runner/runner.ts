///<reference path="typings/tsd.d.ts" />

import {debug, error} from './logger';

try {
  debug('Bootstrapping test runner');

  throw new Error('Not implemented');
}
catch (e) {
    error('Fatal error while bootstrapping test runner; cannot continue', e);

    process.exit(1);
})