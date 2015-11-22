///<reference path="typings/tsd.d.ts" />

var netstat = require('node-netstat');

import _ = require('lodash');

export default class PortAllocator {
  public static getUnassignedPort(): number {
    var ports = [];

    netstat({
      filter: {
        protocol: 'tcp'
      }
    },
    item => {
      ports.push(item.local.port);
      ports.push(item.remote.port);
    });

    let attempts = 0;

    while (true) {
      var p = PortAllocator.randomPort();

      if (_.contains(ports, p) === false) {
        return p;
      }

      if (++attempts > 1024) {
        break;
      }
    }

    throw new Error('Cannot find an unassigned port to listen on');
  }

  private static randomPort(): number {
    const min = 1024;
    const max = 65535;

    return Math.floor(Math.random() * (max - min) + max);
  }
};
