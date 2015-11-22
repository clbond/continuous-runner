///<reference path="typings/tsd.d.ts" />

import {formatString, formatStringAndConsume} from './formatString';

import {Logger as PrimaryLogger, addColors, transports} from 'winston';

import _ = require('lodash');

var logger = new PrimaryLogger({
  transports: [
    new transports.File({
      level: 'debug',
      filename: './logs/runner.log',
      handleExceptions: true,
      json: false,
      colorize: false,
      prettyPrint: false
    }),
    new transports.Console({
      level: 'debug',
      handleExceptions: true,
//      humanReadableUnhandledException: true,
      json: false,
      colorize: true,
      prettyPrint: true
    })
  ],
  exitOnError: false
});

//logger.cli();

addColors({
    debug: 'green',
    info:  'cyan',
    warn:  'yellow',
    error: 'red'
});

export default logger;

function debug(format: string, ...args: any[]) {
  logger.debug(formatString(format, ...args));
}

function info(format: string, ...args: any[]) {
  logger.info(formatString(format, ...args));
}

function warn(format: string, ...args: any[]) {
  logger.warn(formatString(format, ...args));
}

function error(format?: string, ...rest: any[]) {
  var args = _.map(Array.prototype.splice.call(arguments, 0),
    arg => {
      if (arg instanceof Error) {
        return arg.stack;
      }
      return arg;
    });

  var formatted = formatStringAndConsume(format, ...args);

  logger.error(formatted[0]);

  for (var i = 1; i < formatted.length; ++i) {
    logger.error(formatted[i]);
  }
}

export {debug, info, warn, error};
